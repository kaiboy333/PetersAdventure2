using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var buttleEnemyCharas = new List<EnemyChara>();
            if (enemyCharas != null)
            {
                //敵の数を決める
                var enemyNum = Random.Range(enemyNumMin, enemyNumMax + 1);
                for (int i = 0; i < enemyNum; i++)
                {
                    var AddButtleEnemyChara = (EnemyChara)enemyCharas[Random.Range(0, enemyCharas.Count)].Clone();
                    for (int len = buttleEnemyCharas.Count, j = len - 1; j >= 0; j--)
                    {
                        var buttleEnemyChara = buttleEnemyCharas[j];
                        //元が同じ名前のがいたら
                        if (buttleEnemyChara.originalName == AddButtleEnemyChara.originalName)
                        {
                            char alphabet;
                            //今のと元が同じなら
                            if (buttleEnemyChara.name == buttleEnemyChara.originalName)
                            {
                                alphabet = 'A';
                                //リストにあったのをAに
                                buttleEnemyChara.name += alphabet;
                                //追加するのをBにする
                                AddButtleEnemyChara.name += (char)(1 + alphabet);
                            }
                            else
                            {
                                //リストの最後の文字(アルファベット)を取得
                                alphabet = buttleEnemyChara.name[buttleEnemyChara.name.Length - 1];
                                //追加するのをリストのアルファベット+1にする
                                AddButtleEnemyChara.name = AddButtleEnemyChara.originalName + (char)(1 + alphabet);
                            }
                        }
                    }
                    //生成した敵を追加
                    buttleEnemyCharas.Add(AddButtleEnemyChara);
                }
            }
            //ButtleManagerの敵リストにセット
            ButtleManager.enemyCharas = buttleEnemyCharas.Cast<ButtleChara>().ToList();

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
