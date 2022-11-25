using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Thing : ICloneable
{
    //名前
    public string name { get; protected set; }

    public Thing(string name)
    {
        this.name = name;
    }

    public object Clone()
    {
        var thing = (Thing)this.MemberwiseClone();
        return thing;
    }
}
