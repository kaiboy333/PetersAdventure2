using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Frag : ScriptableObject
{
    //public bool isFinished { get; private set; }
    //public bool isOn { get; private set; }
    [SerializeField] private bool isFinished;
    [SerializeField] private bool isOn;

    public void Init()
    {
        isFinished = false;
        isOn = false;
    }

    public void Finish()
    {
        isFinished = true;
    }

    public void On()
    {
        isOn = true;
    }

    public void Off()
    {
        isOn = false;
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }

    public bool GetIsOn()
    {
        return isOn;
    }
}
