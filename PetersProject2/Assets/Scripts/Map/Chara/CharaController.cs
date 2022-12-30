using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class CharaController : MonoBehaviour
{
    protected float speed = 2f;
    public static Vector2[] directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    [HideInInspector] public Key key = Key.NONE;
    public float moveDistance { get; protected set; }
    protected LayerMask hitMask;
    public LayerMask cellMask { get; protected set; }
    protected BoxCollider2D boxCollider2D = null;
    private Tilemap tilemap = null;

    public Animator animator { get; protected set; }

    protected Vector2 targetPos;

    public bool isMoving { get; protected set; }

    //ログとコマンドパネルが見えないかつ、ブラックパネルのアルファ値が0でキーが何かあるなら
    public bool canMove { get { return !logManager.gameObject.activeInHierarchy && !CommandManager.Instance.nowCommandPanel && blackPanelImage.color.a == 0; } }
    //当たり判定に使う球の半径
    protected float sphereRadious = 0;

    private LogManager logManager = null;
    public Image blackPanelImage = null;

    public Vector2 direction { get { return directions[(int)key]; } }

    public enum Key
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,
        NONE,
    }

    private void Awake()
    {
        logManager = FindObjectOfType<LogManager>();
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
    }

    //目的地まで歩けるか
    public bool CanWalk(Vector2 targetPos)
    {
        var hitColliders = Physics2D.OverlapCircleAll(targetPos, sphereRadious, hitMask);

        foreach(var hitCollider in hitColliders)
        {
            //当たったコライダーが自分のとは違うものなら
            if(hitCollider.gameObject != this.gameObject && !hitCollider.gameObject.GetComponent<YushaController>() && this is YushaController)
            {
                //壁がある
                return false;
            }
        }

        return true;
    }

    public IEnumerator Move()
    {
        //歩けないなら
        if (key == Key.NONE || !canMove)
        {
            yield break;
        }

        var direction = directions[(int)key];
        targetPos = GetNextTargetPos(direction);

        //trueに
        isMoving = true;

        //アニメーション再生
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        while(Vector2.Distance(targetPos, transform.position) >= Mathf.Epsilon)
        {
            if (canMove)
            {
                //移動
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

                yield return null;
            }
        }

        //たどり着いたら
        //false
        isMoving = false;

        //ついたときの関数を呼ぶ
        ArriveTargetPos();
    }

    //protected void Move()
    //{
    //    //動いていないなら
    //    if (!isMoving)
    //    {
    //        if (key == Key.NONE)
    //            return;

    //        //最後に押しているキーを見て移動
    //        var direction = directions[(int)key];

    //        targetPos = GetNextTargetPos(direction);

    //        //歩けるなら
    //        if (CanWalk(targetPos))
    //        {
    //            //trueに
    //            isMoving = true;
    //            //ここから離れる時によぶ
    //            LeaveTargetPos();
    //        }

    //        //アニメーション再生
    //        animator.SetFloat("MoveX", direction.x);
    //        animator.SetFloat("MoveY", direction.y);
    //    }
    //    //動いているなら
    //    else
    //    {
    //        //移動
    //        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

    //        //たどり着いたら
    //        if (Vector2.Distance(targetPos, transform.position) < Mathf.Epsilon)
    //        {
    //            //false
    //            isMoving = false;

    //            //ついたときの関数を呼ぶ
    //            ArriveTargetPos();
    //        }
    //    }
    //}

    public Vector2 GetNextTargetPos(Vector2 direction)
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
