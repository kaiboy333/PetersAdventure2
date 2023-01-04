using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharaController;

public class ControllManager : MonoBehaviour
{
    private readonly List<Key> keys = new List<Key>();

    //現れる位置
    public static Vector2 firstPos;

    //向いている方向
    public Vector2 direction { get; private set; }

    private List<YushaController> yushas = new List<YushaController>();
    [SerializeField] private GameObject[] yushaPrefabs;

    public YushaController leader { get { return yushas.Count > 0 ? yushas[0] : null; } }

    [SerializeField] private Image blackPanelImage = null;

    [SerializeField] private PlayerCamera playerCamera = null;

    // Start is called before the first frame update
    private void Start()
    {
        foreach(FriendChara friendChara in ButtleManager.friendCharas)
        {
            var yusha = Instantiate(yushaPrefabs[friendChara.no]).GetComponent<YushaController>();
            yusha.controllManager = this;
            yusha.blackPanelImage = blackPanelImage;
            yushas.Add(yusha);
        }
        playerCamera.charaController = leader;
    }

    // Update is called once per frame
    protected void Update()
    {
        KeyCheck();

        Investigate();
    }

    private void KeyCheck()
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

        if (keys.Count > 0 && !leader.isMoving && leader.canMove && !FragEvent.isEvent)
        {
            AllMove(keys[keys.Count - 1]);
        }
    }

    private void Investigate()
    {
        //スペースキーを押したら
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //動いていないときにキーがNONE以外なら
            if (!leader.isMoving && leader.canMove && leader.key != Key.NONE)
            {
                //CellEventを取得(Checkタイプ)
                var cellEvent = leader.GetCellEvent(leader.GetNextTargetPos(leader.direction), CellEvent.CellType.Check);
                //CellEventがあるなら
                if (cellEvent)
                {
                    //イベントを呼ぶ
                    StartCoroutine(cellEvent.CallEvent());
                }
            }
        }
    }

    public YushaController GetBackYusha(YushaController yusha)
    {
        var index = yushas.IndexOf(yusha);
        if(index < yushas.Count - 1 && index >= 0)
        {
            return yushas[index + 1];
        }
        return null;
    }

    public void AllMove(Key key)
    {
        leader.key = key;
        //向いている向きを記憶
        direction = directions[(int)leader.key];
        //アニメーション再生
        leader.animator.SetFloat("MoveX", direction.x);
        leader.animator.SetFloat("MoveY", direction.y);

        var targetPos = leader.GetNextTargetPos(direction);
        //歩けるなら
        if (leader.CanWalk(targetPos))
        {
            //全員歩く
            foreach (var yusha in yushas)
            {
                StartCoroutine(yusha.Move());
            }
        }
    }

    public void AllTransform(Vector2 pos)
    {
        foreach(var yusha in yushas)
        {
            yusha.transform.position = pos;
        }
    }
}
