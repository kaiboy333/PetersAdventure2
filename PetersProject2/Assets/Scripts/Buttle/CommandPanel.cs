using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPanel : MonoBehaviour
{
    //コマンドたち
    private List<Command> commands = new List<Command>();
    private int row = 0, col = 0;
    private int printRow = 0, printCol = 0;
    //private int maxPage = 0;
    //private int nowPage = 0;
    //矢印が指している番号
    private int nowNo = 0;
    //現在表示している行の一番上
    private int printNowRowNo = 0;

    [SerializeField] private GameObject commandPrefab;
    [SerializeField] private GameObject arrowPrefab;

    public RectTransform frameRect;
    [SerializeField] private RectTransform backRect;
    [SerializeField] private RectTransform contentRect;

    private const float BLANK = 10;
    private Vector2 commandSizeDelta;
    private float arrowWidth;
    private const float FRAME_WIDTH = 12.5f;

    //縦にスクロールするか
    private bool isColScroll = true;

    //一個前のCommandPanel
    [HideInInspector] public CommandPanel beforeCommandPanel = null;

    private RectTransform arrowRect = null;

    //コンテントがスクロール一個分動かす距離
    private float scrollDistance = 0;

    //表示だけであるか
    public bool isOnlyPrint = false;

    private void Awake()
    {
        //コマンドの幅高さ取得
        var commandRect = commandPrefab.GetComponent<RectTransform>();
        commandSizeDelta = new Vector2(commandRect.sizeDelta.x, commandRect.sizeDelta.y);
        //矢印の横幅取得
        var arrowRect = arrowPrefab.GetComponent<RectTransform>();
        arrowWidth = arrowRect.sizeDelta.x;

        //一個動く距離は余白*2とコマンドの高さを足した値
        scrollDistance = BLANK * 2 + commandSizeDelta.y;
    }

    public void MoveArrow()
    {
        //表示だけなら終わり
        if (isOnlyPrint)
            return;

        var nowRowNo = nowNo / col;
        var nowColNo = nowNo % col;

        //最後の行の最大列番号
        var lastRowNoColMaxNo = (nowRowNo == row - 1) ? commands.Count % col : col - 1;

        //縦スクロールで
        if (isColScroll)
        {
            //上を押した時
            if (Input.GetKeyDown(KeyCode.W))
            {
                //一番上なら
                if (nowRowNo == 0)
                    //終わり
                    return;
                //違うなら
                else
                {
                    //表示している番号が今のと一致するなら
                    if (printNowRowNo == nowRowNo)
                    {
                        //Contentを下へ一個移動
                        contentRect.anchoredPosition -= Vector2.up * scrollDistance;
                        //表示列をデクリメント
                        printNowRowNo--;
                    }
                    //一個上へ
                    nowNo -= col;
                }
            }
            //下を押した時
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //一番下なら
                if (nowRowNo == row - 1)
                    //終わり
                    return;
                //違うなら
                else
                {
                    //表示している番号 + printRow-1が今のと一致するなら
                    if (printNowRowNo + (printRow - 1) == nowRowNo)
                    {
                        //Contentを上へ一個移動
                        contentRect.anchoredPosition += Vector2.up * scrollDistance;
                        //表示列をインクリメント
                        printNowRowNo++;
                    }
                    //一個下が存在するなら(空白じゃないなら)
                    if(nowNo + col <= commands.Count - 1)
                    {
                        //一個下へ
                        nowNo += col;
                    }
                }
            }
            //左を押した時
            else if (Input.GetKeyDown(KeyCode.A))
            {
                //一番左なら
                if (nowColNo == 0)
                {
                    ////一番右にする
                    //nowNo += lastRowNoColMaxNo;
                    return;
                }
                //違うなら
                else
                {
                    //一個左へ
                    nowNo -= 1;
                }
            }
            //右を押した時
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //一番右なら
                if (nowColNo == lastRowNoColMaxNo)
                {
                    ////一番左にする
                    //nowNo -= lastRowNoColMaxNo;
                    return;
                }
                //違うなら
                else
                {
                    //一個右が存在するなら(空白じゃないなら)
                    if (nowNo + 1 <= commands.Count - 1)
                    {
                        //一個右へ
                        nowNo += 1;
                    }
                }
            }
        }
        else
        {
        }

        //矢印の位置移動
        //指定のコマンドの左に設定
        arrowRect.anchoredPosition = commands[nowNo].GetComponent<RectTransform>().anchoredPosition - Vector2.right * arrowWidth;
    }

    //コマンドの名前検索
    public Command FindCommand(string name)
    {
        foreach(Command command in commands)
        {
            if(command.Name == name)
            {
                return command;
            }
        }

        return null;
    }

    //コマンドを追加
    public void AddCommand(Command command)
    {
        commands.Add(command);

        //コマンドパネルセット
        command.commandPanel = this;

        var length = commands.Count;
        //ページ数セット
        //maxPage = length / (row * col);
        //今回追加したやつの番号を取得
        var nowNo = length - 1;
        //位置調整(Contentを基準に移動)
        var commandRect = command.gameObject.GetComponent<RectTransform>();
        //var a = nowNo % (row * col);
        var w = nowNo % col;
        var deltaWidth = (w + 1) * (BLANK + arrowWidth) + w * commandSizeDelta.x;
        var h = nowNo / col;
        var deltaHeight = (h + 1) * BLANK + h * (commandSizeDelta.y + BLANK);

        commandRect.anchoredPosition = new Vector2(deltaWidth, -deltaHeight);
    }

    //public void RemoveCommand(Command command)
    //{
    //    commands.Remove(command.Name);
    //}

    public void Init(Vector2 framePos, List<string> strs, int printRow, int printCol, bool isColScroll, bool isOnlyPrint)
    {
        var lastNo = strs.Count - 1;
        this.row = (lastNo / printCol) + 1;
        this.col = printCol;
        this.printRow = printRow;
        this.printCol = printCol;
        this.isOnlyPrint = isOnlyPrint;

        //表示だけなら
        if (isOnlyPrint)
        {
            //矢印の幅なしにする
            arrowWidth = 0;
        }

        //フレームの位置調整
        frameRect.position = framePos;
        //フレームの大きさ調整
        frameRect.sizeDelta = new Vector2(FRAME_WIDTH * 2 + BLANK * (printCol + 1) + (commandSizeDelta.x + arrowWidth) * printCol, FRAME_WIDTH * 2 + BLANK * (printRow * 2) + commandSizeDelta.y * printRow);

        //バックの位置調整
        backRect.anchoredPosition = new Vector2(1, -1) * FRAME_WIDTH;
        //バックの大きさ調整
        backRect.sizeDelta = frameRect.sizeDelta - 2 * FRAME_WIDTH * Vector2.one;

        for (int i = 0; i < strs.Count; i++)
        {
            var str = strs[i];

            //コマンドオブジェクトを生成
            var commandObj = Instantiate(commandPrefab, contentRect);
            var command = commandObj.GetComponent<Command>();
            //名前セット
            command.Name = str;
            //コマンド追加
            AddCommand(command);

            //最初は
            if (i == 0 && !isOnlyPrint)
            {
                //矢印のオブジェクト生成
                var arrowObj = Instantiate(arrowPrefab, contentRect);
                arrowRect = arrowObj.GetComponent<RectTransform>();
                //最初のコマンドの左に設定
                arrowRect.anchoredPosition = commandObj.GetComponent<RectTransform>().anchoredPosition - Vector2.right * arrowWidth;
            }
        }

        this.isColScroll = isColScroll;
    }

    public Command GetSelectedCommand()
    {
        return commands[nowNo];
    }

    public List<Command> GetCommands()
    {
        return commands; 
    }
}
