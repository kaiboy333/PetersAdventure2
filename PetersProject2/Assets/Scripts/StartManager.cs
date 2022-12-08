using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //味方がいないなら
        if (ButtleManager.friendCharas.Count == 0)
        {
            //味方生成
            ButtleManager.friendCharas.Add(FriendEngine.Instance.Get(0));
            ButtleManager.friendCharas.Add(FriendEngine.Instance.Get(1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
