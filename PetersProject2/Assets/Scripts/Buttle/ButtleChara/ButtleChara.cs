using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ButtleChara : ICloneable
{
    //ステータス
    public int hp = 0;  //HitPoint
    public int mp = 0;  //MagicPoint

    public int maxHP = 0;
    public int maxMP = 0;
    public int atp = 0;  //AttackPower
    public int mtp = 0; //MagicAttackPower
    public int df = 0;  //Defence
    public int speed = 0;

    //キャラ
    public string name = "No Name";

    //死んだか
    public bool isDead { get { return hp == 0; } }

    //味方サイドか
    public bool isFriend { get { return this is FriendChara; } }

    //特技
    public List<int> skillKeys = new List<int>();
    //魔法
    public List<int> magicKeys = new List<int>();
    //通常攻撃
    public int normalSkillKey = 0;

    public ButtleChara(string name, int hp, int mp, int atp, int mtp, int df, int speed)
    {
        this.hp = hp;
        this.mp = mp;

        this.maxHP = hp;
        this.maxMP = mp;
        this.atp = atp;
        this.mtp = mtp;
        this.df = df;
        this.speed = speed;

        this.name = name;
    }

    //HPかMPの変化分の値を取得する
    public int GetChangeHPORMPValue(int point, bool isCure, bool isMP)
    {
        var newPoint = isMP ? mp : hp;
        var beforePoint = newPoint;
        var maxPoint = isMP ? maxMP : maxHP;
        var dir = isCure ? 1 : -1;

        newPoint = Mathf.Clamp(newPoint + dir * point, 0, maxPoint);

        return newPoint - beforePoint;
    }

    //全回復
    public void CureAll()
    {
        hp = maxHP;
        mp = maxMP;
    }

    public object Clone()
    {
        return (ButtleChara)this.MemberwiseClone();
    }
}
