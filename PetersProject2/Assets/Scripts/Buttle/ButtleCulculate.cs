using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtleCulculate
{
    private List<string> strs = new List<string>();

    private ButtleChara offence = null;
    public List<ButtleChara> defences = null;
    private Thing thing = null;

    public int speed { get; private set; }

    private ButtleManager buttleManager = null;

    //替えが必要か
    public bool isNeedAlternate
    {
        get
        {
            if (defences.Count != 1)
                return false;

            return defences[0].isDead;
        }
    }

    public ButtleCulculate(ButtleChara offence, List<ButtleChara> defences, Thing thing, ButtleManager buttleManager)
    {
        this.offence = offence;
        this.defences = defences;
        this.thing = thing;

        this.speed = offence.speed;

        this.buttleManager = buttleManager;
    }

    public void Culculate()
    {
        //攻撃側が死んでいるのなら
        if (offence.isDead)
            //終わり
            return;

        var offencePoint = thing.power;

        //thingが技なら
        if (thing is Skill skill)
        {
            Action useSkillFunc = () => {
                //mpがあるなら
                if (offence.mp >= skill.consumeMP)
                {
                    //mp消費
                    offence.mp -= skill.consumeMP;
                }
            };

            string useSkillName;

            switch (skill.skillType)
            {
                case Skill.SkillType.Normal:
                    useSkillName = offence.name + "の攻撃！";
                    break;
                case Skill.SkillType.Skill:
                    useSkillName = offence.name + "は" + thing.name + "を使った！";
                    break;
                case Skill.SkillType.Magic:
                    useSkillName = offence.name + "は" + thing.name + "を唱えた！";
                    break;
                default:
                    Debug.LogError("知らない技のタイプだよ");
                    return;
            }

            //ログリストに追加
            buttleManager.buttleLogs.Add(new Log(useSkillName, useSkillFunc));

            //mpが足りないなら
            if (offence.mp < skill.consumeMP)
            {
                //ログリストに追加
                buttleManager.buttleLogs.Add(new Log("しかしMPが足りない！"));
            }

            switch (skill.skillType)
            {
                case Skill.SkillType.Magic:
                    offencePoint = (int)(offencePoint + offence.mtp / 1000f);
                    break;
                default:
                    offencePoint = (int)(offencePoint + offence.atp / 1000f);
                    break;
            }
        }
        else
        {

        }

        foreach (ButtleChara defence in defences)
        {
            if (defence.isDead)
                return;

            var defencePoint = thing.isCure ? 0 : (int)(defence.df + defence.df / 1000f);
            var actualPoint = defence.GetChangeHPORMPValue(offencePoint - defencePoint, thing.isCure, thing.isMP);
            var actualPointABS = Mathf.Abs(actualPoint);
            var pointName = thing.isMP ? "MP" : "HP";

            Action changePointFunc = () =>
            {
                if (thing.isMP)
                {
                    defence.mp += actualPoint;
                }
                else
                {
                    defence.hp += actualPoint;
                }
            };

            var changePointName = "";

            if (thing.isCure)
            {
                changePointName = defence.name + "の" + pointName + "が" + actualPointABS + "回復した！";
            }
            else
            {
                if (thing.isMP)
                {
                    changePointName = defence.name + "の" + pointName + "が" + actualPointABS + "減った！";
                }
                else
                {
                    if (defence.isFriend)
                    {
                        changePointName = defence.name + "は" + actualPointABS + "のダメージを受けた！";
                    }
                    else
                    {
                        changePointName = defence.name + "に" + actualPointABS + "のダメージを与えた！";
                    }
                }
            }

            //ログリストに追加
            buttleManager.buttleLogs.Add(new Log(changePointName, changePointFunc));

            string deathName = null;
            Action deathCheckFunc = () =>
            {
                if (defence.isDead)
                {
                    if (defence.isFriend)
                    {
                        deathName = defence.name + "は死んでしまった！";
                    }
                    else
                    {
                        deathName = defence.name + "を倒した！";
                    }
                }
            };

            //ログリストに追加(死んでいないとき(deathNameがnullのとき)はログ表示されないようになってる)
            buttleManager.buttleLogs.Add(new Log(deathName, deathCheckFunc));
        }
    }
}
