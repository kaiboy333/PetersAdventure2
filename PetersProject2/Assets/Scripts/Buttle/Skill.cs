using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Thing
{
    //威力
    public int power { get; private set; }
    //回復するか
    public bool isCure { get; private set; }
    //全体か
    public bool isAll { get; private set; }
    //MP関連か
    public bool isMP { get; private set; }

    //消費MP
    public int consumeMP { get; private set; }

    public enum SkillType
    {
        Normal,
        Skill,
        Magic,
        Item,
    }
    public SkillType skillType = SkillType.Normal;

    public Skill(string name, int power, bool isCure, bool isAll, bool isMP, int consumeMP, SkillType skillType) : base(name)
    {
        this.power = power;
        this.isCure = isCure;
        this.isAll = isAll;
        this.isMP = isMP;

        this.consumeMP = consumeMP;
        this.skillType = skillType;
        //アイテムなら
        if(skillType == SkillType.Item)
        {
            //消費MPは0
            this.consumeMP = 0;
        }
    }
}
