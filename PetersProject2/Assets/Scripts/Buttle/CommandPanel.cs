using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanel : MonoBehaviour
{
    //コマンドたち
    private List<Command> commands = new List<Command>();
    private int row { get { return ((commands.Count - 1) / printCol) + 1; } }
    private int col { get { return printCol; } }
    private int printRow = 0, printCol = 0;
    //矢印が指している番号
    public int nowNo { get; private set; }
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

    //コマンドがあるか
    public bool hasCommand { get { return commands.Count != 0; } }

    private void Awake()
    {
        //矢印の横幅取得
        var arrowPrefabRect = arrowPrefab.GetComponent<RectTransform>();
        arrowWidth = arrowPrefabRect.sizeDelta.x;
    }

    //非アクティブになったら
    private void OnDisable()
    {
        //番号を0にして
        nowNo = 0;
        if (arrowRect && commands.Count != 0)
        {
            //矢印を移動
            //指定のコマンドの左に設定
            arrowRect.anchoredPosition = commands[nowNo].GetComponent<RectTransform>().anchoredPosition - Vector2.right * arrowWidth;
        }
    }

    public void MoveArrow()
    {
        //表示だけなら終わり
        if (isOnlyPrint)
            return;

        var nowRowNo = nowNo / col;
        var nowColNo = nowNo % col;

        //最後の行の最大列番号
        var lastRowNoColMaxNo = (nowRowNo == row - 1) && (commands.Count % (col + 1) != 0) ? commands.Count % (col + 1) : col - 1;

        //前回として記憶
        var beforeNo = nowNo;

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

        //選択している番号が変わったら
        if(beforeNo != nowNo)
        {
            //矢印の位置移動
            //指定のコマンドの左に設定
            arrowRect.anchoredPosition = commands[nowNo].GetComponent<RectTransform>().anchoredPosition - Vector2.right * arrowWidth;
        }
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
    public Command AddCommand(string str)
    {
        //コマンドオブジェクトを生成
        var commandObj = Instantiate(commandPrefab, contentRect);
        var command = commandObj.GetComponent<Command>();
        //名前セット
        command.Name = str;

        commands.Add(command);

        //コマンドパネルセット
        command.commandPanel = this;
        UpdateCommands();
        return command;
    }

    public Command RemoveCommand(int no)
    {
        //存在するなら
        if(no < commands.Count)
        {
            var command = commands[no];
            //パネル参照をnullに
            command.commandPanel = null;
            //消した番号が矢印の場所で一番上でないなら
            if (nowNo == commands.Count - 1 && nowNo != 0)
            {
                --nowNo;
            }
            //リストから削除
            commands.RemoveAt(no);
            //ゲームオブジェクトを削除
            Destroy(command.gameObject);
            if(commands.Count == 0)
            {
                CommandManager.Instance.CommandBack();
            }
            UpdateCommands();
        }
        return null;
    }

    //位置の調整更新
    public void UpdateCommands()
    {
        for(int i = 0, len = commands.Count;i < len; i++)
        {
            var command = commands[i];
            //位置調整(Contentを基準に移動)
            var commandRect = command.gameObject.GetComponent<RectTransform>();
            var w = i % col;
            var deltaWidth = (w + 1) * (BLANK + arrowWidth) + w * commandSizeDelta.x;
            var h = i / col;
            var deltaHeight = (h + 1) * BLANK + h * (commandSizeDelta.y + BLANK);

            commandRect.anchoredPosition = new Vector2(deltaWidth, -deltaHeight);

            //幅高さ調整
            commandRect.sizeDelta = commandSizeDelta;

            //最初は
            if (i == 0 && !isOnlyPrint)
            {
                //矢印が生成されていないなら
                if (!arrowRect)
                {
                    //矢印のオブジェクト生成
                    var arrowObj = Instantiate(arrowPrefab, contentRect);
                    arrowRect = arrowObj.GetComponent<RectTransform>();
                }
            }
        }
        //矢印があるなら
        if (arrowRect && commands.Count != 0)
        {
            //現在の番号のコマンドの左に設定
            arrowRect.anchoredPosition = commands[nowNo].GetComponent<RectTransform>().anchoredPosition - Vector2.right * arrowWidth;
        }
    }

    public void Init(Vector2 framePos, List<string> strs, int printRow, int printCol, bool isColScroll, bool isOnlyPrint, int maxStrLength)
    {
        this.printRow = printRow;
        this.printCol = printCol;
        this.isOnlyPrint = isOnlyPrint;

        //表示だけなら
        if (isOnlyPrint)
        {
            //矢印の幅なしにする
            arrowWidth = 0;
        }

        //最大文字数が0なら
        if(maxStrLength == 0)
        {
            //最大文字数を取得する
            foreach (var str in strs)
            {
                maxStrLength = Mathf.Max(maxStrLength, str.Length);
            }
        }

        var commandRect = commandPrefab.GetComponent<RectTransform>();

        //コマンドの幅高さ取得
        commandSizeDelta = new Vector2(commandPrefab.GetComponent<Text>().fontSize * maxStrLength, commandRect.sizeDelta.y);

        //一個動く距離は余白*2とコマンドの高さを足した値
        scrollDistance = BLANK * 2 + commandSizeDelta.y;

        //フレームの大きさ調整
        frameRect.sizeDelta = new Vector2(FRAME_WIDTH * 2 + BLANK * (printCol + 1) + (commandSizeDelta.x + arrowWidth) * printCol, FRAME_WIDTH * 2 + BLANK * (printRow * 2) + commandSizeDelta.y * printRow);

        //バックの位置調整
        backRect.anchoredPosition = new Vector2(1, -1) * FRAME_WIDTH;
        //バックの大きさ調整
        backRect.sizeDelta = frameRect.sizeDelta - 2 * FRAME_WIDTH * Vector2.one;

        //フレームの位置調整
        frameRect.position = framePos;

        foreach (var str in strs)
        {
            //コマンド追加
            AddCommand(str);
        }

        this.isColScroll = isColScroll;
    }

    public Command GetSelectedCommand()
    {
        if (nowNo < 0)
            Debug.LogError("負のnowNo");
        return commands[nowNo];
    }

    public List<Command> GetCommands()
    {
        return commands; 
    }
}
