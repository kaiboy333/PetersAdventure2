using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkEvent : CellEvent
{
    [SerializeField] private LogManager logManager;
    //会話の内容
    [TextArea, SerializeField] private string talkStr;
    private CharaController charaController = null;

    protected override void Start()
    {
        base.Start();

        cellType = CellType.Check;

        charaController = transform.parent?.GetComponent<CharaController>();
    }

    public override IEnumerator CallEvent()
    {
        //動かないようにする
        CharaController.canMove = false;

        if (logManager)
        {
            //改行で分ける
            var strs = talkStr.Split('\n');
            //Logたちをセット
            var logs = new List<string>();
            foreach(var str in strs)
            {
                logs.Add(str);
            }

            yield return logManager.PrintLog(logs);
        }

        //動けるようにする
        CharaController.canMove = true;
    }
}
