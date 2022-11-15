using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommandManager : SingletonMonoBehaviour<CommandManager>
{
    private CommandPanel commandPanel = null;
    [SerializeField] private GameObject commandPanelPrefab;
    [SerializeField] private RectTransform canvasRect;

    protected override bool dontDestroyOnLoad => true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (commandPanel)
        {
            //矢印を動かす
            commandPanel.MoveArrow();

            //スペースキーを押したら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //コマンドを取得
                var command = commandPanel.GetSelectedCommand();

                //関数を呼ぶ
                command.DoAction();
            }
            //Qキーを押した時
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //一個前のがあるなら
                if (commandPanel.beforeCommandPanel)
                {
                    //一個前を取得
                    var beforeCommandPanel = commandPanel.beforeCommandPanel;
                    //パネルを削除
                    Destroy(commandPanel.gameObject);
                    //戻す
                    commandPanel = beforeCommandPanel;
                }
            }
        }
    }

    public CommandPanel MakeCommandPanel(List<string> strs, int row, int col, Vector2 pos, Command command, bool isOnlyPrint, bool isColScroll)
    {
        CommandPanel commandPanel = null;

        Action action = () =>
        {
            //コマンドパネル生成
            var commandPanelObj = Instantiate(commandPanelPrefab, canvasRect);
            commandPanel = commandPanelObj.GetComponent<CommandPanel>();
            //コマンドパネル初期化
            commandPanel.Init(pos, strs, row, col, isColScroll, isOnlyPrint);
            if (!isOnlyPrint)
            {
                if (command)
                {
                    //コマンドのコマンドパネルを取得(前回の)
                    var parentCommandPanel = command.commandPanel;
                    //前回のをセット
                    commandPanel.beforeCommandPanel = parentCommandPanel;
                }
                //操作するパネルを今のにする
                this.commandPanel = commandPanel;
            }
        };

        //入れるコマンドがあるなら
        if (command)
        {
            command.SetAction(action);
        }
        else
        {
            action();
        }

        return commandPanel;
    }

    //生成したコマンドパネルを全て消去
    public void RemoveAllCommandPanel()
    {
        //RootCommandPanelまで遡る
        while (commandPanel.beforeCommandPanel != null)
        {
            commandPanel = commandPanel.beforeCommandPanel;
        }

        //RootCommandPanelを削除
        Destroy(commandPanel.gameObject);
        //参照をnullに
        commandPanel = null;
    }
}
