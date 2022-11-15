using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEngine : Engine<EnemyChara>
{
    protected override string loadTextPath => "EnemyEngine.txt";

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
            var tribeName = strs_t[7];
            var imagePath = strs_t[8];

            var enemy = new EnemyChara(name, hp, mp, atp, mtp, df, speed, tribeName, imagePath);

            var strs_t2 = strs[i + 1].Split(',');

            foreach (var keyStr in strs_t2)
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

            dictionary.Add(j, enemy);
            j++;
        }
    }
}