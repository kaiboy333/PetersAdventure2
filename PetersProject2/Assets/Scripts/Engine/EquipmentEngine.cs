using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentEngine : Engine<Equipment>
{
    protected override string loadTextPath => "EquipmentEngine";

    public override Equipment CloneValue(Equipment t)
    {
        return (Equipment)t.Clone();
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
            var atp = int.Parse(strs_t[1]);
            var mtp = int.Parse(strs_t[2]);
            var df = int.Parse(strs_t[3]);
            var skillKey = int.Parse(strs_t[4]);

            var equipment = new Equipment(name, atp, mtp, df, skillKey);

            dictionary.Add(i, equipment);

            i++;
        }
    }
}
