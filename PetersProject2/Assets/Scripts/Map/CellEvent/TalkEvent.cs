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
            //改行で分ける
            var strs = talkStr.Split('\n');
            //Logたちをセット
            var logs = new List<string>();
            foreach(var str in strs)
            {
                logs.Add(str);
            }

            eventTaskManager.PushTask(new LogEvent(logManager, logs));
        }
    }
}
