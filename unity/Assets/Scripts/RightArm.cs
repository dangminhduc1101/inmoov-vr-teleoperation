using UnityEngine;

public class RightArm : MonoBehaviour
{
    [SerializeField] private SerialInterface _serial;
    [SerializeField] private Haptics _haptic;

    private bool _ready;

    private Vector3 _offset;

    private int _pivot, _omo;
    public int Pivot { get => _pivot; }
    public int Omo { get => _omo; }

    void Awake()
    {
        _ready = false;
        _offset = Vector3.zero;
        _pivot = 90;
        _omo = 20;
    }
    private void InverseKinematics(Vector3 pos)
    {
        float _pivotAngle = Vector3.SignedAngle(pos, Vector3.forward, Vector3.right);
        float _omoAngle = Mathf.Abs(Vector3.Angle(Vector3.right, pos) - 90);
        _pivot = _pivotAngle < 0 ? (int)Controller.Map(_pivotAngle, -90, 0, 90, 180) : 150;
        _omo = (int)Controller.Map(_omoAngle, 0, 90, 0, 180);
    }
    public void UpdateOffset(Vector3 vector)
    {
        _offset = vector;
        Debug.Log("Offset: " + vector);
    }
    public void Enable()
    {
        _ready = true;
    }
    public void Disable()
    {
        _pivot = 90;
        _omo = 20;
        _ready = false;
    }
    void Update()
    {
        if (_ready)
        {
            InverseKinematics(transform.position - _offset);
            _serial.SetArm(true, _pivot, _omo);
        }
    }
}
