using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThingEngine : Engine<Thing>
{
    protected override string loadTextPath => "ThingEngine";

    public override Thing CloneValue(Thing t)
    {
        return (Thing)t.Clone();
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

            Thing thing = null;

            var strs_t = str.Split(',');

            var thingType = strs_t[0];
            var name = strs_t[1];

            //装備なら
            if(thingType.Equals("Equipment"))
            {
                var atp = int.Parse(strs_t[2]);
                var mtp = int.Parse(strs_t[3]);
                var df = int.Parse(strs_t[4]);
                var skillKey = int.Parse(strs_t[5]);

                thing = new Equipment(name, atp, mtp, df, skillKey);
            }
            //Skillなら
            else
            {
                var power = int.Parse(strs_t[2]);
                var isCure = bool.Parse(strs_t[3]);
                var isAll = bool.Parse(strs_t[4]);
                var isMP = bool.Parse(strs_t[5]);
                var consumeMP = int.Parse(strs_t[6]);
                Skill.SkillType skillType = Skill.SkillType.Normal;
                switch (thingType)
                {
                    case "Normal":
                        skillType = Skill.SkillType.Normal;
                        break;
                    case "Skill":
                        skillType = Skill.SkillType.Skill;
                        break;
                    case "Magic":
                        skillType = Skill.SkillType.Magic;
                        break;
                    case "Item":
                        skillType = Skill.SkillType.Item;
                        break;
                }

                thing = new Skill(name, power, isCure, isAll, isMP, consumeMP, skillType);
            }

            dictionary.Add(i, thing);

            i++;
        }
    }
}
