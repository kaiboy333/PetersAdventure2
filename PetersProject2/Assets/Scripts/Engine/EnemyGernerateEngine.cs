using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGernerateEngine : Engine<List<EnemyChara>>
{
    protected override string loadTextPath => "EnemyGenerateEngine";

    public override List<EnemyChara> CloneValue(List<EnemyChara> t)
    {
        return new List<EnemyChara>(t);
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

            var enemyCharas = new List<EnemyChara>();

            foreach (var keyStr in strs_t)
            {
                var key = int.Parse(keyStr);
                //敵を取得
                var enemyChara = EnemyEngine.Instance.Get(key);
                //追加
                enemyCharas.Add(enemyChara);
            }
            //辞書に追加
            dictionary.Add(i, enemyCharas);
            i++;
        }
    }
}
