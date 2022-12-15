using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommandPanelTask : EventTask
{
    private Vector2 commandPanelfirstPos = new Vector2(100, 500);
    private Vector2 enemySelectPanelPos = new Vector2(1000, 500);
    private ButtleManager buttleManager = null;

    private FriendChara friendChara = null;
    private bool isFirstMake = false;

    private bool isFinished = false;

    private RectTransform parentRect = null;

    public CommandPanelTask(ButtleManager buttleManager, FriendChara friendChara, bool isFirstMake, RectTransform parentRect)
    {
        this.buttleManager = buttleManager;
        this.friendChara = friendChara;
        this.isFirstMake = isFirstMake;
        this.parentRect = parentRect;
    }

    public override IEnumerator Event()
    {
        SelectFriendAction();

        while (!isFinished)
        {
            yield return null;
        }
    }

    public void SelectFriendAction()
    {
        Command buttleCommand = null;

        CommandPanel buttleCommandPanel = null;

        //最初の生成なら
        if (isFirstMake)
        {
            //バトルパネル
            buttleCommandPanel = CommandManager.Instance.MakeCommandPanel(new List<string> { "たたかう", "にげる", "さくせん" }, 3, 1, commandPanelfirstPos, null, false, true, parentRect);
            var buttleCommands = buttleCommandPanel.GetCommands();
            buttleCommand = buttleCommands[0];

            //逃げるを選択したら
            buttleCommands[1].SetAction(() =>
            {
                //逃げようとするboolをtrueに
                buttleManager.isEscape = true;
                //終わりの合図
                isFinished = true;
                //コマンドパネルを全削除
                CommandManager.Instance.RemoveAllButtleCommandPanel();
            });
        }

        var commandPanel1 = CommandManager.Instance.MakeCommandPanel(new List<string> { "こうげき", "じゅもん", "とくぎ", "どうぐ" }, 4, 1, commandPanelfirstPos, buttleCommand, false, true, parentRect);
        var commands1 = commandPanel1.GetCommands();

        //こうげきを選択したら
        MakeThingPanel(ThingEngine.Instance.Gets(new List<int>() { friendChara.normalSkillKey }), commands1[0]);

        //じゅもんを選択したら
        MakeThingPanel(ThingEngine.Instance.Gets(friendChara.magicKeys), commands1[1]);

        //とくぎを選択したら
        MakeThingPanel(ThingEngine.Instance.Gets(friendChara.skillKeys), commands1[2]);

        //道具を選択したら
        MakeThingPanel(friendChara.itemBag.items, commands1[3]);
    }

    public void MakeThingPanel(List<Thing> things, Command parentCommand)
    {
        //生きている味方
        var friendCharas = ButtleManager.GetAlliveChara(ButtleManager.friendCharas);
        //生きている敵
        var enemyCharas = ButtleManager.GetAlliveChara(ButtleManager.enemyCharas);


        if (things != null)
        {
            //thingの名前たち
            var thingNames = new List<string>();

            foreach (var thing in things)
            {
                thingNames.Add(thing.name);
            }

            //技パネル表示
            var thingPanel = CommandManager.Instance.MakeCommandPanel(thingNames, 3, 2, Vector2.zero, parentCommand, false, true, parentRect, "使えるものがない！", 7);
            //技コマンド取得
            var thingCommands = thingPanel.GetCommands();
            for (int i = 0, len = thingCommands.Count; i < len; i++)
            {
                var thingCommand = thingCommands[i];
                var thing = things[i];

                List<ButtleChara> targets = null;
                List<ButtleChara> defences = null;

                Skill skill = null;
                //技なら
                if (thing is Skill)
                {
                    skill = (Skill)thing;
                }
                //装備なら
                else
                {
                    skill = (Skill)ThingEngine.Instance.Get(((Equipment)thing).useSkillKey);
                }

                if(skill != null)
                {
                    //回復なら
                    if (skill.isCure)
                    {
                        //ターゲットは味方にする
                        targets = friendCharas;
                    }
                    else
                    {
                        //ターゲットは敵にする
                        targets = enemyCharas;
                    }

                    //全体こうげきなら
                    if (skill.isAll)
                    {
                        defences = targets;

                        //技を選択したら
                        thingCommand.SetAction(() =>
                        {
                            //計算リストに追加
                            buttleManager.buttleCulculates.Add(new ButtleCulculate(friendChara, defences, thing));
                            //終わりの合図
                            isFinished = true;
                            //コマンドパネルを全削除
                            CommandManager.Instance.RemoveAllButtleCommandPanel();
                        });
                    }
                    //単体こうげきなら
                    else
                    {
                        //守備の名前取得
                        var targetNames = ButtleManager.GetCharaName(targets);
                        //守備パネル表示
                        var targetPanel = CommandManager.Instance.MakeCommandPanel(targetNames, targetNames.Count, 1, enemySelectPanelPos, thingCommand, false, true, parentRect);
                        //守備コマンド取得
                        var targetCommands = targetPanel.GetCommands();
                        for (int j = 0, len2 = targetNames.Count; j < len2; j++)
                        {
                            var targetCommand = targetCommands[j];
                            var target = targets[j];

                            //敵を選択したら
                            targetCommand.SetAction(() =>
                            {
                                //計算リストに追加
                                buttleManager.buttleCulculates.Add(new ButtleCulculate(friendChara, new List<ButtleChara>() { target }, thing));
                                //終わりの合図
                                isFinished = true;
                                //コマンドパネルを全削除
                                CommandManager.Instance.RemoveAllButtleCommandPanel();
                            });
                        }
                    }
                }
                //技がない(装備が使えないもの)なら
                else
                {
                    //技を選択したら
                    thingCommand.SetAction(() =>
                    {
                        //計算リストに追加
                        buttleManager.buttleCulculates.Add(new ButtleCulculate(friendChara, null, thing));
                        //終わりの合図
                        isFinished = true;
                        //コマンドパネルを全削除
                        CommandManager.Instance.RemoveAllButtleCommandPanel();
                    });
                }
            }
        }
    }
}
