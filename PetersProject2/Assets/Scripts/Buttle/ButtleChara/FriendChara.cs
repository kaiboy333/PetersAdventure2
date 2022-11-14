using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendChara : ButtleChara
{
    //職業名
    public string professionName = "No Professsion";

    private int level = 1;

    public FriendChara(int hp, int mp, int atp, int mtp, int df, int speed, string name, string professionName) : base(hp, mp, atp, mtp, df, speed, name)
    {
        this.professionName = professionName;
    }
}
