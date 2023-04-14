using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.InputSystem;

public class Haptics : MonoBehaviour
{
    public void Feedback(float intensity, float duration, int side)
    {
        var command = SendHapticImpulseCommand.Create(0, intensity, duration);
        switch (side)
        {
            case 0:
                if (InputSystem.GetDevice<XRController>(CommonUsages.LeftHand) != null)
                {
                    InputSystem.GetDevice<XRController>(CommonUsages.LeftHand).ExecuteCommand(ref command);
                }
                break;
            case 1:
                if (InputSystem.GetDevice<XRController>(CommonUsages.RightHand) != null)
                {
                    InputSystem.GetDevice<XRController>(CommonUsages.RightHand).ExecuteCommand(ref command);
                }
                break;
            case 2:
                if (InputSystem.GetDevice<XRController>(CommonUsages.LeftHand) != null)
                {
                    InputSystem.GetDevice<XRController>(CommonUsages.LeftHand).ExecuteCommand(ref command);
                }
                if (InputSystem.GetDevice<XRController>(CommonUsages.RightHand) != null)
                {
                    InputSystem.GetDevice<XRController>(CommonUsages.RightHand).ExecuteCommand(ref command);
                }
                break;
        }
       
    }
}
