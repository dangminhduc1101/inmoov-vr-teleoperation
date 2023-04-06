using UnityEngine;
using System.Collections.Generic;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private Material[] _materials = new Material[10];
    private readonly Dictionary<int, int[]> _allPuzzles = new() { 
        { 0, new int[] {180, 180, 180, 0, 0, 0, 90, 30, 90, 30} },
        { 1, new int[] {0, 0, 0, 180, 180, 180, 90, 30, 90, 30} },
        { 2, new int[] {180, 180, 180, 180, 180, 180, 120, 20, 90, 20} },
        { 3, new int[] {0, 0, 0, 0, 0, 0, 90, 20, 140, 20} },
        { 4, new int[] {0, 180, 0, 0, 0, 180, 90, 120, 90, 120} },
        { 5, new int[] {180, 180, 0, 0, 0, 0, 90, 20, 90, 120 } },
        { 6, new int[] { 180, 0, 180, 0, 180, 0, 135, 20, 90, 135 } },
        { 7, new int[] { 0, 180, 0, 180, 0, 180, 135, 120, 90, 60 } },
        { 8, new int[] { 180, 0, 0, 180, 0, 0, 135, 20, 135, 20 } },
        { 9, new int[] { 0, 0, 180, 0, 0, 180, 90, 20, 140, 140 } }
    };

    private int[] _currentPuzzle;
    
    public void SwitchPuzzle(int i) {
        GetComponent<Renderer>().material = _materials[i];
        if (!_allPuzzles.TryGetValue(i, out _currentPuzzle)) {
            Debug.Log("Error retrieving puzzle");
        }
    }

    public void PuzzleCompleted(LeftHand leftHand, RightHand rightHand, LeftArm leftArm, RightArm rightArm, out bool puzzleCompleted) {
        bool leftFingers = leftHand.Thumb == _currentPuzzle[0] && leftHand.Index == _currentPuzzle[1] && leftHand.Mrp == _currentPuzzle[2];
        bool rightFingers = rightHand.Thumb == _currentPuzzle[3] && rightHand.Index == _currentPuzzle[4] && rightHand.Mrp == _currentPuzzle[5];
        double leftJoints = Mathf.Sqrt(Mathf.Pow(leftArm.Pivot - _currentPuzzle[6], 2) + Mathf.Pow(leftArm.Omo - _currentPuzzle[7], 2));
        double rightJoints = Mathf.Sqrt(Mathf.Pow(rightArm.Pivot - _currentPuzzle[8], 2) + Mathf.Pow(rightArm.Omo - _currentPuzzle[9], 2));
        puzzleCompleted = leftFingers && rightFingers && leftJoints <= 30 && rightJoints <= 30;
    }
}
