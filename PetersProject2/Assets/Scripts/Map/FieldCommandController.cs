using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCommandController : MonoBehaviour
{
    [SerializeField] private CommandPanel commandPanelRoot = null;
    [SerializeField] private RectTransform parentRect = null;
    [SerializeField] private RectTransform commandPanelfirstRect = null;
    [SerializeField] private RectTransform commandPanelSelectRect = null;
    private YushaController yushaController = null;
    private LogManager logManager = null;

    // Start is called before the first frame update
    void Start()
    {
        yushaController = FindObjectOfType<YushaController>();
        logManager = FindObjectOfType<LogManager>();
    }

    //最初のコマンドパネルとステータスパネルを作る関数
    private void MakeCommandPanels()
    {
        //コマンドパネル
        commandPanelRoot = CommandManager.Instance.MakeCommandPanel(new List<string> { "はなす", "じゅもん", "どうぐ", "しらべる", "つよさ", "さくせん" }, 3, 2, commandPanelfirstRect.position, null, false, true, parentRect);
        var commandsRoot = commandPanelRoot.GetCommands();

        //はなすを押したら
        commandsRoot[0].SetAction(() =>
        {
            //CellEventを取得(Checkタイプ)
            var cellEvent = yushaController.GetCellEvent(yushaController.GetNextTargetPos(yushaController.direction), CellEvent.CellType.Check);
            //CellEventがあるなら
            if (cellEvent)
            {
                //はなすイベントなら
                if (cellEvent is TalkEvent)
                {
                    //イベントを呼ぶ
                    StartCoroutine(cellEvent.CallEvent());
                }
            }
        });

        //じゅもんなら
        MakeThingPanel(true, commandsRoot[1]);

        //道具なら
        MakeThingPanel(false, commandsRoot[2]);

        //しらべるを押したら
        commandsRoot[3].SetAction(() =>
        {
            //CellEventを取得(Checkタイプ)
            var cellEvent = yushaController.GetCellEvent(yushaController.GetNextTargetPos(yushaController.direction), CellEvent.CellType.Check);
            //CellEventがあるなら
            if (cellEvent)
            {
                //はなすイベントでないなら
                if (!(cellEvent is TalkEvent))
                {
                    //イベントを呼ぶ
                    StartCoroutine(cellEvent.CallEvent());
                }
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (logManager)
        {
            //ログが見えている間はreturn
            if (logManager.gameObject.activeInHierarchy)
                return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //プレイヤーが動いていないときでコマンドパネルが生成されていないなら
            if (!yushaController.isMoving && !CommandManager.Instance.nowCommandPanel)
            {
                MakeCommandPanels();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (commandPanelRoot && !logManager.gameObject.activeInHierarchy)
            {
                //今見ているのが一番最初のパネルなら
                if (CommandManager.Instance.nowCommandPanel == commandPanelRoot)
                {
                    //パネルを消す
                    CommandManager.Instance.RemoveAllButtleCommandPanel();
                    //参照をnullに
                    commandPanelRoot = null;
                    //ステータスパネルを非表示
                }
            }
        }
    }

    private void MakeThingPanel(bool isMagic, Command parentCommand)
    {
        //味方取得
        var friendCharas = ButtleManager.friendCharas;
        //味方の名前取得
        var friendCharaNames = ButtleManager.GetCharaName(friendCharas);

        //味方パネル表示
        var friendCharaPanel = CommandManager.Instance.MakeCommandPanel(friendCharaNames, friendCharaNames.Count, 1, commandPanelfirstRect.position, parentCommand, false, true, parentRect);
        var friendCharaCommands = friendCharaPanel.GetCommands();

        for(int i = 0, len = friendCharaCommands.Count; i < len; i++)
        {
            var friendChara = friendCharas[i];
            //死んでいる味方を選択したら
            if (friendChara.isDead)
            {
                friendCharaCommands[i].SetAction(() =>
                {
                    logManager.PrintLog(new List<string>() { friendChara.name + "はしんでいる！" });
                });
            }
            else
            {
                List<Thing> things = null;
                if (isMagic)
                {
                    //使える魔法を取得
                    things = GetMagicInField(ThingEngine.Instance.Gets(friendChara.magicKeys));
                }
                else
                {
                    things = friendChara.items;
                }

                if (things.Count == 0)
                    return;

                //thingの名前たち
                var thingNames = new List<string>();

                foreach (var thing in things)
                {
                    thingNames.Add(thing.name);
                }

                //技パネル表示
                var thingPanel = CommandManager.Instance.MakeCommandPanel(thingNames, 3, 2, commandPanelfirstRect.position, friendCharaCommands[i], false, true, parentRect);
                //技コマンド取得
                var thingCommands = thingPanel.GetCommands();

                for (int j = 0, len2 = thingCommands.Count; j < len2; j++)
                {
                    var thingCommand = thingCommands[j];
                    var thing = things[j];

                    List<ButtleChara> targets = ButtleManager.GetAlliveChara(friendCharas);

                    if (thing is Skill)
                    {
                        List<Command> useCommands = null;
                        //道具なら
                        if (!isMagic)
                        {
                            var usePanel = CommandManager.Instance.MakeCommandPanel(new List<string> { "つかう", "わたす", "すてる" }, 3, 1, commandPanelfirstRect.position, thingCommand, false, true, parentRect);
                            useCommands = usePanel.GetCommands();
                        }
                        //守備の名前取得
                        var targetNames = ButtleManager.GetCharaName(targets);
                        //守備パネル表示
                        var targetPanel = CommandManager.Instance.MakeCommandPanel(targetNames, targetNames.Count, 1, commandPanelSelectRect.position, useCommands != null ? useCommands[0] : thingCommand, false, true, parentRect);
                        //守備コマンド取得
                        var targetCommands = targetPanel.GetCommands();
                        for (int k = 0, len3 = targetNames.Count; k < len3; k++)
                        {
                            var targetCommand = targetCommands[k];
                            var target = targets[k];

                            //敵を選択したら
                            targetCommand.SetAction(() =>
                            {
                                //使用のログ表示
                                StartCoroutine(UseThingCulculate(new ButtleCulculate(friendChara, new List<ButtleChara>() { target }, thing)));
                            });
                        }
                    }
                    //装備なら
                    else
                    {
                        //技を選択したら
                        thingCommand.SetAction(() =>
                        {
                            //使用のログ表示
                            StartCoroutine(UseThingCulculate(new ButtleCulculate(friendChara, null, thing)));
                        });
                    }
                }
            }
        }
    }

    //フィールド上で使える魔法を取得
    private List<Thing> GetMagicInField(List<Thing> magics)
    {
        var useMagics = new List<Thing>();

        foreach(var magic in magics)
        {
            if(magic is Skill)
            {
                var skill = (Skill)magic;
                if(skill.skillType == Skill.SkillType.Magic)
                {
                    //魔法が単体で、回復なら
                    if(!skill.isAll && skill.isCure)
                    {
                        useMagics.Add(magic);
                    }
                }
            }
        }

        return useMagics;
    }

    private IEnumerator UseThingCulculate(ButtleCulculate buttleCulculate)
    {
        //計算とログ表示
        yield return buttleCulculate.Culculate(logManager, false);

        //スペース押すまで待機
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        //押したらログを見えなくする
        logManager.gameObject.SetActive(false);
    }
}
