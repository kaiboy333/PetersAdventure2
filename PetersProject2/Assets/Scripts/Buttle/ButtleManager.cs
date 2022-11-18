using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ButtleManager : MonoBehaviour
{
    [SerializeField] private Image blackPanelImage = null;

    private CommandPanel statusPanel = null;

    public static List<ButtleChara> friendCharas = new List<ButtleChara>();
    public static List<ButtleChara> enemyCharas = new List<ButtleChara>();

    public List<ButtleCulculate> buttleCulculates = new List<ButtleCulculate>();

    private bool isFinished = false;

    //LogManager
    private LogManager logManager = null;

    // Start is called before the first frame update
    void Start()
    {
        //logManager取得
        logManager = FindObjectOfType<LogManager>();

        //味方生成
        friendCharas.Add(FriendEngine.Instance.Get(0));
        //敵生成
        enemyCharas.Add(EnemyEngine.Instance.Get(0));

        EventTaskManager.Instance.PushTask(new DoNowTask(() =>
        {
            //ステータスパネル作成
            statusPanel = CommandManager.Instance.MakeCommandPanel(new List<string> { "?", "HP:?", "MP:?", "Lv:?" }, 4, 1, new Vector2(100, 1000), null, true, true);
            //敵画像生成
        }));

        //明るくする
        EventTaskManager.Instance.PushTask(new AlphaManager(blackPanelImage, true));

        EventTaskManager.Instance.PushTask(new DoNowTask(() =>
        {
            //敵名表示(Log)
        }));
        //数秒まつ

        EventTaskManager.Instance.PushTask(new DoNowTask(() =>
        {
            //一連のバトルの流れを行う
            ButtleLoop();
        }));
    }

    private void Update()
    {
        //ステータスパネルの更新
        if (statusPanel)
        {

        }
    }

    public void ButtleLoop()
    {
        //生きている仲間を取得
        var alliveFriendCharas = GetAlliveChara(friendCharas);
        //味方の技を決める
        for(int i = 0, len = alliveFriendCharas.Count; i < len; i++)
        {
            var alliveFriendChara = alliveFriendCharas[i];
            bool isFirstMake = i == 0;
            //コマンドパネル生成、選択が終わるまで進まない
            EventTaskManager.Instance.PushTask(new CommandPanelTask(this, (FriendChara)alliveFriendChara, isFirstMake));
        }
        //敵の技を決める
        EventTaskManager.Instance.PushTask(new DoNowTask(DesideEnemyTurn));
        //戦闘計算の並び替え
        EventTaskManager.Instance.PushTask(new DoNowTask(SortButtleCulculate));
        //バトル開始
        EventTaskManager.Instance.PushTask(new DoNowTask(ButtleCulculate));
    }

    //敵の技を決める
    public void DesideEnemyTurn()
    {
        foreach(var enemyChara in GetAlliveChara(enemyCharas))
        {
            //技リストを取得
            var skillKeys = new List<int>();
            skillKeys.AddRange(enemyChara.skillKeys);
            skillKeys.AddRange(enemyChara.magicKeys);

            //技リストからランダムに選ぶ
            var skillKey = skillKeys[Random.Range(0, skillKeys.Count)];
            var skill = SkillEngine.Instance.Get(skillKey);

            List<ButtleChara> defences = null;

            //回復なら
            if (skill.isCure)
            {
                //守備側は敵
                defences = GetAlliveChara(ButtleManager.enemyCharas);
            }
            else
            {
                //守備側は味方
                defences = GetAlliveChara(ButtleManager.friendCharas);
            }

            //全体技なら
            if (skill.isAll)
            {
                buttleCulculates.Add(new ButtleCulculate(enemyChara, defences, skill));
            }
            else
            {
                var defence = defences[Random.Range(0, defences.Count)];

                buttleCulculates.Add(new ButtleCulculate(enemyChara, new List<ButtleChara>() { defence }, skill));
            }
        }
    }

    public void SortButtleCulculate()
    {
        //素早さの降順で並び替える
        buttleCulculates.Sort((a, b) =>
        {
            return b.speed - a.speed;
        });
    }

    public void ButtleCulculate()
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
                }

                //バトルの計算(buttleLogsに入れるだけ)
                var buttleLogs = buttleCulculate.Culculate();
                //buttleLogsをLogEventに入れてログ表示
                EventTaskManager.Instance.PushTask(new LogEvent(logManager, buttleLogs, false));
                //終了チェック
                EventTaskManager.Instance.PushTask(new DoNowTask(() =>
                {
                    //バトルの計算が最後なら
                    if (buttleCulculate == buttleCulculates[buttleCulculates.Count - 1])
                    {
                        //バトル計算のリセット
                        buttleCulculates.Clear();

                        //終了したかをチェック
                        isFinished = IsFinished(out bool isWin);

                        //戦闘が終了なら
                        if (isFinished)
                        {
                            //タスクを全消去
                            EventTaskManager.Instance.RemoveAll();

                            if (isWin)
                            {
                                Debug.Log("Win");
                            }
                            else
                            {
                                Debug.Log("Lose");
                            }
                        }
                        else
                        {
                            Debug.Log("Dont Finish");
                            //一連のバトルの流れを行う
                            ButtleLoop();
                        }
                    }
                }));
            }));
        }
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
