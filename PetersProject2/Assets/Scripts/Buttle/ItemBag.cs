using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBag
{
    public readonly List<Thing> items = new List<Thing>();
    private bool isRestrict = true;
    private const float MAX_ITEM_LEN = 8;

    public ItemBag(bool isRestrict)
    {
        this.isRestrict = isRestrict;
    }

    private bool CanAddItem()
    {
        //アイテムの数を制限されているなら
        return isRestrict && items.Count < MAX_ITEM_LEN || !isRestrict;
    }


    public bool AddItem(Thing thing)
    {
        //追加できないなら
        if (!CanAddItem())
            //終わり
            return false;

        if (thing is Skill skill)
        {
            //アイテム以外なら
            if (skill.skillType != Skill.SkillType.Item)
                //なしにする
                thing = null;
        }

        if(thing != null)
        {
            items.Add(thing);
        }

        return thing != null;
    }

    public Thing RemoveItem(int no)
    {
        Thing item = items[no];
        items.Remove(item);
        return item;
    }

    public Thing RemoveItem(Thing thing)
    {
        items.Remove(thing);
        return thing;
    }
}
