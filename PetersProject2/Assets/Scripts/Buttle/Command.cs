using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Command : MonoBehaviour
{
    //選択されたときに実行する関数
    private Action action = null;
    public Text text = null;
    public string Name
    {
        get { return text.text; }
        set { text.text = value; }
    }

    public CommandPanel commandPanel = null;
    public CommandPanel childPanel = null;

    public RectTransform CommandRect { get { return GetComponent<RectTransform>(); } }

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void DoAction()
    {
        //関数が入っているなら
        if (action != null)
        {
            //関数実行
            action();
        }
    }

    public void SetAction(Action action)
    {
        //関数セット
        this.action = action;
    }
}
