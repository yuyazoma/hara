using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    string myPortName = "\\\\.\\COM3";
    public int bitRate = 115200;

    public string portName;
    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    void Awake()
    {
        portName = myPortName;
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_ && OnDataReceived != null)
        {
            OnDataReceived(message_);
            isNewMessageReceived_ = false;
        }
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        try
        {
            serialPort_ = new SerialPort(portName, bitRate, Parity.None, 8, StopBits.One);
            serialPort_.RtsEnable = true;
            serialPort_.DtrEnable = true;

            serialPort_.Open();
            isRunning_ = true;

            thread_ = new Thread(Read);
            thread_.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to open serial port {portName}: {e.Message}");
        }
    }

    private void Close()
    {
        isNewMessageReceived_ = false;
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join(); // 安全にスレッドを終了
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                message_ = serialPort_.ReadLine();
                isNewMessageReceived_ = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(string message)
    {
        try
        {
            if (serialPort_ != null && serialPort_.IsOpen)
            {
                serialPort_.DiscardOutBuffer(); // 🔹 バッファをクリア
                serialPort_.Write(message);
                serialPort_.BaseStream.Flush(); // 🔹 即時送信を保証
                Debug.Log($"Sent to Arduino: {message}");
            }
            else
            {
                Debug.LogWarning("Serial port is not open!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Serial write error: {e.Message}");
        }
    }
}