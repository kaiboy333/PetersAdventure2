using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventTask
{
    //public EventTask()
    //{
    //    EventTaskManager.Instance.PushTask(this);
    //}

    ////TaskManagerはここで毎回呼び出す
    //public void UpdateEvent()
    //{
    //    if (isFinished)
    //        return;

    //    //終わったら
    //    if (Event())
    //    {
    //        //trueに
    //        isFinished = true;
    //    }
    //}

    //イベント実行中
    public abstract IEnumerator Event();

    ////終わったか
    //public bool isFinished { get; set; }
}
