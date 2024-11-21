using System.IO.Ports;
using UnityEngine;

public class TestPort : MonoBehaviour
{
    PortManager portManager;
    void Start()
    {
        portManager = new PortManager();
        string[] portArray = portManager.ScanPorts_TryFail();//ʹ���Դ��������Խ��COM��ռ������
        portManager.OpenSerialPort(portArray[0], 115200, Parity.None, 8, StopBits.None);
        portManager.SendData("12345");
    }
}
