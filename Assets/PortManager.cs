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
    #region 扫描端口
    //使用API扫描
    public string[] ScanPorts_API()
    {
        string[] portList = SerialPort.GetPortNames();
        return portList;
    }
    //使用注册表信息扫描
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
    //试错方式扫描
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

    #region 打开串口/关闭串口
    /// <summary>
    /// 打开串口
    /// </summary>
    /// <param name="_portName">端口号</param>
    /// <param name="_baudRate">波特率</param>
    /// <param name="_parity">校验位</param>
    /// <param name="dataBits">数据位</param>
    /// <param name="_stopbits">停止位</param>
    public void OpenSerialPort(string _portName, int _baudRate, Parity _parity, int dataBits, StopBits _stopbits)
    {
        try
        {
            sp = new SerialPort(_portName, _baudRate, _parity, dataBits, _stopbits);//绑定端口
            sp.Open();
            //使用线程
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
    /// 关闭串口
    /// </summary>
    public void CloseSerialPort()
    {
        sp.Close();
    }
    #endregion

    #region 发送数据
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="_info">string数据</param>
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
    /// 发送数据
    /// </summary>
    /// <param name="send">byte数据</param>
    /// <param name="offSet">起始位</param>
    /// <param name="count">byte长度</param>
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

    #region 接收数据与处理信号
    /// <summary>
    /// 接收数据 线程
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
    /// 处理接收到的数据
    /// </summary>
    /// <param name="data">字节数组</param>
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
                // 来自IO板的数据
                Debug.Log(data[2]);
                switch (data[2])
                {
                    case 0x03:
                        // 投币信号
                        if (data[3] == 0x03)
                        {
                            int playerID = data[4];
                            int coinCount = data[5];
                            Debug.Log($"投币信号：玩家 {playerID} 投币 {coinCount} 个");
                        }
                        break;
                    case 0x06:
                        // 退票信号
                        if (data[3] == 0x06)
                        {
                            int playerID = data[4];
                            int ticketCount = data[5];
                            Debug.Log($"退票信号：玩家 {playerID} 退票 {ticketCount} 张");
                        }
                        break;
                    case 0x07:
                        // 系统按键信号
                        if (data[3] == 0x07 && data[4] == 0x00)
                        {
                            int keyData = data[5];
                            Debug.Log($"系统按键信号：{GetSystemKeyName(keyData)}");
                        }
                        break;
                    case 0x17:
                        // 玩家按键信号
                        if (data[3] == 0x17)
                        {
                            int playerID = data[4];
                            int buttonData = data[5];
                            Debug.Log($"玩家按键信号：玩家 {playerID} 按下按钮 {GetPlayerButtonName(buttonData)}");
                        }
                        break;
                    case 0x0E:
                        // 故障返回
                        if (data[3] == 0x0E)
                        {
                            int playerID = data[4];
                            int faultData = data[5];
                            Debug.Log($"故障返回：玩家 {playerID} 设备故障 - {GetFaultDescription(faultData)}");
                        }
                        break;
                }
            }
            else if (data[0] == 0xA3 && data[1] == 0xA9)
            {
                // 来自电脑的数据（目前只有控台按键灯控制和获取IO板软硬件版本有电脑发往IO板的指令，但获取版本信息没有回复处理逻辑，这里只处理控台按键灯控制）
                if (data[2] == 0x0C)
                {
                    // 控台按键灯控制
                    if (data.Length == 6)
                    {
                        int playerID = data[3];
                        int lightMode = data[4];
                        Debug.Log($"控台按键灯控制：玩家 {playerID} 控台按键灯设置为 {GetLightModeName(lightMode)}");
                    }
                }
            }
        }
    }

    private string GetSystemKeyName(int keyData)
    {
        StringBuilder keyName = new StringBuilder();
        if ((keyData & 0x80) > 0) keyName.Append("清除键 ");
        if ((keyData & 0x40) > 0) keyName.Append("确定键 ");
        if ((keyData & 0x20) > 0) keyName.Append("下翻键 ");
        if ((keyData & 0x10) > 0) keyName.Append("上翻键 ");
        if ((keyData & 0x08) > 0) keyName.Append("后台键 ");
        return keyName.ToString().TrimEnd();
    }

    private string GetPlayerButtonName(int buttonData)
    {
        // 这里根据实际游戏中玩家按钮的定义来返回按钮名称，假设简单定义如下（需根据实际情况修改）
        if ((buttonData & 0x80) > 0) return "按钮1";
        if ((buttonData & 0x40) > 0) return "按钮2";
        if ((buttonData & 0x20) > 0) return "按钮3";
        if ((buttonData & 0x10) > 0) return "按钮4";
        if ((buttonData & 0x08) > 0) return "扳机";
        return "未知按钮";
    }

    private string GetFaultDescription(int faultData)
    {
        if ((faultData & 0x04) > 0) return "出彩票超时";
        return "未知故障";
    }

    private string GetLightModeName(int lightMode)
    {
        switch (lightMode)
        {
            case 0: return "灯光常灭";
            case 1: return "灯光常亮";
            case 2: return "灯光一直自动闪烁(2Hz)";
            case 3: return "灯光一直自动闪烁(4Hz)";
            case 4: return "灯光一直自动闪烁(10Hz)";
            default: return "未知灯光模式";
        }
    }
    #endregion
}