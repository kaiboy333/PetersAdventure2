using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoNowTask : EventTask
{
    //実行する関数
    private readonly Action doEventFunc = null;

    public DoNowTask(Action doEventFunc)
    {
        this.doEventFunc = doEventFunc;
    }

    protected override bool Event()
    {
        doEventFunc();
        return true;
    }
}
