using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialPortManager : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning;

    public string[] availablePorts;

    void Start()
    {
        ScanPorts();
    }

    void ScanPorts()
    {
        availablePorts = SerialPort.GetPortNames();
        foreach (string port in availablePorts)
        {
            Debug.Log("Available Port: " + port);
        }
    }

    public void Connect(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            serialPort.Open();
            isRunning = true;
            readThread = new Thread(Read);
            readThread.Start();
            Debug.Log("Connected to " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to port: " + e.Message);
        }
    }

    private void Read()
    {
        while (isRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    OnDataReceived(message);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading data: " + e.Message);
                }
            }
        }
    }

    public void OnDataReceived(string message)
    {
        Debug.Log("Data Received: " + message);
        // Handle the received data here (e.g., invoke an event or a delegate)
    }

    public void SendMessage(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine(message);
                Debug.Log("Sent: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending data: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open.");
        }
    }

    public void Disconnect()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            isRunning = false;
            readThread.Abort();
            serialPort.Close();
            Debug.Log("Disconnected from port.");
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
