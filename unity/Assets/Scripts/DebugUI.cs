using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public bool debug;
    private Text _text;
    private string _leftHand;
    private string _rightHand;
    private string _headSet;

    public void SetDebug(string s, int i)
    {
        switch (i)
        {
            case -1:
                _leftHand = s;
                break;
            case 0:
                _headSet = s;
                break;
            case 1:
                _rightHand = s;
                break;
        }
    }

    void Start()
    {
        _text = GetComponent<Text>();
        debug = false;
    }
    void Update()
    {
        if (debug) { 
            _text.text = string.Format("Left hand: {0}\n Right hand: {1}\n Headset: {2}\n", _leftHand, _rightHand, _headSet);
        }
    }
}
