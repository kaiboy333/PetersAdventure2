using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YushaController : CharaController
{
    private readonly List<Key> keys = new List<Key>();

    //現れる位置
    public static Vector2 firstPos;

    //向いている方向
    public Vector2 direction { get; private set; }

    // Start is called before the first frame update
    protected override void Start()
    {

        //初期位置に移動
        transform.position = firstPos;

        //タイルによる位置修正
        base.Start();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            keys.Add(Key.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            keys.Add(Key.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            keys.Add(Key.UP);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            keys.Add(Key.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            keys.Remove(Key.RIGHT);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            keys.Remove(Key.LEFT);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            keys.Remove(Key.UP);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            keys.Remove(Key.DOWN);
        }

        //if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        //{
        //    keys.Clear();
        //    Debug.Log("No Move");
        //}

        if (keys.Count > 0)
        {
            key = keys[keys.Count - 1];
            //向いている向きを記憶
            direction = directions[(int)key];
        }
        else
        {
            key = Key.NONE;
        }

        if (!canMove)
            return;

        Move();

        //動いていないときに
        if (!isMoving)
        {
            //スペースキーを押したら
            if(Input.GetKeyDown(KeyCode.Space)) {
                //CellEventを取得(Checkタイプ)
                var cellEvent = GetCellEvent(GetNextTargetPos(direction), CellEvent.CellType.Check);
                //CellEventがあるなら
                if (cellEvent)
                {
                    //イベントを呼ぶ
                    StartCoroutine(cellEvent.CallEvent());
                }
            }
        }
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
        //CellEventを取得
        var cellEvent = GetCellEvent(targetPos, CellEvent.CellType.ON);
        //CellEventがあるなら
        if (cellEvent)
        {
            //イベントを呼ぶ
            StartCoroutine(cellEvent.CallEvent());
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider2D)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(GetNextTargetPos(direction), sphereRadious);
        }
    }
}
