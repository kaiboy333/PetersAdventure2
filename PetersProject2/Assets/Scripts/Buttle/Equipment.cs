using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Thing
{
    public int atp = 0;  //AttackPower
    public int mtp = 0; //MagicAttackPower
    public int df = 0;  //Defence

    public int useSkillKey = 0;

    public bool isEquiped = false;

    //装備タイプ
    public enum EquipmentType
    {
        WEAPON,//武器
        SHIELD,//盾
        PANTS,//ズボン
    }
    public EquipmentType equipmentType = EquipmentType.WEAPON;

    public Equipment(string name, int atp, int mtp, int df, int useSkillKey, EquipmentType equipmentType) : base(name)
    {
        this.atp = atp;
        this.mtp = mtp;
        this.df = df;
        this.useSkillKey = useSkillKey;
        this.equipmentType = equipmentType;
    }
}
