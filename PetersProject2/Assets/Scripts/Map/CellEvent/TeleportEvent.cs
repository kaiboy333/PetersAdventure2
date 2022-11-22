using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportEvent : CellEvent
{
    //飛ぶシーンの名前
    [SerializeField] private string sceneName;
    //使うパネル
    [SerializeField] private Image blackPanelImage;
    //飛んだ先の位置
    [SerializeField] private Vector2 teleportPosition;

    protected override void Start()
    {
        base.Start();

        //優先順位を高くする
        priorityNo = 1;
    }

    public override IEnumerator CallEvent()
    {
        //動かないようにする
        CharaController.canMove = false;

        if (blackPanelImage)
        {
            //指定位置に移動するように設定
            YushaController.firstPos = teleportPosition;
            //暗くする
            var alphaManager = new AlphaManager(blackPanelImage, false);
            yield return alphaManager.Event();
            //動けるようにする
            CharaController.canMove = true;
            //シーン移動
            SceneManager.LoadScene(sceneName);
        }
    }
}
