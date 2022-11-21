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

    //表示中か
    public bool isPrinting { get; private set; }
    private bool isLastPrint = false;

    //private bool isMoving = false;

    //今表示している一番上の列
    private int row = 0;

    //使用するLogEvent
    private LogEvent logEvent = null;

    private const float MOVE_SPEED = 400;

    //自動でスクロールするか
    private bool isAutoScroll = false;

    ////列を表示するのに空ける時間
    //private const float PRINT_STR__NORMAL_INTERVAL = 0.3f;
    //private float printStrInterval = PRINT_STR__NORMAL_INTERVAL;

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
        //if (!text)
        //    return;

        ////スペースキーを押したら
        //if (Input.GetKeyDown(KeyCode.Space) && !isAutoScroll)
        //{
        //    //表示中ではないなら
        //    if (!isPrinting)
        //    {
        //        //最後の表示だったなら
        //        if (isLastPrint)
        //        {
        //            FinishAllPrint();
        //        }
        //        else
        //        {
        //            //列を表示分ずらす
        //            row += PRINT_MAX_ROW;

        //            //スクロールを行う
        //            StartCoroutine(Scroll());

        //            //表示
        //            StartCoroutine(PrintStr(logEvent.logs));
        //        }
        //    }
        //    //表示中なら
        //    else
        //    {
        //        //表示を高速にする
        //        isFastPrint = true;
        //    }
        //}
    }

    public void SetLogEvent(LogEvent logEvent)
    {
        this.logEvent = logEvent;

        //初期化
        ResetLog(false);

        //表示
        StartCoroutine(PrintLog());
    }

    private IEnumerator PrintLog()
    {
        for (int i = 0, len = logEvent.logs.Count; i < len; i++)
        {
            var log = logEvent.logs[i];

            yield return PrintStr(log, false);

            //最後ではなく次ページがあるなら
            if ((i + 1) % PRINT_MAX_ROW == 0 && !(i == len - 1))
            {
                //スペース押すまで待機
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                //スクロールする
                yield return Scroll();
            }
            //最後なら
            else if(i == len - 1)
            {
                //スペース押すまで待機
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                //見えなくする
                FinishAllPrint();

                //LogEventを終わり判定にする
                logEvent.isFinished = true;
            }
        }
    }

    public void ResetLog(bool isButtle)
    {
        //初期化
        row = 0;
        isPrinting = false;
        isLastPrint = false;
        contentRectTransform.anchoredPosition = firstContentPos;
        //バトルならオートスクロール
        isAutoScroll = isButtle;
        //バトルなら表示を速く
        isFastPrint = isButtle;

        //文字列リセット
        text.text = "";

        //見えるように
        gameObject.SetActive(true);
    }

    public IEnumerator PrintStr(string log, bool isLastPrint = false)
    {
        var allLogCount = text.text.Split('\n').Length - 1 + 1;
        this.isLastPrint = isLastPrint;

        ////オートスクロールなら
        //if (isAutoScroll)
        //{
        //    //表示が最後でないなら
        //    if (!isLastPrint)
        //    {
        //        //スクロール
        //        StartCoroutine(Scroll());
        //    }
        //}
        //else
        //{
        //    //速度をリセット
        //    isFastPrint = false;
        //}

        //表示が最後でないなら
        if (row + PRINT_MAX_ROW < allLogCount)
        {
            //スクロール
            yield return Scroll();
        }

        ////指定列から表示列分を足す
        //for (var i = 0; i < printRow; i++)
        //{
        //    printLogs.Add(logs[row + i]);
        //}

        isPrinting = true;
        //一文字ずつ表示
        //foreach (var printLog in printLogs)
        //{
            if (log != null)
            {
                for (int i = 0, len = log.Length; i < len; i++)
                {
                    var c = log[i];

                    text.text += c;

                    if (!isFastPrint)
                    {
                        //ほんの少し待つ
                        yield return new WaitForSeconds(printCharInterval);
                    }
                }

                text.text += "\n";
            }
        //}
        isPrinting = false;
    }

    IEnumerator Scroll()
    {
        //列を表示分ずらす
        row += PRINT_MAX_ROW;

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
    }
}