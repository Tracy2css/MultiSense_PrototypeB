using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class ArduinoFanController : MonoBehaviour
{
    [Header("Serial Port Settings")]
    SerialPort serialPort;
    public string portName = "COM9";      // Set to your Arduino COM port
    public int baudRate = 57600;

    [Header("UI Controls")]
    public Toggle fanToggle;              // Fan power toggle
    public Button increaseButton;         // Increase fan speed button
    public Button decreaseButton;         // Decrease fan speed button

    bool isFanOn = false;

    // Fan IR command strings (same as your WinForms version)
    string UP1 = "s,2232,728,764,712,768,712,732,1408,732,1392,764,696,768,688,764,692,732,772,772,708,740,736,772,704,764,696,736,728,764,696,728,720,764,0,";
    string UP2 = "s,2232,720,772,704,776,704,772,1368,772,1360,740,720,772,684,772,680,776,732,772,1376,772,704,776,1364,772,696,768,1360,764,1352,772,688,760,0,";
    string DOWN2 = "s,2236,716,768,712,772,704,768,1364,772,1368,764,696,768,688,764,684,764,1412,764,1384,764,1376,764,1368,764,1368,764,1360,764,696,764,1348,772,0,";
    string DOWN1 = "s,2232,728,764,712,768,712,732,1408,732,1392,764,696,768,688,764,692,732,772,772,708,740,736,772,704,764,696,736,728,764,696,728,720,764,0,";

    void Start()
    {
        InitializeSerialPort();
        SetupUIListeners();
    }

    void InitializeSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            Debug.Log("Serial port opened: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void SetupUIListeners()
    {
        // Setup Toggle listener
        if (fanToggle != null)
        {
            fanToggle.onValueChanged.AddListener(OnFanToggleChanged);
        }
        else
        {
            Debug.LogWarning("Fan Toggle not assigned!");
        }

        // Setup increase speed button
        if (increaseButton != null)
        {
            increaseButton.onClick.AddListener(OnIncreaseButtonClick);
        }
        else
        {
            Debug.LogWarning("Increase Button not assigned!");
        }

        // Setup decrease speed button
        if (decreaseButton != null)
        {
            decreaseButton.onClick.AddListener(OnDecreaseButtonClick);
        }
        else
        {
            Debug.LogWarning("Decrease Button not assigned!");
        }
    }

    // Called when toggle state changes
    void OnFanToggleChanged(bool isOn)
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        if (isOn && !isFanOn)
        {
            SendCommand(UP1, "Turn On");
            isFanOn = true;
        }
        else if (!isOn && isFanOn)
        {
            SendCommand(DOWN1, "Turn Off");
            isFanOn = false;
        }
    }

    // Increase speed button clicked
    void OnIncreaseButtonClick()
    {
        if (serialPort == null || !serialPort.IsOpen) return;
        SendCommand(UP2, "Increase");
    }

    // Decrease speed button clicked
    void OnDecreaseButtonClick()
    {
        if (serialPort == null || !serialPort.IsOpen) return;
        SendCommand(DOWN2, "Decrease");
    }

    void SendCommand(string command, string action)
    {
        try
        {
            serialPort.Write(command);
            Debug.Log($"Sent [{action}]: {command}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to send command [{action}]: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    void OnDestroy()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }

    // Keep keyboard controls as backup (optional)
    void Update()
    {
        // If UI controls are not set, keep keyboard controls as backup
        if (fanToggle == null || increaseButton == null || decreaseButton == null)
        {
            HandleKeyboardInput();
        }
    }

    void HandleKeyboardInput()
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        // Toggle fan ON/OFF with SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isFanOn)
            {
                SendCommand(UP1, "Turn On");
                isFanOn = true;
            }
            else
            {
                SendCommand(DOWN1, "Turn Off");
                isFanOn = false;
            }
        }
        // Increase airflow with UP arrow
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SendCommand(UP2, "Increase");
        }
        // Decrease airflow with DOWN arrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SendCommand(DOWN2, "Decrease");
        }
    }
}