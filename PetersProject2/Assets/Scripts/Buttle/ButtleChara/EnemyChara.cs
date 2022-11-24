using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChara : ButtleChara
{
    //種族名
    public string tribeName = "No Professsion";
    //敵の画像
    public Sprite sprite = null;
    //元の名前
    public string originalName {get; private set;}
    //表示する画像の横幅、縦幅
    public int width { get; private set; }
    public int height { get; private set; }

    public GameObject enemyObj = null;

    public EnemyChara(string name, int hp, int mp, int atp, int mtp, int df, int speed, string tribeName, string spritePath, int width, int height) : base(name, hp, mp, atp, mtp, df, speed)
    {
        this.tribeName = tribeName;
        this.sprite = Resources.Load<Sprite>(spritePath);
        this.width = width;
        this.height = height;
        originalName = name;
    }
}
