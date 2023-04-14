using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public bool Test;

    [SerializeField] private GameObject _leftController, _rightController;
    [SerializeField] private Puzzle _puzzle;
    [SerializeField] private SerialInterface _serial;
    [SerializeField] private Haptics _haptics;
    [SerializeField] private Data _data;
    [SerializeField] private DataDump _dataDump;

    [SerializeField] private int _participantNumber;
    [SerializeField] private bool _startingConditionContinuous;

    private const float INITIALIZE_TIME = 300.0f;
    private const int NUM_TRIAL = 10;

    private LeftArm _leftArm;
    private LeftHand _leftHand;
    private RightArm _rightArm;
    private RightHand _rightHand;
    private InMoov.LeftHandActions _leftHandActions;
    private InMoov.RightHandActions _rightHandActions;
    private Vector3 _leftDown, _leftUp, _leftForward, _leftSide, _rightDown, _rightUp, _rightForward, _rightSide;

    private bool _ready;
    private bool _puzzleCompleted;
    private bool _currentConditionContinuous;
    private bool _leftUnstable, _rightUnstable;

    private void Awake()
    {
        _leftDown = Vector3.zero;
        _leftUp = Vector3.zero;
        _leftForward = Vector3.zero;
        _leftSide = Vector3.zero;
        _rightDown = Vector3.zero;
        _rightUp = Vector3.zero;
        _rightForward = Vector3.zero;
        _rightSide = Vector3.zero;

        _leftArm = _leftController.GetComponent<LeftArm>();
        _leftHand = _leftController.GetComponent<LeftHand>();
        _rightArm = _rightController.GetComponent<RightArm>();
        _rightHand = _rightController.GetComponent<RightHand>();

        InMoov inMoov = new();
        inMoov.Enable();
        _leftHandActions = inMoov.LeftHand;
        _rightHandActions = inMoov.RightHand;

        _ready = false;
        _puzzleCompleted = false;
        _currentConditionContinuous = _startingConditionContinuous;

        _leftUnstable = false;
        _rightUnstable = false;
    }
    IEnumerator Start()
    {
        yield return StartCoroutine(InputInformation());
        yield return StartCoroutine(Initialize((l, r) => { _leftDown = l; _rightDown = r; }, Keyboard.current.digit1Key));
        yield return StartCoroutine(Initialize((l, r) => { _leftUp = l; _rightUp = r; }, Keyboard.current.digit2Key));
        yield return StartCoroutine(Initialize((l, r) => { _leftForward = l; _rightForward = r; }, Keyboard.current.digit3Key));
        yield return StartCoroutine(Initialize((l, r) => { _leftSide = l; _rightSide = r; }, Keyboard.current.digit4Key));
        yield return StartCoroutine(Ready());
        for (int i = 0; i < NUM_TRIAL; i++)
        {
            yield return StartCoroutine(Begin(i));
            yield return StartCoroutine(Finish());
        }
        yield return StartCoroutine(Done());
    }

    private IEnumerator InputInformation()
    {
        Debug.Log("Make sure that the controller's test value is set to False!!");
        Debug.Log("Please enter the current participant number in the Unity interface of this controller, then press J.");
        yield return new WaitUntil(() => Keyboard.current.jKey.wasPressedThisFrame);
        Debug.Log("The current participant number is " + _participantNumber + ".");
       
        Debug.Log("Please enter the starting condition for this participant in the Unity interface of this controller, then press K.");
        yield return new WaitUntil(() => Keyboard.current.kKey.wasPressedThisFrame);
        string condition = _startingConditionContinuous ? "Continuous" : "Discrete";
        Debug.Log("The current starting condition is " + condition + ".");
        
        Debug.Log(string.Format("Participant number is {0} and starting condition is {1}. If this is correct, press L to start initialization."
            , _participantNumber, condition));
        yield return new WaitUntil(() => Keyboard.current.lKey.wasPressedThisFrame);
        _data.Participant = _participantNumber;
        _dataDump.Participant = _participantNumber;
        _data.StartingCondition = condition;
    }

    private IEnumerator Initialize(System.Action<Vector3, Vector3> reference, UnityEngine.InputSystem.Controls.KeyControl key)
    {
        int number = (int)key.keyCode - 40;
        string direction = number switch
        {
            1 => "Down",
            2 => "Up",
            3 => "Forward",
            4 => "Side",
            _ => "ERROR!"
        };
        Debug.Log(string.Format("Press {0} to start initializing the {1} direction.", number, direction));

        yield return new WaitUntil(() => key.wasPressedThisFrame);
        _haptics.Feedback(0.1f, 0.3f, 2);
        Debug.Log("Key pressed, starting initialization!");

        Vector3 left = Vector3.zero;
        Vector3 right = Vector3.zero;
        int i = 0;
        float time = INITIALIZE_TIME;
        while (time >= 0)
        {
            left += _leftController.transform.position;
            right += _rightController.transform.position;
            i++;
            time -= Time.deltaTime;
        }
        left /= i;
        right /= i;
        reference(left, right);
        Debug.Log(string.Format("Initialization done. Left Vector: {0}; Right Vector: {1}", left, right));
    }

    private IEnumerator Ready()
    {
        Debug.Log("Press 0 to compute offsets.");
        yield return new WaitUntil(() => Keyboard.current.digit0Key.wasPressedThisFrame);

        Debug.Log(string.Format("Left values: {0}, {1}, {2}, {3}", _leftDown, _leftUp, _leftForward, _leftSide));
        Vector3 left = (_leftDown + _leftUp) / 2;
        left.y = left.y * 0.6f + _leftForward.y * 0.2f + _leftSide.y * 0.2f;
        left.x = left.x * 0.65f + _leftForward.x * 0.35f;
        left.z = left.z * 0.65f + _leftSide.z * 0.35f;
        Debug.Log(string.Format("Left offset: {0}", left));

        Debug.Log(string.Format("Right values: {0}, {1}, {2}, {3}", _rightDown, _rightUp, _rightForward, _rightSide));
        Vector3 right = (_rightDown + _rightUp) / 2;
        left.y = left.y * 0.6f + _rightForward.y * 0.2f + _rightSide.y * 0.2f;
        left.x = left.x * 0.65f + _rightForward.x * 0.35f;
        left.z = left.z * 0.65f + _rightSide.z * 0.35f;
        Debug.Log(string.Format("Right offset: {0}", right));

        _leftArm.UpdateOffset(left);
        _rightArm.UpdateOffset(right);
    }

    private IEnumerator Begin(int i)
    {
        Debug.Log("Press SPACE to begin Trial " + (i + 1));
        _currentConditionContinuous = i < NUM_TRIAL / 2 ? _startingConditionContinuous : !_startingConditionContinuous;

        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        _leftArm.Enable();
        _leftHand.Enable();
        _rightArm.Enable();
        _rightHand.Enable();
        _serial.Enable(test: Test);
        _ready = true;
        _puzzle.SwitchPuzzle(i);

        _dataDump.Trial = i;
        _dataDump.Condition = _currentConditionContinuous ? "Continuous" : "Discrete";
        _data.StartTrial(i, _currentConditionContinuous ? "Continuous" : "Discrete");
        _haptics.Feedback(0.5f, 0.3f, 2);
        Debug.Log("Trial started!");
    }

    private IEnumerator Finish()
    {
        Debug.Log("Press BackSpace to end the task early.");
        yield return new WaitUntil(() => Keyboard.current.backspaceKey.wasPressedThisFrame || _puzzleCompleted);
        _haptics.Feedback(0.5f, 0.3f, 2);
        _leftArm.Disable();
        _leftHand.Disable();
        _rightArm.Disable();
        _rightHand.Disable();
        _serial.Disable();

        _data.EndTrial(_puzzleCompleted);
        _dataDump.End();
        _puzzleCompleted = false;
        _ready = false;
        Debug.Log("Trial ended!");
    }

    private IEnumerator Done()
    {
        Debug.Log("The experiment is finished. Press SPACE to exit.");
        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        UnityEditor.EditorApplication.ExitPlaymode();
    }

    public static float Map(float value, float from1, float to1, float from2, float to2)
    {
        if (value < from1)
        {
            return from2;
        }
        if (value > to1)
        {
            return to2;
        }
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void MonitorStability(float pivot, float omo, bool left)
    {
        if (left)
        {
            _leftUnstable = pivot > 150 || omo > 150;
        }
        else
        {
            _rightUnstable = pivot > 150 || omo > 150;
        }

        if (_currentConditionContinuous)
        {
            float intensity = Map(pivot, 90.0f, 180.0f, 0.0f, 0.3f) + Map(omo, 0.0f, 180.0f, 0.0f, 0.3f);
            _haptics.Feedback(intensity, 0.1f, left ? 0 : 1);
        }
        else
        {
            if (_leftUnstable)
            {
                _haptics.Feedback(0.6f, 0.1f, 0);
            } 
            if (_rightUnstable)
            {
                _haptics.Feedback(0.6f, 0.1f, 1);
            }
        }
    }

    void Update()
    {
        if (_ready)
        {
            MonitorStability(_leftArm.Pivot, _leftArm.Omo, left: true);
            MonitorStability(_rightArm.Pivot, _rightArm.Omo, left: false);
            _puzzle.PuzzleCompleted(_leftHand, _rightHand, _leftArm, _rightArm, out _puzzleCompleted);
            if (_leftUnstable || _rightUnstable)
            {
                _data.UnstableTime += Time.deltaTime;
            }
            double velocityLeft = _leftHandActions.Velocity.ReadValue<Vector3>().magnitude;
            double velocityRight = _rightHandActions.Velocity.ReadValue<Vector3>().magnitude;
            double accelLeft = (velocityLeft - _data.CurrentVelocityLeft) / Time.deltaTime;
            double accelRight = (velocityRight - _data.CurrentVelocityRight) / Time.deltaTime;
            double jerkLeft = (accelLeft - _data.CurrentAccelarationLeft) / Time.deltaTime;
            double jerkRight = (accelRight - _data.CurrentAccelarationRight) / Time.deltaTime;
            _data.MaxJerkLeft = Math.Abs(jerkLeft) > Math.Abs(_data.MaxJerkLeft) ? jerkLeft : _data.MaxJerkLeft;
            _data.MaxJerkRight = Math.Abs(jerkRight) > Math.Abs(_data.MaxJerkRight) ? jerkRight : _data.MaxJerkRight;
            _data.MaxAccelerationLeft = Math.Abs(accelLeft) > Math.Abs(_data.MaxAccelerationLeft) ? accelLeft : _data.MaxAccelerationLeft;
            _data.MaxAccelerationRight = Math.Abs(accelRight) > Math.Abs(_data.MaxAccelerationRight) ? accelRight : _data.MaxAccelerationRight;
            _data.MaxVelocityLeft = Math.Abs(velocityLeft) > Math.Abs(_data.MaxVelocityLeft) ? velocityLeft : _data.MaxVelocityLeft;
            _data.MaxVelocityRight = Math.Abs(velocityRight) > Math.Abs(_data.MaxVelocityRight) ? velocityRight : _data.MaxVelocityRight;
            _data.CurrentVelocityLeft = velocityLeft;
            _data.CurrentAccelarationRight = velocityRight;
            _data.CurrentAccelarationLeft = accelLeft;
            _data.CurrentAccelarationRight = accelRight;

            _data.VelocityLeft += Math.Abs(velocityLeft);
            _data.VelocityRight += Math.Abs(velocityRight);
            _data.AccelerationLeft += Math.Abs(accelLeft);
            _data.AcceleratationRight += Math.Abs(accelRight);
            _data.JerkLeft += Math.Abs(jerkLeft);
            _data.JerkRight += Math.Abs(jerkRight);
            _data.Frames++;
            _dataDump.Dump(_leftController, _rightController);
        }
    }
}
