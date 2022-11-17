using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEngine : Engine<EnemyChara>
{
    protected override string loadTextPath => "EnemyEngine";

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

            if(i % 2 == 0)
            {
                var name = strs_t[0];
                var hp = int.Parse(strs_t[1]);
                var mp = int.Parse(strs_t[2]);
                var atp = int.Parse(strs_t[3]);
                var mtp = int.Parse(strs_t[4]);
                var df = int.Parse(strs_t[5]);
                var speed = int.Parse(strs_t[6]);
                var tribeName = strs_t[7];
                var imagePath = strs_t[8];

                var enemy = new EnemyChara(name, hp, mp, atp, mtp, df, speed, tribeName, imagePath);

                dictionary.Add(i / 2, enemy);
            }
            else
            {
                var enemy = dictionary[i / 2];

                foreach (var keyStr in strs_t)
                {
                    var key = int.Parse(keyStr);
                    var skill = SkillEngine.Instance.Get(key);
                    switch (skill.skillType)
                    {
                        case Skill.SkillType.Normal:
                            enemy.normalSkillKey = key;
                            break;
                        case Skill.SkillType.Skill:
                            enemy.skillKeys.Add(key);
                            break;
                        case Skill.SkillType.Magic:
                            enemy.magicKeys.Add(key);
                            break;
                    }
                }

            }

            i++;
        }
    }
}