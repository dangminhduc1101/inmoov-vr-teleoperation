using System;
using System.IO.Ports;
using UnityEngine;

public class SerialInterface : MonoBehaviour
{
    private SerialPort _serial;

    private string _serialText;

    private bool _ready;
    private bool _test;

    private int _leftThumb, _leftIndex, _leftMRP, _rightThumb, _rightIndex, _rightMRP;
    private int _leftPivot, _leftOmo, _rightPivot, _rightOmo;

    private void Awake()
    {
        _serial = new SerialPort("COM3", 115200);
        _ready = false;
    }
    void Start()
    {
        try { 
            _serial.Open(); 
        }
        catch (System.IO.IOException)
        {
            Debug.Log("Serial Port not connected.");
        }
    }

    public void SetHand(bool right, int t, int i, int mrp)
    {
        if (right)
        {
            _rightThumb = t;
            _rightIndex = i;
            _rightMRP = mrp;
        }
        else
        {
            _leftThumb = t;
            _leftIndex = i;
            _leftMRP = mrp;
        }
    }

    public void SetArm(bool right, int p, int o)
    {
        if (right)
        {
            _rightPivot = p;
            _rightOmo = o;
        }
        else 
        {
            _leftPivot = p;
            _leftOmo = o;
        }
    }

    public void Enable(bool test)
    {
        _ready = true;
        _test = test;
    }
    public void Disable()
    {
        try
        {
            _serial.Write("180.180.180.0.0.0.90.20.90.20\n");
        } catch (InvalidOperationException)
        {
            Debug.Log("Testing, Serial Port not connected.");
        }
        _ready = false;
    }

    void Update()
    {
        if (_ready)
        {
            _serialText = string.Join('.', _leftThumb, _leftIndex, _leftMRP, _rightThumb, _rightIndex, _rightMRP,
                _leftPivot, _leftOmo, _rightPivot, _rightOmo) + '\n';
            if (_test)
            {
                Debug.Log(_serialText);
            }
            else
            {
                try
                {
                    _serial.Write(_serialText);
                }
                catch (System.InvalidOperationException)
                {
                    Debug.Log("Serial Port error!");
                    _ready = false;
                }
            }
        }
    }
}
