using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureEvent : CellEvent
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private LogManager logManager = null;
    [SerializeField] private int thingNo = 0;
    private bool isOpen = false;

    protected override void Start()
    {
        base.Start();
    }

    public override IEnumerator CallEvent()
    {
        //空いているなら何もしない
        if (isOpen)
            yield break;

        isOpen = true;

        if (animator)
        {
            //宝箱を開いた状態にする
            animator.SetBool("IsOpen", isOpen);
        }

        if (logManager)
        {
            var thing = ThingEngine.Instance.Get(thingNo);
            if (thing is Skill skill)
            {
                //アイテム以外なら
                if (skill.skillType != Skill.SkillType.Item)
                    //なしにする
                    thing = null;
            }

            var logs = new List<string>();
            logs.Add(ButtleManager.friendCharas[0].name + "は宝箱をあけた！");
            logs.Add("");
            logs.Add("");

            if (thing != null)
            {
                logs.Add(thing.name + "を手に入れた！");
                //実際にアイテムを追加
                ButtleManager.GetItem(thing);
            }
            else
            {
                logs.Add("しかし、中身は空っぽだった。");
            }

            yield return logManager.PrintLog(logs);
        }
    }
}
