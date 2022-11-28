using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCommandController : MonoBehaviour
{
    [SerializeField] private CommandPanel commandPanelRoot = null;
    [SerializeField] private RectTransform parentRect = null;
    [SerializeField] private RectTransform commandPanelfirstRect = null;
    private YushaController yushaController = null;

    // Start is called before the first frame update
    void Start()
    {
        yushaController = FindObjectOfType<YushaController>();

        commandPanelRoot = CommandManager.Instance.MakeCommandPanel(new List<string> { "はなす", "じゅもん", "どうぐ", "しらべる", "つよさ" }, 4, 1, commandPanelfirstRect.position, null, false, true, parentRect);
        commandPanelRoot.gameObject.SetActive(false);
        var commandsRoot = commandPanelRoot.GetCommands();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            //プレイヤーが動いていないときは
            if (!yushaController.isMoving)
            {
                //見えるようにする
                commandPanelRoot.gameObject.SetActive(true);
                //動けないように
                CharaController.canMove = false;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            //今見ているのが一番最初のパネルなら
            if (CommandManager.Instance.nowCommandPanel == commandPanelRoot)
            {
                //見えなくする
                commandPanelRoot.gameObject.SetActive(false);
                //動けるように
                CharaController.canMove = true;
            }
        }
    }
}
