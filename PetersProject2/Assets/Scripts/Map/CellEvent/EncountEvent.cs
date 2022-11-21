using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EncountEvent : CellEvent
{
    [SerializeField] private int areaNo = 0;
    [SerializeField] private int enemyNumMin = 1;
    [SerializeField] private int enemyNumMax = 3;
    [SerializeField] private float encountRate = 0.05f;

    [SerializeField] private Image blackPanelImage = null;

    private YushaController yushaController = null;

    protected override void Start()
    {
        //base.Start();

        yushaController = FindObjectOfType<YushaController>();
    }

    public override void CallEvent()
    {
        var parcent = Random.Range(0f, 1f);
        //遭遇するなら
        if (parcent <= encountRate)
        {
            //プレイヤーの初期位置を記憶
            YushaController.firstPos = yushaController.gameObject.transform.position;

            //敵を生成
            var enemyCharas = EnemyGernerateEngine.Instance.Get(areaNo);
            var buttleEnemyCharas = new List<ButtleChara>();
            if (enemyCharas != null)
            {
                //敵の数を決める
                var enemyNum = Random.Range(enemyNumMin, enemyNumMax + 1);
                for (int i = 0; i < enemyNum; i++)
                {
                    //生成した敵を追加
                    buttleEnemyCharas.Add(enemyCharas[Random.Range(0, enemyCharas.Count)]);
                }
            }
            //ButtleManagerの敵リストにセット
            ButtleManager.enemyCharas = buttleEnemyCharas;

            //戦闘へ転換する演出をする
            EventTaskManager.Instance.PushTask(new AlphaManager(blackPanelImage, false));

            //シーン移動
            EventTaskManager.Instance.PushTask(new DoNowTask(() =>
            {
                //バトルシーンへ
                SceneManager.LoadScene("Buttle");
            }));
        }
    }
}
