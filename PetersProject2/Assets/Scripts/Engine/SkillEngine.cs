using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkillEngine : Engine<Skill>
{
    protected override string loadTextPath => "SkillEngine";

    public override Skill CloneValue(Skill t)
    {
        return (Skill)t.Clone();
    }

    protected override void LoadDictionary(TextAsset textAsset)
    {
        var strs = textAsset.text.Split('\n');
        int i = 0;
        foreach(var str in strs)
        {
            //空白なら飛ばす
            if (str == "")
                continue;
            //最初の文字が#か飛ばす
            if (str[0] == '#')
                continue;

            var strs_t = str.Split(',');

            var name = strs_t[0];
            var power = int.Parse(strs_t[1]);
            var isCure = bool.Parse(strs_t[2]);
            var isAll = bool.Parse(strs_t[3]);
            var isMP = bool.Parse(strs_t[4]);
            var consumeMP = int.Parse(strs_t[5]);
            Skill.SkillType skillType = (Skill.SkillType)Enum.GetValues(typeof(Skill.SkillType)).GetValue(int.Parse(strs_t[6]));

            var skill = new Skill(name, power, isCure, isAll, isMP, consumeMP, skillType);

            dictionary.Add(i, skill);

            i++;
        }
    }
}
