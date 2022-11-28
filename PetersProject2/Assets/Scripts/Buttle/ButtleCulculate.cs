using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtleCulculate
{
    private ButtleChara offence = null;
    public List<ButtleChara> defences = null;
    private Thing thing = null;

    public int speed { get; private set; }

    //敵が点滅している時間
    private float blinkTime = 0.8f;

    //替えが必要か
    public bool isNeedAlternate
    {
        get
        {
            if (defences == null)
                return false;

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

    public IEnumerator Culculate(LogManager logManager, bool isButttle = true)
    {
        //計算できない状態なら
        if (!CanCulculate())
            //終わり
            yield break;

        float logInterVal = 0;
        if (isButttle)
        {
            logInterVal = ButtleManager.BUTTLE_LOG_INTERVAL;
        }

        //ログの初期化
        logManager.ResetLog(isButttle);

        Skill skill = null;
        if (thing is Skill)
        {
            skill = (Skill)thing;
        }
        else
        {
            //バトルなら
            if (isButttle)
            {
                //技を取得
                skill = (Skill)ThingEngine.Instance.Get(((Equipment)thing).useSkillKey);
            }
        }

        string useSkillLog = null;
        //技なら
        if (thing is Skill)
        {
            switch (skill.skillType)
            {
                case Skill.SkillType.Normal:
                    useSkillLog = offence.name + "のこうげき！";
                    break;
                case Skill.SkillType.Skill:
                    useSkillLog = offence.name + "は" + thing.name + "を使った！";
                    break;
                case Skill.SkillType.Magic:
                    useSkillLog = offence.name + "は" + thing.name + "を唱えた！";
                    break;
                case Skill.SkillType.Item:
                    //アイテムを消費
                    offence.items.Remove(skill);
                    useSkillLog = offence.name + "は" + thing.name + "を使った！";
                    break;
            }
        }
        //装備なら
        else if (thing is Equipment)
        {
            useSkillLog = offence.name + "は" + thing.name + "を使った！";
        }

        //攻撃実数値
        var offencePoint = 0f;
        //MPが使えたか
        var isUseMP = false;
        //技があるならMPを消費
        if (skill != null)
        {
            offencePoint = skill.power;

            isUseMP = offence.mp >= skill.consumeMP;
            //mpがあるなら
            if (isUseMP)
            {
                //mp消費
                offence.mp -= skill.consumeMP;
            }
        }
        //ログの追加表示
        yield return logManager.PrintStr(useSkillLog);
        //敵の攻撃なら
        if (!offence.isFriend)
        {
            //画像を点滅(明暗を切り替え)
            yield return ((EnemyChara)offence).enemyObj.GetComponent<BlinkImage>().BlinkEnemyImage(blinkTime, false);
            //少し待つ
            yield return new WaitForSeconds(logInterVal - blinkTime);
        }
        else
        {
            //少し待つ
            yield return new WaitForSeconds(logInterVal);
        }

        //技が使えるなら
        if (skill != null)
        {
            //mpがないなら
            if (!isUseMP)
            {
                //ログ
                string noMPLog = "しかしMPが足りない！";
                //ログの追加表示
                yield return logManager.PrintStr(noMPLog);
                //少し待つ
                yield return new WaitForSeconds(logInterVal);
                //終わり
                yield break;
            }

            switch (skill.skillType)
            {
                case Skill.SkillType.Magic:
                    if (skill.isCure || skill.isMP)
                    {
                        offencePoint += offence.mtp;
                    }
                    else
                    {
                        offencePoint *= offence.mtp;
                    }
                    break;
                default:
                    if (skill.isCure || skill.isMP)
                    {
                        offencePoint += offence.atp;
                    }
                    else
                    {
                        offencePoint *= offence.atp;
                    }
                    break;
            }

            foreach (ButtleChara defence in defences)
            {
                //守備が死んでいるなら飛ばす
                if (defence.isDead)
                    continue;


                var defencePoint = skill.isCure ? 1 : defence.df;
                var actualPoint = defence.ChangeHPORMPValue((int)((float)offencePoint / defencePoint), skill.isCure, skill.isMP);
                var actualPointABS = Mathf.Abs(actualPoint);
                var pointName = skill.isMP ? "MP" : "HP";

                string changePointLog = null;

                if (skill.isCure)
                {
                    changePointLog = defence.name + "の" + pointName + "が" + actualPointABS + "回復した！";
                }
                else
                {
                    if (skill.isMP)
                    {
                        changePointLog = defence.name + "の" + pointName + "が" + actualPointABS + "減った！";
                    }
                    else
                    {
                        if (actualPointABS != 0)
                        {
                            if (defence.isFriend)
                            {
                                changePointLog = defence.name + "は" + actualPointABS + "のダメージを受けた！";
                            }
                            else
                            {
                                changePointLog = defence.name + "に" + actualPointABS + "のダメージを与えた！";
                            }
                        }
                        else
                        {
                            changePointLog = "ミス！" + offence.name + "はダメージを与えられない！";
                        }
                    }
                }

                //ログの追加表示
                yield return logManager.PrintStr(changePointLog);
                if (!defence.isFriend)
                {
                    //画像を点滅(透明度を切り替え)
                    yield return ((EnemyChara)defence).enemyObj.GetComponent<BlinkImage>().BlinkEnemyImage(blinkTime, true);
                    //死んでしまったら
                    if (defence.isDead)
                    {
                        //敵の画像を非表示にする
                        ((EnemyChara)defence).enemyObj.SetActive(false);
                    }
                    //少し待つ
                    yield return new WaitForSeconds(logInterVal - blinkTime);
                }
                else
                {
                    //少し待つ
                    yield return new WaitForSeconds(logInterVal);
                }

                if (defence.isDead)
                {
                    string deadlog = null;

                    if (defence.isFriend)
                    {
                        deadlog = defence.name + "は死んでしまった！";
                    }
                    else
                    {
                        deadlog = defence.name + "を倒した！";
                    }

                    //ログの追加表示
                    yield return logManager.PrintStr(deadlog);
                    //少し待つ
                    yield return new WaitForSeconds(logInterVal);
                }
            }
        }
        else
        {
            //ログの追加表示
            yield return logManager.PrintStr("...");
            //少し待つ
            yield return new WaitForSeconds(logInterVal * 2);

            //ログの追加表示
            yield return logManager.PrintStr("しかし、何も起こらなかった。");
            //少し待つ
            yield return new WaitForSeconds(logInterVal);
            //終わり
            yield break;
        }
    }

    private bool CanCulculate()
    {

        if (offence.isDead)
            return false;

        if (defences == null)
            return true;

        bool isEnemyAllDead = true;

        foreach(var defence in defences)
        {
            isEnemyAllDead &= defence.isDead;
        }

        return !isEnemyAllDead;
    }
}
