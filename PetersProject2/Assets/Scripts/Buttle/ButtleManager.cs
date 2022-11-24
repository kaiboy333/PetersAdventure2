using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class ButtleManager : MonoBehaviour
{
    [SerializeField] private Image blackPanelImage = null;

    private CommandPanel statusPanel = null;

    public static List<ButtleChara> friendCharas = null;
    public static List<ButtleChara> enemyCharas = null;

    public List<ButtleCulculate> buttleCulculates = new List<ButtleCulculate>();

    private bool isFinished = false;

    //LogManager
    [SerializeField] private LogManager logManager = null;

    //バトルログのインターバル
    public const float BUTTLE_LOG_INTERVAL = 1;

    //バトル中か
    private bool isButtle = false;

    [SerializeField] private GameObject enemyPrefab = null;

    [SerializeField] private RectTransform backGroundRect = null;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        //味方がいないなら
        if(friendCharas == null)
        {
            //味方生成
            friendCharas = new List<ButtleChara>() { FriendEngine.Instance.Get(0) };
        }
        //敵がいないなら
        if (enemyCharas == null)
        {
            //敵生成
            enemyCharas = new List<ButtleChara>() { EnemyEngine.Instance.Get(0) };
        }

        var sumWidth = 0;
        //敵画像生成
        foreach(var enemyChara in enemyCharas)
        {
            var castedEnemyChara = (EnemyChara)enemyChara;

            //敵画像のゲームオブジェクト生成
            GameObject enemyObj = Instantiate(enemyPrefab, backGroundRect);
            //敵画像をセット
            enemyObj.GetComponent<Image>().sprite = castedEnemyChara.sprite;
            //縦、横幅セット
            enemyObj.GetComponent<RectTransform>().sizeDelta = new Vector2(castedEnemyChara.width, castedEnemyChara.height);
            //横幅を加算
            sumWidth += castedEnemyChara.width;

            //敵クラスに登録
            castedEnemyChara.enemyObj = enemyObj;
        }
        var xPos = -sumWidth / 2;
        foreach (var enemyChara in enemyCharas)
        {
            var castedEnemyChara = (EnemyChara)enemyChara;

            //敵画像のゲームオブジェクト取得
            GameObject enemyObj = castedEnemyChara.enemyObj;
            //敵画像のx位置を調整
            var rect = enemyObj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(xPos, rect.anchoredPosition.y);
            //横幅分だけ右に移動
            xPos += (int)rect.sizeDelta.x;
        }

        var allStrs = new List<List<string>>();
        //ステータスパネル作成
        for (int i = 0; i < friendCharas.Count; i++)
        {
            var friendChara = friendCharas[i];
            var strs = new List<string>() { friendChara.name, "HP:" + friendChara.hp.ToString(), "MP:" + friendChara.mp.ToString(), "Lv:" + ((FriendChara)friendChara).level.ToString() };
            allStrs.Add(strs);
        }
        var strs_t = new List<string>();
        for (int i = 0, len = allStrs[0].Count; i < len; i++)
        {
            for (int j = 0, len2 = allStrs.Count; j < len2; j++)
            {
                var str = allStrs[j][i];
                strs_t.Add(str);
            }
        }
        statusPanel = CommandManager.Instance.MakeCommandPanel(strs_t, 4, strs_t.Count / 4, new Vector2(100, 1000), null, true, true, backGroundRect);
        //ステータスパネルの名前変更
        statusPanel.gameObject.name = "StatusPanel";

        //明るくする
        yield return new AlphaManager(blackPanelImage, true).Event();

        //ログ初期化
        logManager.ResetLog(true);
        //ログ表示
        yield return logManager.PrintStr("魔物が現れた！");

        yield return new WaitForSeconds(2f);

        //見えなくする
        logManager.gameObject.SetActive(false);

        //一連のバトルの流れを行う
        yield return ButtleLoop();
    }

    private void Update()
    {
        //ステータスパネルの更新
        if (statusPanel && isButtle)
        {
            var friendCharasNum = friendCharas.Count;
            var commands = statusPanel.GetCommands();
            for(int len = commands.Count, i = friendCharasNum; i < len; i++)
            {
                var rowNo = i / friendCharasNum;
                var colNo = i % friendCharasNum;
                switch (rowNo)
                {
                    //hp
                    case 1:
                        commands[i].text.text = "HP:" + friendCharas[colNo].hp.ToString();
                        break;
                    //mp
                    case 2:
                        commands[i].text.text = "MP:" + friendCharas[colNo].mp.ToString();
                        break;
                    //lv
                    case 3:
                        commands[i].text.text = "Lv:" + ((FriendChara)friendCharas[colNo]).level.ToString();
                        break;
                }
            }
        }
    }

    public IEnumerator ButtleLoop()
    {
        //生きている仲間を取得
        var alliveFriendCharas = GetAlliveChara(friendCharas);
        //味方の技を決める
        for(int i = 0, len = alliveFriendCharas.Count; i < len; i++)
        {
            var alliveFriendChara = alliveFriendCharas[i];
            bool isFirstMake = i == 0;
            //コマンドパネル生成、選択が終わるまで進まない
            yield return new CommandPanelTask(this, (FriendChara)alliveFriendChara, isFirstMake, backGroundRect).Event();
        }
        //敵の技を決める
        DesideEnemyTurn();
        //戦闘計算の並び替え
        SortButtleCulculate();
        //バトル開始
        yield return ButtleCulculate();
    }

    //敵の技を決める
    public void DesideEnemyTurn()
    {
        foreach(var enemyChara in GetAlliveChara(enemyCharas))
        {
            //技リストを取得
            var skillKeys = new List<int>();
            skillKeys.Add(enemyChara.normalSkillKey);
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

    IEnumerator ButtleCulculate()
    {
        isButtle = true;
        foreach (var buttleCulculate in buttleCulculates)
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

            //バトルの計算
            yield return buttleCulculate.Culculate(logManager);
        }

        //バトル計算のリセット
        buttleCulculates.Clear();

        //終了したかをチェック
        isFinished = IsFinished(out bool isWin);

        //戦闘が終了なら
        if (isFinished)
        {
            if (isWin)
            {
                logManager.ResetLog(false);
                //ログの追加表示
                yield return logManager.PrintStr("魔物を全滅させた！");
                yield return logManager.PrintStr("");
                yield return logManager.PrintStr("");
                //スペース押すまで待機
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                //ログの追加表示
                yield return logManager.PrintStr("?の経験値を獲得！");
                yield return logManager.PrintStr("");
                yield return logManager.PrintStr("");
                //スペース押すまで待機
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL);

                var alphaManager = new AlphaManager(blackPanelImage, false);
                //画面を暗くする
                yield return alphaManager.Event();
                //シーン移動
                //ワールドへ移動
                SceneManager.LoadScene("World");
            }
            else
            {
                logManager.ResetLog(false);
                //ログの追加表示
                yield return logManager.PrintStr("全滅してしまった！");
                //スペース押すまで待機
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL);

                var alphaManager = new AlphaManager(blackPanelImage, false);
                //画面を暗くする
                yield return alphaManager.Event();

                //ステータス全快
                foreach (var friendChara in friendCharas)
                {
                    friendChara.CureAll();
                }

                //シーン移動
                //城へ移動
                SceneManager.LoadScene("Castle");
            }
        }
        else
        {
            //ログを見えなくする
            logManager.gameObject.SetActive(false);
            //一連のバトルの流れを行う
            yield return ButtleLoop();
        }
        isButtle = false;
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
