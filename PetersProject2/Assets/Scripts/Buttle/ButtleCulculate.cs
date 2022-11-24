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

    public IEnumerator Culculate(LogManager logManager)
    {
        //計算できない状態なら
        if (!CanCulculate())
            //終わり
            yield break;

        var offencePoint = 0;

        //ログの初期化
        logManager.ResetLog(true);

        //thingが技なら
        if (thing is Skill skill)
        {
            string useSkillLog = null;

            //mpがあるなら
            if (offence.mp >= skill.consumeMP)
            {
                //mp消費
                offence.mp -= skill.consumeMP;
            }

            switch (skill.skillType)
            {
                case Skill.SkillType.Normal:
                    useSkillLog = offence.name + "のこうげき！";
                    break;
                case Skill.SkillType.Skill:
                    useSkillLog = offence.name + "は" + skill.name + "を使った！";
                    break;
                case Skill.SkillType.Magic:
                    useSkillLog = offence.name + "は" + skill.name + "を唱えた！";
                    break;
            }

            //ログの追加表示
            yield return logManager.PrintButtleStr(useSkillLog);
            //敵の攻撃なら
            if (!offence.isFriend)
            {
                //画像を点滅(明暗を切り替え)
                yield return ((EnemyChara)offence).enemyObj.GetComponent<BlinkImage>().BlinkEnemyImage(blinkTime, false);
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL - blinkTime);
            }
            else
            {
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL);
            }

            //mpがないなら
            if (offence.mp < skill.consumeMP)
            {
                //ログ
                string noMPLog = "しかしMPが足りない！";
                //ログの追加表示
                yield return logManager.PrintButtleStr(noMPLog);
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL);
                //終わり
                yield break;
            }


            switch (skill.skillType)
            {
                case Skill.SkillType.Magic:
                    offencePoint = (int)(offence.mtp * thing.power);
                    break;
                default:
                    offencePoint = (int)(offence.atp + thing.power);
                    break;
            }
        }
        else
        {

        }

        foreach (ButtleChara defence in defences)
        {
            //守備が死んでいるなら飛ばす
            if (defence.isDead)
                continue;


            var defencePoint = thing.isCure ? 1 : defence.df;
            var actualPoint = defence.ChangeHPORMPValue((int)((float)offencePoint / defencePoint), thing.isCure, thing.isMP);
            var actualPointABS = Mathf.Abs(actualPoint);
            var pointName = thing.isMP ? "MP" : "HP";

            //if (thing.isMP)
            //{
            //    defence.mp += actualPoint;
            //}
            //else
            //{
            //    defence.hp += actualPoint;
            //}

            string changePointLog = null;

            if (thing.isCure)
            {
                changePointLog = defence.name + "の" + pointName + "が" + actualPointABS + "回復した！";
            }
            else
            {
                if (thing.isMP)
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
            yield return logManager.PrintButtleStr(changePointLog);
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
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL - blinkTime);
            }
            else
            {
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL);
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
                yield return logManager.PrintButtleStr(deadlog);
                //少し待つ
                yield return new WaitForSeconds(ButtleManager.BUTTLE_LOG_INTERVAL);
            }
        }
    }

    private bool CanCulculate()
    {
        if (offence.isDead)
            return false;

        bool isEnemyAllDead = true;

        foreach(var defence in defences)
        {
            isEnemyAllDead &= defence.isDead;
        }

        return !isEnemyAllDead;
    }

    public object Clone()
    {
        return (ButtleChara)this.MemberwiseClone();
    }
}
