using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //味方がいないなら
        if (ButtleManager.friendCharas == null)
        {
            //味方生成
            ButtleManager.friendCharas = new List<ButtleChara>() { FriendEngine.Instance.Get(0) };
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
