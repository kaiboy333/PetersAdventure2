using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendChara : ButtleChara
{
    //職業名
    public string professionName = "No Professsion";

    public int level { get; private set; } = 1;

    public Equipment[] equipments = new Equipment[System.Enum.GetValues(typeof(Equipment.EquipmentType)).Length];

    //public float atp2 = 0;
    //public float mtp2 = 0;
    //public float df2 = 0;

    //エンジンでの番号
    public int no { get; private set; }

    public FriendChara(string name, int hp, int mp, int atp, int mtp, int df, int speed, string professionName, int no) : base(name, hp, mp, atp, mtp, df, speed)
    {
        this.professionName = professionName;
        //this.atp2 = atp;
        //this.mtp2 = mtp;
        //this.df2 = df;
        this.no = no;
    }

    //装備をする
    public void Equip(Equipment equipment)
    {
        var equipmentType = equipment.equipmentType;
        //装備を外す
        RemoveEquipment(equipmentType);
        //装備の能力分アップ
        atp += equipment.atp;
        mtp += equipment.mtp;
        df += equipment.df;
        //装備をする
        equipments[(int)equipmentType] = equipment;
        //boolをtrueに
        equipment.isEquiped = true;
    }

    //装備を外す
    public Equipment RemoveEquipment(Equipment.EquipmentType equipmentType)
    {
        var equipment = equipments[(int)equipmentType];
        //装備をしているなら
        if (equipment != null)
        {
            //装備の能力分ダウン
            atp -= equipment.atp;
            mtp -= equipment.mtp;
            df -= equipment.df;
            //外す
            equipments[(int)equipmentType] = null;
            //boolをfalseに
            equipment.isEquiped = false;
        }

        return equipment;
    }
}
