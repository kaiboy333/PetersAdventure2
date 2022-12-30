using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YushaController : CharaController
{
    //現れる位置
    public static Vector2 firstPos;

    public ControllManager controllManager = null;

    // Start is called before the first frame update
    protected override void Start()
    {

        //初期位置に移動
        transform.position = firstPos;

        //タイルによる位置修正
        base.Start();
    }

    public CellEvent GetCellEvent(Vector2 targetPos, CellEvent.CellType cellType)
    {
        var cellEvents = new List<CellEvent>();
        var hitColliders = Physics2D.OverlapCircleAll(targetPos, moveDistance / 4.0f, cellMask);

        foreach (var hitCollider in hitColliders)
        {
            //当たったコライダーが自分のとは違うものなら
            if (hitCollider.gameObject != this.gameObject)
            {
                //CellEventを取得(Onタイプ)
                var cellEvent = hitCollider.gameObject.GetComponent<CellEvent>();
                if (cellEvent)
                {
                    //CellTypeが一致したら
                    if(cellEvent.cellType == cellType)
                    {
                        cellEvents.Add(cellEvent);
                    }
                }
            }
        }

        //イベントがあるなら
        if(cellEvents.Count != 0)
        {
            //優先順位でソート
            cellEvents.Sort((a, b) =>
            {
                return b.priorityNo - a.priorityNo;
            });
            return cellEvents[0];
        }
        else
        {
            return null;
        }
    }

    protected override void ArriveTargetPos()
    {
        //先頭だけ
        if(controllManager.leader == this)
        {
            //CellEventを取得
            var cellEvent = GetCellEvent(targetPos, CellEvent.CellType.ON);
            //CellEventがあるなら
            if (cellEvent)
            {
                //イベントを呼ぶ
                StartCoroutine(cellEvent.CallEvent());
            }
        }
        //後ろにいる人を取得
        var backYusha = controllManager.GetBackYusha(this);
        if (backYusha)
        {
            //後ろの人が動くキーをセット
            backYusha.key = key;
        }
    }
}
