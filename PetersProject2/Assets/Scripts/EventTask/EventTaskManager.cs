using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTaskManager : SingletonMonoBehaviour<EventTaskManager>
{
    //先入れ先出しのイベントたち
    private Queue<EventTask> tasks = new Queue<EventTask>();
    private EventTask nowtask = null;

    //動いているか
    public bool IsWorking
    {
        get { return nowtask != null; }
    }

    protected override bool dontDestroyOnLoad => true;

    // Update is called once per frame
    void Update()
    {
        //タスクがあるなら
        if (nowtask != null)
        {
            //タスクのイベントを行う
            nowtask.UpdateEvent();
            //終わったなら
            if (nowtask.isFinished)
            {
                nowtask = null;
            }
        }

        //タスクがないなら
        if(nowtask == null)
        {
            //次のタスクがあるなら
            if (tasks.Count != 0)
            {
                //先頭を取り出す
                nowtask = tasks.Dequeue();
            }
        }
    }

    public void PushTask(EventTask task)
    {
        //リストに追加
        tasks.Enqueue(task);
    }

    //全てを消す
    public void RemoveAll()
    {
        tasks.Clear();
    }
}
