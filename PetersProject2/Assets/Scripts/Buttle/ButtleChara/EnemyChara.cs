using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyChara : ButtleChara
{
    //種族名
    public string tribeName = "No Professsion";
    //敵の画像
    public Image image;

    public EnemyChara(int hp, int mp, int atp, int mtp, int df, string name, int speed, string tribeName, Image image) : base(hp, mp, atp, mtp, df, speed, name)
    {
        this.tribeName = tribeName;
        this.image = image;
    }
}
