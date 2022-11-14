using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class CharaController : MonoBehaviour
{
    protected float speed = 2f;
    protected readonly Vector2[] directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    protected float moveDistance;
    protected LayerMask hitMask;
    protected LayerMask cellMask;
    protected BoxCollider2D boxCollider2D = null;
    private Tilemap tilemap = null;

    protected Animator animator = null;

    protected Vector2 targetPos;

    protected bool isMoving = false;

    protected Key key = Key.NONE;

    private EventTaskManager eventTaskManager = null;
    public bool CanMove { get { return !eventTaskManager.IsWorking; } }

    //当たり判定に使う球の半径
    protected float sphereRadious = 0;

    public enum Key
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,
        NONE,
    }

    protected virtual void Start()
    {
        hitMask = LayerMask.GetMask("Sea", "Player", "Wall");
        cellMask = LayerMask.GetMask("CellEvent");

        //タイルマップによる位置調整
        tilemap = FindObjectOfType<Tilemap>();

        if (tilemap)
        {
            moveDistance = tilemap.cellSize.x;

            var cellPos = tilemap.WorldToCell(transform.position);

            var complementPos = new Vector3(tilemap.cellSize.x / 2.0f, tilemap.cellSize.y / 2.0f, 0);

            transform.position = tilemap.CellToWorld(cellPos) + complementPos;

            //半径を設定
            sphereRadious = moveDistance / 3.0f;
        }

        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        //目標位置初期化
        targetPos = transform.position;

        eventTaskManager = FindObjectOfType<EventTaskManager>();
    }

    //目的地まで歩けるか
    protected bool CanWalk(Vector2 targetPos)
    {
        var hitColliders = Physics2D.OverlapCircleAll(targetPos, sphereRadious, hitMask);

        foreach(var hitCollider in hitColliders)
        {
            //当たったコライダーが自分のとは違うものなら
            if(hitCollider.gameObject != this.gameObject)
            {
                //壁がある
                return false;
            }
        }

        return true;
    }

    protected void Move()
    {
        //動いていないなら
        if (!isMoving)
        {
            if (key == Key.NONE)
                return;

            //最後に押しているキーを見て移動
            var direction = directions[(int)key];

            targetPos = GetNextTargetPos(direction);

            //歩けるなら
            if (CanWalk(targetPos))
            {
                //trueに
                isMoving = true;
            }

            //アニメーション再生
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }
        //動いているなら
        else
        {
            //移動
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            //たどり着いたら
            if (Vector2.Distance(targetPos, transform.position) < Mathf.Epsilon)
            {
                //false
                isMoving = false;

                //ついたときの関数を呼ぶ
                ArriveTargetPos();
            }
        }
    }

    protected Vector2 GetNextTargetPos(Vector2 direction)
    {
        return (Vector2)transform.position + direction * moveDistance;
    }

    protected abstract void ArriveTargetPos();

    private void OnDrawGizmos()
    {
        if (boxCollider2D)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(targetPos, sphereRadious);
        }
    }
}
