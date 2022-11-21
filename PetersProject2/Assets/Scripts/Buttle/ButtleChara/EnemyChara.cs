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
    //元の名前
    public string originalName {get; private set;}

    public EnemyChara(string name, int hp, int mp, int atp, int mtp, int df, int speed, string tribeName, string imagePath) : base(name, hp, mp, atp, mtp, df, speed)
    {
        this.tribeName = tribeName;
        this.image = Resources.Load<Image>(imagePath);
        originalName = name;
    }
}
