using System.IO.Ports;
using UnityEditor;
using UnityEngine;

public class TestPort : MonoBehaviour
{
    PortManager portManager;
    void Start()
    {
        asd();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            portManager.CloseSerialPort();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            asd();
        }
    }

    void asd()
    {
        portManager = new PortManager();
        string[] portArray = portManager.ScanPorts_TryFail();//使用试错函数，可以解决COM被占用问题
        portManager.OpenSerialPort(portArray[1], 9600, Parity.None, 8, StopBits.None);
        portManager.SendData("13254");
    }

    private void OnDestroy()
    {
        portManager.CloseSerialPort();
    }
}
