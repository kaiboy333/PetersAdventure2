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

    public ButtleCulculate(ButtleChara offence, List<ButtleChara> defences, Thing thing)
    {
        this.offence = offence;
        this.defences = defences;
        this.thing = thing;

        this.speed = offence.speed;
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
            switch (skill.skillType)
            {
                case Skill.SkillType.Normal:
                    strs.Add(offence.name + "の攻撃！");
                    break;
                case Skill.SkillType.Skill:
                    strs.Add(offence.name + "は" + thing.name + "を使った！");
                    break;
                case Skill.SkillType.Magic:
                    strs.Add(offence.name + "は" + thing.name + "を唱えた！");
                    break;
            }
            //mp消費
            //mpがあるなら
            if (offence.mp >= skill.consumeMP)
            {
                offence.mp -= skill.consumeMP;
            }
            //mpが足りないなら
            else
            {
                strs.Add("しかしMPが足りない！");
                return;
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
            var actualPoint = defence.ChangeHPORMP(offencePoint - defencePoint, thing.isCure, thing.isMP);
            var pointName = thing.isMP ? "MP" : "HP";

            if (thing.isCure)
            {
                strs.Add(defence.name + "の" + pointName + "が" + actualPoint + "回復した！");
            }
            else
            {
                if (thing.isMP)
                {
                    strs.Add(defence.name + "の" + pointName + "が" + actualPoint + "減った！");
                }
                else
                {
                    if (defence.isFriend)
                    {
                        strs.Add(defence.name + "は" + actualPoint + "のダメージを受けた！");
                        if (defence.isDead)
                        {
                            strs.Add(defence.name + "は死んでしまった！");
                        }
                    }
                    else
                    {
                        strs.Add(defence.name + "に" + actualPoint + "のダメージを与えた！");
                        if (defence.isDead)
                        {
                            strs.Add(defence.name + "を倒した！");
                        }
                    }
                }
            }
        }
    }
}
