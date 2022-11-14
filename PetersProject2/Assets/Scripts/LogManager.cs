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
    //str全部
    private string[] strs;

    //一度に表示できる列
    private const int PRINT_MAX_ROW = 3;
    private float moveScrollVal = 0;
    
    //一文字表示するのに空ける時間
    private float printInterval = 0.05f;
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

    //指定行目で行う関数たち
    private Dictionary<int, Action> logActions = null;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //表示中ではないなら
            if (!isPrinting)
            {
                //最後の表示だったなら
                if (isLastPrint)
                {
                    //文字列リセット
                    text.text = "";

                    //見えないように
                    gameObject.SetActive(false);

                    //LogEventを終わり判定にする
                    logEvent.isFinished = true;
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

        //改行で分ける
        strs = logEvent.Str.Split('\n');

        //文字列リセット
        text.text = "";

        //見えるように
        gameObject.SetActive(true);

        //表示
        StartCoroutine(PrintStr());
    }

    IEnumerator PrintStr()
    {
        var str = "";
        var printRow = PRINT_MAX_ROW;

        //表示が最後なら
        if (row + PRINT_MAX_ROW >= strs.Length)
        {
            //表示列数を調整
            printRow = strs.Length - row;
            //最後boolをtrue
            isLastPrint = true;
        }

        //指定列から表示列分を足す
        for(var i = 0; i < printRow; i++)
        {
            str += strs[row + i] + '\n';
        }

        isPrinting = true;
        //一文字ずつ表示
        var line = 0;
        for(var i = 0; i < str.Length; i++)
        {
            text.text += str[i];
            if (!isFastPrint)
            {
                //改行かつ、最後の改行ではないなら
                if (str[i] == '\n')
                {
                    if(i != str.Length - 1)
                    {
                        //少し待つ
                        yield return new WaitForSeconds(0.3f);
                    }
                    line++;
                    if (logActions != null)
                    {
                        //line行めに行う関数があるなら
                        if (logActions.ContainsKey(line))
                        {
                            //関数を呼ぶ
                            logActions[line]();
                        }
                    }
                }
                //ほんの少し待つ
                yield return new WaitForSeconds(printInterval);
            }
        }
        isPrinting = false;

        //速度を通常に
        isFastPrint = false;
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
}
