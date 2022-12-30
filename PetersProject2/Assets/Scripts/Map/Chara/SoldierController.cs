using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : CharaController
{
    //動くまでの時間
    public float waitInterval = 3f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        StartCoroutine(DesideDirection());
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void ArriveTargetPos()
    {
        StartCoroutine(DesideDirection());
    }

    IEnumerator DesideDirection()
    {
        //待つ
        yield return new WaitForSeconds(waitInterval);

        //歩けるまで進む方向を決める
        do
        {
            yield return null;
            var keyIndex = Random.Range(0, System.Enum.GetValues(typeof(Key)).Length - 1);
            key = (Key)System.Enum.ToObject(typeof(Key), keyIndex);
        }
        while (!CanWalk(GetNextTargetPos(directions[(int)key])));

        //動く
        Move();
    }
}
