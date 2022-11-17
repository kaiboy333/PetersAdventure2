using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogEvent : EventTask
{
    public List<Log> logs = null;
    public bool isButtle = false;

    public LogEvent(LogManager logManager, List<Log> logs, bool isButtle)
    {
        //Logセット
        this.logs = logs;
        this.isButtle = isButtle;
        //表示開始
        logManager.SetLogEvent(this);
    }

    protected override bool Event()
    {
        return isFinished;
    }
}
