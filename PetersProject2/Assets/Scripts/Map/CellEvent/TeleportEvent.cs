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
    [SerializeField] private Image panelImage;
    ////プレイヤーのScript
    //private YushaController yushaController;
    //飛んだ先の位置
    [SerializeField] private Vector2 teleportPosition;

    protected override void Start()
    {
        base.Start();

        //yushaController = FindObjectOfType<YushaController>();
    }

    public override void CallEvent()
    {
        //タスクマネージャーがあるなら
        if (eventTaskManager && panelImage)
        {
            //タスクマネージャーが動いていないなら
            if (!eventTaskManager.IsWorking)
            {
                //指定位置に移動するように設定
                eventTaskManager.PushTask(new DoNowTask(() => { YushaController.firstPos = teleportPosition; }));
                //暗くする
                eventTaskManager.PushTask(new AlphaManager(panelImage, false));
                //シーン移動
                eventTaskManager.PushTask(new DoNowTask(() => { SceneManager.LoadScene(sceneName); }));
            }
        }
    }
}
