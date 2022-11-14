using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ButtleManager : MonoBehaviour
{
    [SerializeField] private CommandManager commandManager = null;
    [SerializeField] private EventTaskManager eventTaskManager = null;
    [SerializeField] private Image blackPanelImage = null;

    private CommandPanel statusPanel = null;

    private static List<FriendChara> friendCharas = null;
    private static List<EnemyChara> enemyCharas = null;

    private List<string> buttleStrs = new List<string>();

    private Vector2 commandPanelfirstPos = new Vector2(100, 500);
    private Vector2 enemySelectPanelPos = new Vector2(1000, 500);

    private List<ButtleCulculate> buttleCulculates = new List<ButtleCulculate>();

    private SkillEngine skillEngine = null;

    // Start is called before the first frame update
    void Start()
    {
        skillEngine = FindObjectOfType<SkillEngine>();

        if (eventTaskManager)
        {
            //明るくする
            eventTaskManager.PushTask(new AlphaManager(blackPanelImage, true));
            //コマンドパネル生成
            eventTaskManager.PushTask(new DoNowTask(() =>
            {
                if (commandManager)
                {
                    //バトルパネル
                    var commandPanelRoot = commandManager.MakeCommandPanel(new List<string> { "たたかう", "にげる", "さくせん" }, 3, 1, commandPanelfirstPos, null, false, true);
                    var commandsRoot = commandPanelRoot.GetCommands();

                    var commandLast = commandsRoot[0];

                    foreach (var friendChara in friendCharas)
                    {
                        var commandPanel1 = commandManager.MakeCommandPanel(new List<string> { "こうげき", "じゅもん", "とくぎ", "どうぐ" }, 4, 1, commandPanelfirstPos, commandLast, false, true);
                        var commands1 = commandPanel1.GetCommands();
                        //こうげきを選択したら
                        var skill = skillEngine.Get(friendChara.normalSkillKey);
                        //単体攻撃なら
                        if (!skill.isAll)
                        {
                            //敵表示
                            var enemyNames = GetCharaNames(enemyCharas.Cast<ButtleChara>().ToList());
                            var enemySelectPanel = commandManager.MakeCommandPanel(enemyNames, enemyNames.Count, 1, enemySelectPanelPos, commands1[0], false, true);
                        }
                        //全体攻撃なら
                        else
                        {
                            //計算リストに追加
                            commands1[0].SetAction(() =>
                            {
                                buttleCulculates.Add(new ButtleCulculate(friendChara, enemyCharas.Cast<ButtleChara>().ToList(), skill));
                            });
                        }

                        //じゅもんを選択したら
                        //呪文表示
                        var magicNames = GetThingNames(friendChara.magics.Cast<Thing>().ToList());
                        var magicPanel = commandManager.MakeCommandPanel(magicNames, 3, 2, enemySelectPanelPos, commands1[1], false, true);
                        var magicCommands = magicPanel.GetCommands();

                    }


                    //ステータスパネル
                    statusPanel = commandManager.MakeCommandPanel(new List<string> { "ピーター", "HP:?", "MP:?", "Lv:?" }, 4, 1, new Vector2(100, 1000), null, true, true);
                }
            }));
        }
    }

    private void Update()
    {
        //ステータスパネルの更新
        if (statusPanel)
        {

        }
    }

    public static void SetButtleCharas(List<FriendChara> friendCharas, List<EnemyChara> enemyCharas)
    {
        ButtleManager.friendCharas = friendCharas;
        ButtleManager.enemyCharas = enemyCharas;
    }

    public List<string> GetCharaNames(List<ButtleChara> buttleCharas)
    {
        var names = new List<string>();

        foreach (ButtleChara buttleChara in buttleCharas)
        {
            names.Add(buttleChara.name);
        }

        return names;
    }

    public List<string> GetThingNames(List<Thing> things)
    {
        var names = new List<string>();

        foreach (Thing thing in things)
        {
            names.Add(thing.name);
        }

        return names;
    }
}
