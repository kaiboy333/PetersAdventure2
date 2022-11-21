using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogEvent : EventTask
{
    public List<string> logs = null;
    private LogManager logManager = null;
    private bool isFirst = true;

    public LogEvent(LogManager logManager, List<string> logs)
    {
        //Logセット
        this.logs = logs;
        this.logManager = logManager;
    }

    protected override bool Event()
    {
        if (isFirst)
        {
            //表示開始
            logManager.SetLogEvent(this);
            isFirst = false;
        }

        return isFinished;
    }
}
