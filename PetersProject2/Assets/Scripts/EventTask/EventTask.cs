using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventTask
{
    //イベント実行中
    public abstract IEnumerator Event();
}
