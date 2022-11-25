using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Thing
{
    public int atp = 0;  //AttackPower
    public int mtp = 0; //MagicAttackPower
    public int df = 0;  //Defence

    public Skill useSkill = null;

    public Equipment(string name, int atp, int mtp, int df) : base(name)
    {
        this.atp = atp;
        this.mtp = mtp;
        this.df = df;
    }
}
