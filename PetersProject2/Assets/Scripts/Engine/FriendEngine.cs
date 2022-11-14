using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendEngine : Engine<FriendChara>
{
    protected override string loadTextPath => "FriendEngine.txt";

    protected override void LoadDictionary(TextAsset textAsset)
    {
        var strs = textAsset.text.Split('\n');
        int j = 0;
        for (int i = 0; i < strs.Length; i += 2)
        {
            var strs_t = strs[i].Split(',');

            var name = strs_t[0];
            var hp = int.Parse(strs_t[1]);
            var mp = int.Parse(strs_t[2]);
            var atp = int.Parse(strs_t[3]);
            var mtp = int.Parse(strs_t[4]);
            var df = int.Parse(strs_t[5]);
            var speed = int.Parse(strs_t[6]);
            var professionName = strs_t[7];

            var friend = new FriendChara(name, hp, mp, atp, mtp, df, speed, professionName);

            var strs_t2 = strs[i + 1].Split(',');

            foreach (var keyStr in strs_t2)
            {
                var key = int.Parse(keyStr);
                var skill = SkillEngine.Instance.Get(key);
                switch (skill.skillType)
                {
                    case Skill.SkillType.Normal:
                        friend.normalSkillKey = key;
                        break;
                    case Skill.SkillType.Skill:
                        friend.skillkeys.Add(key);
                        break;
                    case Skill.SkillType.Magic:
                        friend.magickeys.Add(key);
                        break;
                }
            }

            dictionary.Add(j, friend);
            j++;
        }        
    }
}