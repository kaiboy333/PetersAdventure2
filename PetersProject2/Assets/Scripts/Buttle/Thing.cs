using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Thing
{
    //名前
    public string name { get; protected set; }
    //威力
    public int power { get; private set; }
    //回復するか
    public bool isCure { get; private set; }
    //全体か
    public bool isAll { get; private set; }
    //MP関連か
    public bool isMP { get; private set; }

    public Thing(string name, int power, bool isCure, bool isAll, bool isMP)
    {
        this.name = name;
        this.power = power;
        this.isCure = isCure;
        this.isAll = isAll;
        this.isMP = isMP;
    }
}
