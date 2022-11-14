using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkEvent : CellEvent
{
    [SerializeField] private LogManager logManager;
    //会話の内容
    [TextArea, SerializeField] private string talkStr;

    protected override void Start()
    {
        base.Start();

        cellType = CellType.Check;
    }

    public override void CallEvent()
    {
        if (eventTaskManager && logManager)
        {
            eventTaskManager.PushTask(new LogEvent(logManager, talkStr));
        }
    }
}
