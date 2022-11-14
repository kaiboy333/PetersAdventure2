using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventTask
{
    //TaskManagerはここで毎回呼び出す
    public void UpdateEvent()
    {
        if (isFinished)
            return;

        //終わったら
        if (Event())
        {
            //trueに
            isFinished = true;
        }
    }

    //イベント実行中
    protected abstract bool Event();

    //終わったか
    public bool isFinished { get; set; }
}
