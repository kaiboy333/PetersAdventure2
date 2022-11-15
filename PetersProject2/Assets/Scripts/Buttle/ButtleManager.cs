using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ButtleManager : MonoBehaviour
{
    [SerializeField] private Image blackPanelImage = null;

    private CommandPanel statusPanel = null;

    public static List<ButtleChara> friendCharas = null;
    public static List<ButtleChara> enemyCharas = null;

    private List<string> buttleStrs = new List<string>();

    public List<ButtleCulculate> buttleCulculates = new List<ButtleCulculate>();

    private bool isFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        //ステータスパネル作成
        statusPanel = CommandManager.Instance.MakeCommandPanel(new List<string> { "?", "HP:?", "MP:?", "Lv:?" }, 4, 1, new Vector2(100, 1000), null, true, true);
        //敵画像生成

        //明るくする
        EventTaskManager.Instance.PushTask(new AlphaManager(blackPanelImage, true));
        //敵名表示(Log)

        //一連のバトルの流れを行う
        ButtleLoop();
    }

    private void Update()
    {
        if (!isFinished)
        {
            if (!EventTaskManager.Instance.IsWorking)
            {
                //一連のバトルの流れを行う
                ButtleLoop();
            }
        }

        //ステータスパネルの更新
        if (statusPanel)
        {

        }
    }

    public void ButtleLoop()
    {
        //コマンドパネル生成、選択が終わるまで進まない
        EventTaskManager.Instance.PushTask(new CommandPanelTask(this));
        //敵の技を決める
        EventTaskManager.Instance.PushTask(new DoNowTask(DesideEnemyTurn));
        //戦闘計算の並び替え
        EventTaskManager.Instance.PushTask(new DoNowTask(() => 
        {
            //素早さの降順で並び替える
            buttleCulculates.Sort((a, b) =>
            {
                return b.speed - a.speed;
            });
        }));
        //バトル開始
        EventTaskManager.Instance.PushTask(new DoNowTask(() =>
        {
            foreach (var buttleCulculate in buttleCulculates)
            {
                EventTaskManager.Instance.PushTask(new DoNowTask(() =>
                {
                    //替えが必要なら
                    if (buttleCulculate.isNeedAlternate)
                    {
                        var buttleChara = buttleCulculate.defences[0];
                        //変えの候補を取得
                        var alliveDefences = buttleChara.isFriend ? GetAlliveChara(friendCharas) : GetAlliveChara(enemyCharas);
                        //誰か替えがいるなら
                        if (alliveDefences.Count != 0)
                        {
                            //最初のやつを変えにする
                            buttleCulculate.defences = new List<ButtleChara>() { alliveDefences[0] };
                        }
                        ////いないなら
                        //else
                        //{
                        //    //イベントを全て消す
                        //    EventTaskManager.Instance.RemoveAll();
                        //}
                    }
                    //バトルの計算
                    buttleCulculate.Culculate();
                }));
            }
        }));
        EventTaskManager.Instance.PushTask(new DoNowTask(() =>
        {
            isFinished = IsFinished(out bool isWin);

            //戦闘が終了なら
            if (isFinished)
            {
                if (isWin)
                {
                    Debug.Log("Win");
                }
                else
                {
                    Debug.Log("Lose");
                }
            }
        }));
    }

    //敵の技を決める
    public void DesideEnemyTurn()
    {

    }

    public List<ButtleChara> GetAlliveChara(List<ButtleChara> buttleCharas)
    {
        var alliveCharas = new List<ButtleChara>();

        foreach (ButtleChara buttleChara in buttleCharas)
        {
            if (!buttleChara.isDead)
            {
                alliveCharas.Add(buttleChara);
            }
        }

        return alliveCharas;
    }

    public List<string> GetCharaName(List<ButtleChara> buttleCharas)
    {
        var charaNames = new List<string>();

        foreach (ButtleChara buttleChara in buttleCharas)
        {
            charaNames.Add(buttleChara.name);
        }

        return charaNames;
    }

    public List<string> GetSkillNames(List<int> keys)
    {
        var skillNames = new List<string>();

        foreach (var key in keys)
        {
            skillNames.Add(SkillEngine.Instance.Get(key).name);
        }

        return skillNames;
    }

    //戦闘終了か
    public bool IsFinished(out bool isWin)
    {
        isWin = false;

        var alliveFriendCharas = GetAlliveChara(friendCharas);
        if(alliveFriendCharas.Count == 0)
        {
            return true;
        }
            
        var alliveEnemyCharas = GetAlliveChara(enemyCharas);
        if (alliveEnemyCharas.Count == 0)
        {
            isWin = true;
            return true;
        }

        return false;
    }
}
