using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogEvent : EventTask
{
    public List<Log> logs = null;
    public bool isButtle = false;
    private LogManager logManager = null;
    private bool isFirst = true;

    public LogEvent(LogManager logManager, List<Log> logs, bool isButtle)
    {
        //Logセット
        this.logs = logs;
        this.isButtle = isButtle;
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
