using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharaController;

public class MoveTask : EventTask
{
    private List<Key> moveKeys = null;
    private CharaController charaController = null;

    public MoveTask(CharaController charaController, List<Key> moveKeys)
    {
        this.charaController = charaController;
        this.moveKeys = moveKeys;
    }

    public override IEnumerator Event()
    {
        foreach(var moveKey in moveKeys)
        {
            charaController.key = moveKey;
            yield return charaController.Move();
        }
    }
}
