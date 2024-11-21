using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

public class PortManager
{
    private SerialPort sp;
    #region ɨ��˿�
    //ʹ��APIɨ��
    public string[] ScanPorts_API()
    {
        string[] portList = SerialPort.GetPortNames();
        return portList;
    }
    //ʹ��ע�����Ϣɨ��
    public string[] ScanPorts_Regedit()
    {
        RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
        string[] SubKeys = keyCom.GetValueNames();
        string[] portList = new string[SubKeys.Length];
        for (int i = 0; i < SubKeys.Length; i++)
        {
            portList[i] = (string)keyCom.GetValue(SubKeys[i]);
        }
        return portList;
    }
    //�Դ�ʽɨ��
    public string[] ScanPorts_TryFail()
    {
        List<string> tempPost = new List<string>();
        bool mark = false;
        for (int i = 1; i < 10; i++)
        {
            try
            {
                SerialPort sp = new SerialPort("COM" + (i + 1).ToString());
                sp.Open();
                sp.Close();
                tempPost.Add("COM" + (i + 1).ToString());
                mark = true;
            }
            catch (System.Exception)
            {
                continue;
            }

        }
        if (mark)
        {
            string[] portList = tempPost.ToArray();
            return portList;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region �򿪴���/�رմ���
    /// <summary>
    /// �򿪴���
    /// </summary>
    /// <param name="_portName">�˿ں�</param>
    /// <param name="_baudRate">������</param>
    /// <param name="_parity">У��λ</param>
    /// <param name="dataBits">����λ</param>
    /// <param name="_stopbits">ֹͣλ</param>
    public void OpenSerialPort(string _portName, int _baudRate, Parity _parity, int dataBits, StopBits _stopbits)
    {
        try
        {
            sp = new SerialPort(_portName, _baudRate, _parity, dataBits, _stopbits);//�󶨶˿�
            sp.Open();
            //ʹ���߳�
            Thread thread = new Thread(new ThreadStart(DataReceived));
            thread.Start();
        }
        catch (Exception ex)
        {
            sp = new SerialPort();
            Debug.Log(ex);
        }
    }

    /// <summary>
    /// �رմ���
    /// </summary>
    public void CloseSerialPort()
    {
        sp.Close();
    }
    #endregion

    #region ��������
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="_info">string����</param>
    public void SendData(string _info)
    {
        try
        {
            if (sp.IsOpen)
            {
                sp.WriteLine(_info);
            }
            else
            {
                sp.Open();
                sp.WriteLine(_info);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="send">byte����</param>
    /// <param name="offSet">��ʼλ</param>
    /// <param name="count">byte����</param>
    public void SendData(byte[] send, int offSet, int count)
    {
        try
        {
            if (sp.IsOpen)
            {
                sp.Write(send, offSet, count);
            }
            else
            {
                sp.Open();
                sp.Write(send, offSet, count);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    #endregion

    #region ���������봦���ź�
    /// <summary>
    /// �������� �߳�
    /// </summary>
    public void DataReceived()
    {
        while (true)
        {
            if (sp.IsOpen)
            {
                int count = sp.BytesToRead;
                if (count > 0)
                {
                    byte[] readBuffer = new byte[count];
                    try
                    {
                        sp.Read(readBuffer, 0, count);
                        ProcessReceivedData(readBuffer);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }
            }
            Thread.Sleep(10);
        }
    }

    /// <summary>
    /// ������յ�������
    /// </summary>
    /// <param name="data">�ֽ�����</param>
    private void ProcessReceivedData(byte[] data)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sb.AppendFormat("{0:x2}" + "  ", data[i]);
        }
        Debug.Log(sb.ToString());

        if (data.Length >= 6)
        {
            if (data[0] == 0xB5 && data[1] == 0xB6)
            {
                // ����IO�������
                Debug.Log(data[2]);
                switch (data[2])
                {
                    case 0x03:
                        // Ͷ���ź�
                        if (data[3] == 0x03)
                        {
                            int playerID = data[4];
                            int coinCount = data[5];
                            Debug.Log($"Ͷ���źţ���� {playerID} Ͷ�� {coinCount} ��");
                        }
                        break;
                    case 0x06:
                        // ��Ʊ�ź�
                        if (data[3] == 0x06)
                        {
                            int playerID = data[4];
                            int ticketCount = data[5];
                            Debug.Log($"��Ʊ�źţ���� {playerID} ��Ʊ {ticketCount} ��");
                        }
                        break;
                    case 0x07:
                        // ϵͳ�����ź�
                        if (data[3] == 0x07 && data[4] == 0x00)
                        {
                            int keyData = data[5];
                            Debug.Log($"ϵͳ�����źţ�{GetSystemKeyName(keyData)}");
                        }
                        break;
                    case 0x17:
                        // ��Ұ����ź�
                        if (data[3] == 0x17)
                        {
                            int playerID = data[4];
                            int buttonData = data[5];
                            Debug.Log($"��Ұ����źţ���� {playerID} ���°�ť {GetPlayerButtonName(buttonData)}");
                        }
                        break;
                    case 0x0E:
                        // ���Ϸ���
                        if (data[3] == 0x0E)
                        {
                            int playerID = data[4];
                            int faultData = data[5];
                            Debug.Log($"���Ϸ��أ���� {playerID} �豸���� - {GetFaultDescription(faultData)}");
                        }
                        break;
                }
            }
            else if (data[0] == 0xA3 && data[1] == 0xA9)
            {
                // ���Ե��Ե����ݣ�Ŀǰֻ�п�̨�����ƿ��ƺͻ�ȡIO����Ӳ���汾�е��Է���IO���ָ�����ȡ�汾��Ϣû�лظ������߼�������ֻ�����̨�����ƿ��ƣ�
                if (data[2] == 0x0C)
                {
                    // ��̨�����ƿ���
                    if (data.Length == 6)
                    {
                        int playerID = data[3];
                        int lightMode = data[4];
                        Debug.Log($"��̨�����ƿ��ƣ���� {playerID} ��̨����������Ϊ {GetLightModeName(lightMode)}");
                    }
                }
            }
        }
    }

    private string GetSystemKeyName(int keyData)
    {
        StringBuilder keyName = new StringBuilder();
        if ((keyData & 0x80) > 0) keyName.Append("����� ");
        if ((keyData & 0x40) > 0) keyName.Append("ȷ���� ");
        if ((keyData & 0x20) > 0) keyName.Append("�·��� ");
        if ((keyData & 0x10) > 0) keyName.Append("�Ϸ��� ");
        if ((keyData & 0x08) > 0) keyName.Append("��̨�� ");
        return keyName.ToString().TrimEnd();
    }

    private string GetPlayerButtonName(int buttonData)
    {
        // �������ʵ����Ϸ����Ұ�ť�Ķ��������ذ�ť���ƣ�����򵥶������£������ʵ������޸ģ�
        if ((buttonData & 0x80) > 0) return "��ť1";
        if ((buttonData & 0x40) > 0) return "��ť2";
        if ((buttonData & 0x20) > 0) return "��ť3";
        if ((buttonData & 0x10) > 0) return "��ť4";
        if ((buttonData & 0x08) > 0) return "���";
        return "δ֪��ť";
    }

    private string GetFaultDescription(int faultData)
    {
        if ((faultData & 0x04) > 0) return "����Ʊ��ʱ";
        return "δ֪����";
    }

    private string GetLightModeName(int lightMode)
    {
        switch (lightMode)
        {
            case 0: return "�ƹⳣ��";
            case 1: return "�ƹⳣ��";
            case 2: return "�ƹ�һֱ�Զ���˸(2Hz)";
            case 3: return "�ƹ�һֱ�Զ���˸(4Hz)";
            case 4: return "�ƹ�һֱ�Զ���˸(10Hz)";
            default: return "δ֪�ƹ�ģʽ";
        }
    }
    #endregion
}