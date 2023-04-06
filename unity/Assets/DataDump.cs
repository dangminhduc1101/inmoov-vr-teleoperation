using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

[Serializable]
public class DataDump : MonoBehaviour
{
    public int Participant { get; set; }
    public int Trial;
    public string Condition;
    public float LeftX, LeftY, LeftZ, RightX, RightY, RightZ;
    public List<float> LeftXList { get; set; }
    public List<float> LeftYList { get; set; } 
    public List<float> LeftZList { get; set; } 
    public List<float> RightXList { get; set; } 
    public List<float> RightYList { get; set; } 
    public List<float> RightZList { get; set; }

    public void Awake()
    {
        Reset();
    }
    private void Reset()
    {
        LeftXList = new List<float>();
        LeftYList = new List<float>();
        LeftZList = new List<float>();
        RightXList = new List<float>();
        RightZList = new List<float>();
        RightYList = new List<float>();
    }

    public void Dump(GameObject leftHand, GameObject rightHand)
    {
        LeftXList.Add(leftHand.transform.position.x);
        LeftYList.Add(leftHand.transform.position.y);
        LeftZList.Add(leftHand.transform.position.z);
        RightXList.Add(rightHand.transform.position.x);
        RightYList.Add(rightHand.transform.position.y);
        RightZList.Add(rightHand.transform.position.z);
    }

    public void End()
    {
        for (int i = 0; i < Mathf.Min(LeftXList.Count, LeftYList.Count, LeftZList.Count, RightXList.Count, RightYList.Count, RightZList.Count); i++)
        {
            LeftX = LeftXList[i];
            LeftY = LeftYList[i];
            LeftZ = LeftZList[i];
            RightX = RightXList[i];
            RightY = RightYList[i];
            RightZ = RightZList[i];
            string json = JsonUtility.ToJson(this, true);
            string path = string.Format(@"C:\InMoov\datadump\datadump-participant-{0}.json", Participant);
            File.AppendAllText(path, json);
        }
        Reset();
    }



}
