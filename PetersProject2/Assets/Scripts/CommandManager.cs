using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommandManager : SingletonMonoBehaviour<CommandManager>
{
    private CommandPanel nowCommandPanel = null;
    [SerializeField] private GameObject commandPanelPrefab;

    protected override bool dontDestroyOnLoad => true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nowCommandPanel)
        {
            //矢印を動かす
            nowCommandPanel.MoveArrow();

            //スペースキーを押したら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //コマンドを取得
                var command = nowCommandPanel.GetSelectedCommand();

                //関数を呼ぶ
                command.DoAction();
            }
            //Qキーを押した時
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //一個前のがあるなら
                if (nowCommandPanel.beforeCommandPanel)
                {
                    //一個前を取得
                    var beforeCommandPanel = nowCommandPanel.beforeCommandPanel;
                    //パネルを見えなくする
                    nowCommandPanel.gameObject.SetActive(false);
                    //戻す
                    nowCommandPanel = beforeCommandPanel;
                }
            }
        }
    }

    public CommandPanel MakeCommandPanel(List<string> strs, int row, int col, Vector2 pos, Command command, bool isOnlyPrint, bool isColScroll, RectTransform parentRect, int maxStrLen = 0)
    {
        CommandPanel commandPanel = null;

        //コマンドパネル生成
        var commandPanelObj = Instantiate(commandPanelPrefab, parentRect);
        commandPanel = commandPanelObj.GetComponent<CommandPanel>();
        //コマンドパネル初期化
        commandPanel.Init(pos, strs, row, col, isColScroll, isOnlyPrint, maxStrLen);
        if (!isOnlyPrint)
        {
            if (command)
            {
                //コマンドのコマンドパネルを取得(前回の)
                var parentCommandPanel = command.commandPanel;
                //前回のをセット
                commandPanel.beforeCommandPanel = parentCommandPanel;

                //コマンドの子パネルを作ったものにする
                command.childPanel = commandPanel;
            }
        }
        //見えないようにする
        commandPanel.gameObject.SetActive(false);

        //入れるコマンドがあるなら
        if (command)
        {
            //親コマンドの名前をつけてわかりやすくする
            commandPanel.gameObject.name = commandPanel.gameObject.name + command.Name;

            command.SetAction(() =>
            {
                //見えるようにする
                commandPanel.gameObject.SetActive(true);
                //操作するパネルを今のにする
                this.nowCommandPanel = commandPanel;
            });
        }
        else
        {
            //見えるようにする
            commandPanel.gameObject.SetActive(true);
            //操作するパネルを今のにする
            this.nowCommandPanel = commandPanel;
        }

        return commandPanel;
    }

    //バトルコマンドパネルを全て消去
    public void RemoveAllButtleCommandPanel()
    {
        //RootCommandPanelまで遡る
        while (nowCommandPanel.beforeCommandPanel != null)
        {
            nowCommandPanel = nowCommandPanel.beforeCommandPanel;
        }

        RemoveCommandPanel(nowCommandPanel);
    }

    private void RemoveCommandPanel(CommandPanel commandPanel)
    {
        if (!commandPanel)
            return;

        foreach(var command in commandPanel.GetCommands())
        {
            //子のパネルを消す
            RemoveCommandPanel(command.childPanel);
        }

        //自信を消す
        Destroy(commandPanel.gameObject);
    }
}
