using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogEvent : EventTask
{
    public string Str { get; private set; }

    public LogEvent(LogManager logManager, string str)
    {
        //文字列セット
        Str = str;
        //表示開始
        logManager.SetLogEvent(this);
    }

    protected override bool Event()
    {
        return isFinished;
    }
}
