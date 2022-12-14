using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCommandController : MonoBehaviour
{
    private CommandPanel commandPanelRoot = null;
    [SerializeField] private RectTransform parentRect = null;
    [SerializeField] private RectTransform commandPanelfirstRect = null;
    [SerializeField] private RectTransform commandPanelSelectRect = null;
    [SerializeField] private RectTransform statusPanelRect = null;
    [SerializeField] private LogManager logManager = null;
    //ステータスのパネル
    private CommandPanel statusPanel = null;

    [SerializeField] private ControllManager controllManager = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    //最初のコマンドパネルとステータスパネルを作る関数
    private void MakeCommandPanels()
    {
        var allStrs = new List<List<string>>();
        //ステータスパネル作成
        for (int i = 0; i < ButtleManager.friendCharas.Count; i++)
        {
            var friendChara = ButtleManager.friendCharas[i];
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
        statusPanel = CommandManager.Instance.MakeCommandPanel(strs_t, 4, strs_t.Count / 4, statusPanelRect.position, null, true, true, parentRect, "コマンドがないよ。", 4);
        //ステータスパネルの名前変更
        statusPanel.gameObject.name = "StatusPanel";
        //ステータスパネルのフレームの位置Xを変更
        var frameRect = statusPanel.frameRect;
        frameRect.position = new Vector3(Screen.width / 2 - frameRect.sizeDelta.x / 2, frameRect.position.y);

        //コマンドパネル
        commandPanelRoot = CommandManager.Instance.MakeCommandPanel(new List<string> { "はなす", "じゅもん", "どうぐ", "しらべる", "つよさ", "さくせん" }, 3, 2, commandPanelfirstRect.position, null, false, true, parentRect);
        var commandsRoot = commandPanelRoot.GetCommands();

        var leader = controllManager.leader;
        //はなすを押したら
        commandsRoot[0].SetAction(() =>
        {
            if(leader.key != CharaController.Key.NONE)
            {
                //CellEventを取得(Checkタイプ)
                var cellEvent = leader.GetCellEvent(leader.GetNextTargetPos(leader.direction), CellEvent.CellType.Check);
                //CellEventがあるなら
                if (cellEvent)
                {
                    //はなすイベントなら
                    if (cellEvent is TalkEvent)
                    {
                        //パネルを消す
                        CommandManager.Instance.RemoveAllButtleCommandPanel();
                        //参照をnullに
                        commandPanelRoot = null;
                        //ステータスパネルを消す
                        Destroy(statusPanel.gameObject);

                        //イベントを呼ぶ
                        StartCoroutine(cellEvent.CallEvent());
                    }
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
            if (leader.key != CharaController.Key.NONE)
            {
                //CellEventを取得(Checkタイプ)
                var cellEvent = leader.GetCellEvent(leader.GetNextTargetPos(leader.direction), CellEvent.CellType.Check);
                //CellEventがあるなら
                if (cellEvent)
                {
                    //はなすイベントでないなら
                    if (!(cellEvent is TalkEvent))
                    {
                        //パネルを消す
                        CommandManager.Instance.RemoveAllButtleCommandPanel();
                        //参照をnullに
                        commandPanelRoot = null;
                        //ステータスパネルを消す
                        Destroy(statusPanel.gameObject);

                        //イベントを呼ぶ
                        StartCoroutine(cellEvent.CallEvent());
                    }
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
        //ステータスパネルがあるなら
        if (statusPanel)
        {
            //ステータスの更新
            var friendCharas = ButtleManager.friendCharas;
            var friendCharasNum = friendCharas.Count;
            var commands = statusPanel.GetCommands();
            for (int len = commands.Count, i = friendCharasNum; i < len; i++)
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            //プレイヤーが動いていないときでコマンドパネルが生成されていないなら
            if (!controllManager.leader.isMoving && !CommandManager.Instance.nowCommandPanel && !FragEvent.isEvent)
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
                    //ステータスパネルを消す
                    Destroy(statusPanel.gameObject);
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
        var friendCharaPanel = CommandManager.Instance.MakeCommandPanel(friendCharaNames, friendCharaNames.Count, 1, Vector2.zero, parentCommand, false, true, parentRect);
        var friendCharaCommands = friendCharaPanel.GetCommands();

        for(int i = 0, len = friendCharaCommands.Count; i < len; i++)
        {
            var friendChara = friendCharas[i];
            List<Thing> things = null;
            if (isMagic)
            {
                //使える魔法を取得
                things = GetMagicInField(ThingEngine.Instance.Gets(friendChara.magicKeys));
            }
            else
            {
                things = friendChara.itemBag.items;
            }

            //thingの名前たち
            var thingNames = new List<string>(ButtleManager.GetThingNames(things));

            //技パネル表示
            var thingPanel = CommandManager.Instance.MakeCommandPanel(thingNames, 8, 1, Vector2.zero, friendCharaCommands[i], false, true, parentRect, friendChara.name + (isMagic ? "はつかえるまほうがない。" : "はもちものをもっていない。"), 7);
            //技コマンド取得
            var thingCommands = thingPanel.GetCommands();

            for (int j = 0, len2 = thingCommands.Count; j < len2; j++)
            {
                var thingCommand = thingCommands[j];
                var thing = things[j];

                List<ButtleChara> targets = ButtleManager.GetAlliveChara(friendCharas);

                var useCommandNames = new List<string> { "つかう", "わたす", "すてる" };
                //装備なら
                if (thing is Equipment)
                {
                    //そうびコマンド追加
                    useCommandNames.Add("そうび");
                }

                List<Command> useCommands = null;
                //道具なら
                if (!isMagic)
                {
                    var usePanel = CommandManager.Instance.MakeCommandPanel(useCommandNames, useCommandNames.Count, 1, Vector2.zero, thingCommand, false, true, parentRect);
                    useCommands = usePanel.GetCommands();
                }
                //守備の名前取得
                var targetNames = ButtleManager.GetCharaName(targets);
                //装備なら
                if(useCommands != null && thing is Equipment)
                {
                    //使うを押したら
                    useCommands[0].SetAction(() =>
                    {
                        //使用のログ表示
                        StartCoroutine(UseThingCulculate(new ButtleCulculate(friendChara, null, thing)));
                        //一個前に戻る
                        CommandManager.Instance.CommandBack();
                    });
                }
                else
                {
                    //つかうを押したら守備パネル表示
                    var targetPanel = CommandManager.Instance.MakeCommandPanel(targetNames, targetNames.Count, 1, commandPanelSelectRect.position, useCommands != null ? useCommands[0] : thingCommand, false, true, parentRect);
                    //守備コマンド取得
                    var targetCommands = targetPanel.GetCommands();
                    for (int k = 0, len3 = targetNames.Count; k < len3; k++)
                    {
                        var targetCommand = targetCommands[k];
                        var target = targets[k];

                        //ターゲットを選択したら
                        targetCommand.SetAction(() =>
                        {
                            //魔法を使用するキャラが死んでいるなら
                            if (friendChara.isDead && isMagic)
                            {
                                //ログ表示
                                StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "はしんでいる！"}));
                            }
                            //生きているなら
                            else
                            {
                                //使用のログ表示
                                StartCoroutine(UseThingCulculate(new ButtleCulculate(friendChara, new List<ButtleChara>() { target }, thing)));
                                for (int l = 0, len4 = isMagic ? 1 : 2; l < len4; l++)
                                {
                                    //一個前に戻る
                                    CommandManager.Instance.CommandBack();
                                }
                                //アイテムなら消費
                                if (!isMagic && thing is Skill)
                                {
                                    //アイテムをすてる
                                    friendChara.itemBag.RemoveItem(thingPanel.nowNo);
                                    //コマンド削除
                                    thingPanel.RemoveCommand(thingPanel.nowNo);
                                }
                            }
                        });
                    }
                }

                if (useCommands != null)
                {
                    //わたすを押したら守備パネル表示
                    var targetPanel2 = CommandManager.Instance.MakeCommandPanel(targetNames, targetNames.Count, 1, commandPanelSelectRect.position, useCommands[1], false, true, parentRect);
                    var action = useCommands[1].GetAction();
                    //渡すを押したとき
                    useCommands[1].SetAction(() =>
                    {
                        //装備で
                        if (thing is Equipment equipment)
                        {
                            //装備中なら
                            if (equipment.isEquiped)
                            {
                                //わたせない
                                StartCoroutine(logManager.PrintLog(new List<string>() { "装備中のはわたせない！" }));
                                return;
                            }
                        }
                        //それ以外は渡せる
                        action();
                    });
                    //守備コマンド取得
                    var targetCommands2 = targetPanel2.GetCommands();
                    for (int k = 0, len3 = targetNames.Count; k < len3; k++)
                    {
                        var targetCommand2 = targetCommands2[k];
                        var target = targets[k];
                        var no = k;

                        //ターゲットを選択したら
                        targetCommand2.SetAction(() =>
                        {
                            //渡す候補のアイテム取得
                            var item = friendChara.itemBag.items[thingPanel.nowNo];
                            //アイテムを渡せるなら
                            if (target.itemBag.AddItem(item))
                            {
                                //渡したときのログ表示
                                StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "は" + thing.name + "を" + target.name + "にわたした。" }));
                                for (int l = 0, len4 = 2; l < len4; l++)
                                {
                                    //一個前に戻る
                                    CommandManager.Instance.CommandBack();
                                }

                                //アイテムを追加
                                target.itemBag.AddItem(item);
                                //コマンド追加
                                MakeThingCommand(friendCharas[no], item, target.itemBag.items, friendCharaCommands[no].childPanel, friendCharaCommands);

                                //アイテムを元のから消す
                                friendChara.itemBag.RemoveItem(thingPanel.nowNo);
                                //コマンド削除
                                thingPanel.RemoveCommand(thingPanel.nowNo);
                            }
                            //渡せないなら
                            else
                            {
                                StartCoroutine(logManager.PrintLog(new List<string>() { friendCharas[no].name + "のもちものはいっぱいだ！" }));
                            }

                        });
                    }

                    //すてるを押したら
                    useCommands[2].SetAction(() =>
                    {
                        //装備で
                        if (thing is Equipment equipment)
                        {
                            //装備中なら
                            if (equipment.isEquiped)
                            {
                                //わたせない
                                StartCoroutine(logManager.PrintLog(new List<string>() { "装備中のはすてられない！" }));
                                return;
                            }
                        }

                        //一個前に戻る
                        CommandManager.Instance.CommandBack();
                        //アイテムをすてる
                        friendChara.itemBag.RemoveItem(thingPanel.nowNo);
                        //コマンド削除
                        thingPanel.RemoveCommand(thingPanel.nowNo);
                        //使用のログ表示
                        StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "は" + thing.name + "をすてた。" }));
                    });

                    //装備なら
                    if (thing is Equipment equipment)
                    {
                        //そうびを押したら
                        useCommands[3].SetAction(() =>
                        {
                            //一個前に戻る
                            CommandManager.Instance.CommandBack();
                            var log = "";
                            //装備しているなら
                            if (equipment.isEquiped)
                            {
                                //外す
                                ((FriendChara)friendChara).RemoveEquipment(equipment.equipmentType);
                                //名前を変える(Eを外す)
                                thingCommand.Name = thing.name;
                                //ログを入れる
                                log = "はずした。";
                            }
                            else
                            {
                                //外す
                                var beforeEquipment = ((FriendChara)friendChara).RemoveEquipment(equipment.equipmentType);
                                //コマンドのインデックスを取得
                                int no = things.IndexOf(beforeEquipment);
                                //コマンドがあるなら
                                if (no != -1)
                                {
                                    //名前を変える(Eを外す)
                                    thingCommands[no].Name = beforeEquipment.name;
                                }

                                //装備する
                                ((FriendChara)friendChara).Equip(equipment);
                                //名前を変える(Eをつける)
                                thingCommand.Name = "E" + thing.name;
                                //ログを入れる
                                log = "そうびした。";
                            }
                            //ログ表示
                            StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "は" + thing.name + "を" + log }));
                        });
                    }
                }
            }
        }
    }

    private void MakeThingCommand(ButtleChara friendChara, Thing thing, List<Thing> things, CommandPanel thingPanel, List<Command> friendCharaCommands)
    {
        List<ButtleChara> targets = ButtleManager.GetAlliveChara(ButtleManager.friendCharas);
        var thingCommands = thingPanel.GetCommands();
        var thingCommand = thingPanel.AddCommand(thing.name);
        var useCommandNames = new List<string> { "つかう", "わたす", "すてる" };
        //装備なら
        if(thing is Equipment)
        {
            //そうびコマンド追加
            useCommandNames.Add("そうび");
        }
        //道具なら
        var usePanel = CommandManager.Instance.MakeCommandPanel(useCommandNames, useCommandNames.Count, 1, Vector2.zero, thingCommand, false, true, parentRect);
        var useCommands = usePanel.GetCommands();
        //守備の名前取得
        var targetNames = ButtleManager.GetCharaName(targets);
        //つかうを押したら守備パネル表示
        var targetPanel = CommandManager.Instance.MakeCommandPanel(targetNames, targetNames.Count, 1, commandPanelSelectRect.position, useCommands != null ? useCommands[0] : thingCommand, false, true, parentRect);
        //守備コマンド取得
        var targetCommands = targetPanel.GetCommands();
        for (int k = 0, len3 = targetNames.Count; k < len3; k++)
        {
            var targetCommand = targetCommands[k];
            var target = targets[k];

            //ターゲットを選択したら
            targetCommand.SetAction(() =>
            {
                //使用のログ表示
                StartCoroutine(UseThingCulculate(new ButtleCulculate(friendChara, new List<ButtleChara>() { target }, thing)));
                for (int l = 0, len4 = 2; l < len4; l++)
                {
                    //一個前に戻る
                    CommandManager.Instance.CommandBack();
                }
                //アイテムなら消費
                //アイテムをすてる
                friendChara.itemBag.RemoveItem(thingPanel.nowNo);
                //コマンド削除
                thingPanel.RemoveCommand(thingPanel.nowNo);
            });
        }

        if (useCommands != null)
        {
            //わたすを押したら守備パネル表示
            var targetPanel2 = CommandManager.Instance.MakeCommandPanel(targetNames, targetNames.Count, 1, commandPanelSelectRect.position, useCommands[1], false, true, parentRect);
            var action = useCommands[1].GetAction();
            //渡すを押したとき
            useCommands[1].SetAction(() =>
            {
                //装備で
                if (thing is Equipment equipment)
                {
                    //装備中なら
                    if (equipment.isEquiped)
                    {
                        //わたせない
                        StartCoroutine(logManager.PrintLog(new List<string>() { "装備中のはわたせない！" }));
                        return;
                    }
                }
                //それ以外は渡せる
                action();
            });
            //守備コマンド取得
            var targetCommands2 = targetPanel2.GetCommands();
            for (int k = 0, len3 = targetNames.Count; k < len3; k++)
            {
                var targetCommand2 = targetCommands2[k];
                var target = targets[k];
                var no = k;

                //ターゲットを選択したら
                targetCommand2.SetAction(() =>
                {
                    //渡す候補のアイテム取得
                    var item = friendChara.itemBag.items[thingPanel.nowNo];
                    //アイテムを渡せるなら
                    if (target.itemBag.AddItem(item))
                    {
                        //渡したときのログ表示
                        StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "は" + thing.name + "を" + target.name + "にわたした。" }));
                        for (int l = 0, len4 = 2; l < len4; l++)
                        {
                            //一個前に戻る
                            CommandManager.Instance.CommandBack();
                        }

                        //アイテムを追加
                        target.itemBag.AddItem(item);
                        //コマンド追加
                        MakeThingCommand(ButtleManager.friendCharas[no], item, target.itemBag.items, friendCharaCommands[no].childPanel, friendCharaCommands);

                        //アイテムを元のから消す
                        friendChara.itemBag.RemoveItem(thingPanel.nowNo);
                        //コマンド削除
                        thingPanel.RemoveCommand(thingPanel.nowNo);
                    }
                    //渡せないなら
                    else
                    {
                        StartCoroutine(logManager.PrintLog(new List<string>() { ButtleManager.friendCharas[no].name + "のどうぐはいっぱいだ！" }));
                    }

                });
            }

            //すてるを押したら
            useCommands[2].SetAction(() =>
            {
                //装備で
                if (thing is Equipment equipment)
                {
                    //装備中なら
                    if (equipment.isEquiped)
                    {
                        //わたせない
                        StartCoroutine(logManager.PrintLog(new List<string>() { "装備中のはすてられない！" }));
                        return;
                    }
                }

                //一個前に戻る
                CommandManager.Instance.CommandBack();
                //アイテムをすてる
                friendChara.itemBag.RemoveItem(thingPanel.nowNo);
                //コマンド削除
                thingPanel.RemoveCommand(thingPanel.nowNo);
                //使用のログ表示
                StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "は" + thing.name + "をすてた。" }));
            });

            //装備なら
            if (thing is Equipment equipment)
            {
                //そうびを押したら
                useCommands[3].SetAction(() =>
                {
                    //一個前に戻る
                    CommandManager.Instance.CommandBack();
                    var log = "";
                    //装備しているなら
                    if (equipment.isEquiped)
                    {
                        //外す
                        ((FriendChara)friendChara).RemoveEquipment(equipment.equipmentType);
                        //名前を変える(Eを外す)
                        thingCommand.Name = thing.name;
                        //ログを入れる
                        log = "はずした。";
                    }
                    else
                    {
                        //外す
                        var beforeEquipment = ((FriendChara)friendChara).RemoveEquipment(equipment.equipmentType);
                        //コマンドのインデックスを取得
                        int no = things.IndexOf(beforeEquipment);
                        //コマンドがあるなら
                        if (no != -1)
                        {
                            //名前を変える(Eを外す)
                            thingCommands[no].Name = beforeEquipment.name;
                        }

                        //装備する
                        ((FriendChara)friendChara).Equip(equipment);
                        //名前を変える(Eをつける)
                        thingCommand.Name = "E" + thing.name;
                        //ログを入れる
                        log = "そうびした。";
                    }
                    //ログ表示
                    StartCoroutine(logManager.PrintLog(new List<string>() { friendChara.name + "は" + thing.name + "を" + log }));
                });
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
