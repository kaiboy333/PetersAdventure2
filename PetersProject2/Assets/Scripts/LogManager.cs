using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LogManager : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private RectTransform viewRectTransform;
    [SerializeField] private RectTransform contentRectTransform;
    private Vector2 firstContentPos;

    ////Log全部
    //private List<Log> logs;

    //一度に表示できる列
    private const int PRINT_MAX_ROW = 3;
    private float moveScrollVal = 0;
    
    //一文字表示するのに空ける時間
    private float printCharInterval = 0.05f;
    //高速にするか
    private bool isFastPrint = false;

    private bool isPrinting = false;
    private bool isLastPrint = false;

    //private bool isMoving = false;

    //今表示している一番上の列
    private int row = 0;

    //使用するLogEvent
    private LogEvent logEvent = null;

    private const float MOVE_SPEED = 400;

    //自動でスクロールするか
    private bool isAutoScroll = false;

    //列を表示するのに空ける時間(バトルのみ)
    private float printButtleStrInterval = 1;

    // Start is called before the first frame update
    void Start()
    {
        //見えないように
        gameObject.SetActive(false);

        if (text)
        {
            //文字列リセット
            text.text = "";
        }

        if (viewRectTransform)
        {
            //縦幅取得
            moveScrollVal = viewRectTransform.sizeDelta.y;
        }
        if (contentRectTransform)
        {
            //Contentの初期位置記憶
            firstContentPos = contentRectTransform.anchoredPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!text)
            return;

        //スペースキーを押したら
        if (Input.GetKeyDown(KeyCode.Space) && !isAutoScroll)
        {
            //表示中ではないなら
            if (!isPrinting)
            {
                //最後の表示だったなら
                if (isLastPrint)
                {
                    FinishAllPrint();
                }
                else
                {
                    //列を表示分ずらす
                    row += PRINT_MAX_ROW;

                    //スクロールを行う
                    StartCoroutine(Scroll());

                    //表示
                    StartCoroutine(PrintStr());
                }
            }
            //表示中なら
            else
            {
                //表示を高速にする
                //printInterval = FAST_PRINT_INTERVAL;
                isFastPrint = true;
            }
        }
    }

    public void SetLogEvent(LogEvent logEvent)
    {
        this.logEvent = logEvent;

        //初期化
        row = 0;
        isPrinting = false;
        isLastPrint = false;
        contentRectTransform.anchoredPosition = firstContentPos;
        //バトルならオートスクロール
        isAutoScroll = logEvent.isButtle;
        //バトルなら表示を速く
        isFastPrint = logEvent.isButtle;

        //文字列リセット
        text.text = "";

        //見えるように
        gameObject.SetActive(true);

        //表示
        StartCoroutine(PrintStr());
    }

    IEnumerator PrintStr()
    {
        //表示するLogたち
        var printLogs = new List<Log>();
        var printRow = PRINT_MAX_ROW;

        //表示が最後なら
        if (row + PRINT_MAX_ROW >= logEvent.logs.Count)
        {
            //表示列数を調整
            printRow = logEvent.logs.Count - row;
            //最後boolをtrue
            isLastPrint = true;
        }

        //指定列から表示列分を足す
        for(var i = 0; i < printRow; i++)
        {
            printLogs.Add(logEvent.logs[row + i]);
        }

        isPrinting = true;
        //一文字ずつ表示
        foreach(var printLog in printLogs)
        {
            //関数を呼ぶ
            if(printLog.action != null)
            {
                printLog.action();
            }

            if(printLog.str != null)
            {
                for (int i = 0, len = printLog.str.Length; i < len; i++)
                {
                    var c = printLog.str[i];

                    text.text += c;

                    if (!isFastPrint)
                    {
                        //if (i != len - 1)
                        //{
                        //    //少し待つ
                        //    yield return new WaitForSeconds(0.3f);
                        //}
                        //ほんの少し待つ
                        yield return new WaitForSeconds(printCharInterval);
                    }
                }

                text.text += "\n";
            }
        }
        //for (var i = 0; i < str.Length; i++)
        //{
        //    text.text += str[i];
        //    if (!isFastPrint)
        //    {
        //        //改行かつ、最後の改行ではないなら
        //        if (str[i] == '\n')
        //        {
        //            if(i != str.Length - 1)
        //            {
        //                //少し待つ
        //                yield return new WaitForSeconds(0.3f);
        //            }
        //            line++;
        //            if (logActions != null)
        //            {
        //                //line行めに行う関数があるなら
        //                if (logActions.ContainsKey(line))
        //                {
        //                    //関数を呼ぶ
        //                    logActions[line]();
        //                }
        //            }
        //        }
        //        //ほんの少し待つ
        //        yield return new WaitForSeconds(printInterval);
        //    }
        //}
        isPrinting = false;

        //オートスクロールなら
        if (isAutoScroll)
        {
            //表示が最後なら
            if (isLastPrint)
            {
                FinishAllPrint();
            }
            //まだなら
            else
            {
                //スクロール
                StartCoroutine(Scroll());
            }
        }
        else
        {
            //速度をリセット
            isFastPrint = false;
        }
    }

    IEnumerator Scroll()
    {
        var targetPos = contentRectTransform.anchoredPosition + Vector2.up * moveScrollVal;
        while (Vector2.Distance(targetPos, contentRectTransform.anchoredPosition) > Mathf.Epsilon)
        {
            contentRectTransform.anchoredPosition = Vector2.MoveTowards(contentRectTransform.anchoredPosition, targetPos, MOVE_SPEED * Time.deltaTime);
            yield return null;
        }
    }

    //表示が全て終わったときに呼ぶ
    public void FinishAllPrint()
    {
        //文字列リセット
        text.text = "";

        //見えないように
        gameObject.SetActive(false);

        //LogEventを終わり判定にする
        logEvent.isFinished = true;
    }
}

public class Log
{
    public string str { get; private set; }
    public Action action { get; private set; }

    public Log(string str, Action action = null)
    {
        this.str = str;
        this.action = action;
    }
}