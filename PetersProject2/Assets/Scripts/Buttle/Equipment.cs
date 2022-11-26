using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Thing
{
    public int atp = 0;  //AttackPower
    public int mtp = 0; //MagicAttackPower
    public int df = 0;  //Defence

    public int useSkillKey = 0;

    public Equipment(string name, int atp, int mtp, int df, int useSkillKey) : base(name)
    {
        this.atp = atp;
        this.mtp = mtp;
        this.df = df;
        this.useSkillKey = useSkillKey;
    }
}
