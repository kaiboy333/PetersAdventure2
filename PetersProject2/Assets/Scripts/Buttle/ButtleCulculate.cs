using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtleCulculate
{
    private List<string> strs = new List<string>();

    ButtleChara offence = null;
    List<ButtleChara> defences = null;
    Thing thing = null;

    private int speed = 0;

    public ButtleCulculate(ButtleChara offence, List<ButtleChara> defences, Thing thing)
    {
        this.offence = offence;
        this.defences = defences;
        this.thing = thing;

        this.speed = offence.speed;
    }

    public void Culculate()
    {
        //計算できないなら終わり
        if (!CanCulculate())
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
                    }
                    else
                    {
                        strs.Add(defence.name + "に" + actualPoint + "のダメージを与えた！");
                    }
                }
            }
        }
    }

    //計算しても良いか
    public bool CanCulculate()
    {
        if (offence.isDead)
            return false;

        foreach (ButtleChara defence in defences)
        {
            if (defence.isDead)
                return false;
        }

        return true;
    }
}
