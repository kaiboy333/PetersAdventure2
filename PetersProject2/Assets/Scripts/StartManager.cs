using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Image blackPanelImage = null;
    // Start is called before the first frame update
    void Awake()
    {
        //味方がいないなら
        if (ButtleManager.friendCharas.Count == 0)
        {
            //味方生成
            ButtleManager.friendCharas.Add(FriendEngine.Instance.Get(0));
        }
    }

    private IEnumerator Start()
    {
        var alphaManager = new AlphaManager(blackPanelImage, true);
        yield return alphaManager.Event();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
