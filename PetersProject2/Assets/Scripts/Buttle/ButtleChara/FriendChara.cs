using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendChara : ButtleChara
{
    //職業名
    public string professionName = "No Professsion";

    public int level { get; private set; }

    public FriendChara(string name, int hp, int mp, int atp, int mtp, int df, int speed, string professionName) : base(name, hp, mp, atp, mtp, df, speed)
    {
        this.professionName = professionName;
    }
}
