using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Thing
{
    //消費MP
    public int consumeMP { get; private set; }

    public enum SkillType
    {
        Normal,
        Skill,
        Magic,
    }
    public SkillType skillType = SkillType.Normal;

    public Skill(string name, int power, bool isCure, bool isAll, bool isMP, int consumeMP, SkillType skillType) : base(name, power, isCure, isAll, isMP)
    {
        this.consumeMP = consumeMP;
        this.skillType = skillType;
    }
}
