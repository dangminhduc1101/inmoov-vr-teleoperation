using UnityEngine;
using UnityEngine.InputSystem;

public class RightHand : MonoBehaviour
{
    private InMoov _inMoov;
    private InMoov.RightHandActions _hand;

    private bool _ready;

    [SerializeField] private SerialInterface _serial;

    private int _mrp, _index, _thumb;

    public int Mrp { get => _mrp; }
    public int Index { get => _index; }
    public int Thumb { get => _thumb; }

    void Awake()
    {
        _inMoov = new InMoov();
        _hand = _inMoov.RightHand;
        _mrp = 0;
        _index = 0;
        _thumb = 0;
    }
    void Start()
    {
        _inMoov.Enable();
        _hand.MRPPress.performed += OnMRPChanged;
        _hand.MRPPress.canceled += OnMRPChanged;
        _hand.IndexPress.performed += OnIndexChanged;
        _hand.IndexPress.canceled += OnIndexChanged;
        _hand.ThumbPress.performed += OnThumbChanged;
        _hand.ThumbPress.canceled += OnThumbChanged;
    }
    private void OnMRPChanged(InputAction.CallbackContext c)
    {
        CalculateAngle(_hand.MRPPress, out _mrp);
    }

    private void OnIndexChanged(InputAction.CallbackContext c)
    {
        CalculateAngle(_hand.IndexPress, out _index);
    }
    private void OnThumbChanged(InputAction.CallbackContext c)
    {
        CalculateAngle(_hand.ThumbPress, out _thumb);
    }
    private void CalculateAngle(InputAction press, out int finger)
    {
        finger = press.ReadValue<float>().Equals(1) ? 180 : 0;
        SetAnimatorState();
    }
    private void SetAnimatorState()
    {
        bool thumb = _hand.ThumbPress.ReadValue<float>().Equals(1);
        bool index = _hand.IndexPress.ReadValue<float>().Equals(1);
        bool mrp = _hand.MRPPress.ReadValue<float>().Equals(1);
        int state;
        if (thumb & index & mrp)
        {
            state = 7;
        }
        else if (thumb && mrp)
        {
            state = 6;
        }
        else if (index && mrp)
        {
            state = 5;
        }
        else if (index & thumb)
        {
            state = 4;
        }
        else if (mrp)
        {
            state = 3;
        }
        else if (index)
        {
            state = 2;
        }
        else if (thumb)
        {
            state = 1;
        }
        else
        {
            state = 0;
        }
        GetComponentInChildren<Animator>().SetFloat("State", state);
    }
    public void Enable()
    {
        _ready = true;
    }
    public void Disable()
    {
        _mrp = 0;
        _index = 0;
        _thumb = 0;
        _ready = false;
    }
    void Update()
    {
        if (_ready)
        {
            _serial.SetHand(true, _thumb, _index, _mrp);
        }
    }
}
