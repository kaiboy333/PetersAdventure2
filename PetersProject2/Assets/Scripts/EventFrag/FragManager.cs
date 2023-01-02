using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FragManager : ScriptableObject
{
    public List<Frag> eventFrags = new List<Frag>();
    [SerializeField] private int index = 0;

    //初期化(最初のフラグのisOnはtrue)
    public void Init()
    {
        for(int i = 0; i < eventFrags.Count; i++)
        {
            var eventFrag = eventFrags[i];
            eventFrag.Init();
            if(i == index)
            {
                eventFrag.On();
            }
        }
    }

    public void NextFragOn()
    {
        if(index < eventFrags.Count - 1)
        {
            //今のイベントが終了しているなら
            if(eventFrags[index].GetIsFinished())
            {
                //新しいフラグをオンにする
                eventFrags[++index].On();
            }
            else
            {
                Debug.LogError("Now Event Not Finished");
            }
        }
        else
        {
            Debug.Log("All Finished");
        }
    }

    private void OnValidate()
    {
        Init();
    }
}
