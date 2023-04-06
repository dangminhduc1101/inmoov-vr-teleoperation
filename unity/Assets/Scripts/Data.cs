using UnityEngine;
using System.IO;
using System;

[Serializable]
public class Data : MonoBehaviour
{
    public int Participant;
    public string StartingCondition;

    public int Trial;
    public string Condition;
    public bool Completed;
    public double CompletionTime, UnstableTime;
    public double VelocityRight, VelocityLeft, MaxVelocityRight, MaxVelocityLeft;
    public double AcceleratationRight, AccelerationLeft, MaxAccelerationRight, MaxAccelerationLeft;
    public double JerkRight, JerkLeft, MaxJerkRight, MaxJerkLeft;
    public int Frames;

    public double CurrentVelocityRight { get; set; } 
    public double CurrentVelocityLeft { get; set; }
    public double CurrentAccelarationRight { get; set; }
    public double CurrentAccelarationLeft { get; set; }

    public void StartTrial(int trial, string condition)
    {
        Trial = trial;
        Condition = condition;
        CompletionTime = Time.timeAsDouble;
        VelocityLeft = 0;
        VelocityRight = 0;
        MaxVelocityLeft = 0;
        MaxVelocityRight = 0;
        AccelerationLeft = 0;
        AcceleratationRight = 0;
        MaxAccelerationLeft = 0;
        MaxAccelerationRight = 0;
        JerkLeft = 0;
        JerkRight = 0;
        MaxJerkLeft = 0;
        MaxJerkRight = 0;
        Frames = 0;
    }

    public void EndTrial(bool completed)
    {
        Completed = completed;
        CompletionTime = Time.timeAsDouble - CompletionTime;
        VelocityLeft /= Frames;
        VelocityRight /= Frames;
        AccelerationLeft /= Frames;
        AcceleratationRight /= Frames;
        JerkLeft /= Frames;
        JerkRight /= Frames;
        string json = JsonUtility.ToJson(this, true);
        string path = string.Format(@"C:\InMoov\data\participant-{0}.json", Participant);
        File.AppendAllText(path, json);
    }
}
