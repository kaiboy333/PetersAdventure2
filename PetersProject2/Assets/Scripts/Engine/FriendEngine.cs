using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendEngine : Engine<FriendChara>
{
    protected override string loadTextPath => "FriendEngine";

    private const int READ_MAX_ROW = 2;

    public override FriendChara CloneValue(FriendChara t)
    {
        return (FriendChara)t.Clone();
    }

    protected override void LoadDictionary(TextAsset textAsset)
    {
        var strs = textAsset.text.Split('\n');
        int i = 0;
        foreach (var str in strs)
        {
            //空白なら飛ばす
            if (str == "")
                continue;
            //最初の文字が#か飛ばす
            if (str[0] == '#')
                continue;

            var strs_t = str.Split(',');

            if (i % READ_MAX_ROW == 0)
            {
                var name = strs_t[0];
                var hp = int.Parse(strs_t[1]);
                var mp = int.Parse(strs_t[2]);
                var atp = int.Parse(strs_t[3]);
                var mtp = int.Parse(strs_t[4]);
                var df = int.Parse(strs_t[5]);
                var speed = int.Parse(strs_t[6]);
                var professionName = strs_t[7];

                var friend = new FriendChara(name, hp, mp, atp, mtp, df, speed, professionName, i / READ_MAX_ROW);

                dictionary.Add(i / READ_MAX_ROW, friend);
            }
            else if(i % READ_MAX_ROW == 1)
            {
                var friend = dictionary[i / READ_MAX_ROW];

                foreach (var keyStr in strs_t)
                {
                    var key = int.Parse(keyStr);
                    var thing = ThingEngine.Instance.Get(key);
                    if(thing is Skill)
                    {
                        var skill = (Skill)thing;
                        switch (skill.skillType)
                        {
                            case Skill.SkillType.Normal:
                                friend.normalSkillKey = key;
                                break;
                            case Skill.SkillType.Skill:
                                friend.skillKeys.Add(key);
                                break;
                            case Skill.SkillType.Magic:
                                friend.magicKeys.Add(key);
                                break;
                            case Skill.SkillType.Item:
                                friend.itemBag.AddItem(skill);
                                break;
                        }
                    }
                    else if (thing is Equipment)
                    {
                        friend.itemBag.AddItem(thing);
                    }
                }

            }

            i++;
        }
    }
}