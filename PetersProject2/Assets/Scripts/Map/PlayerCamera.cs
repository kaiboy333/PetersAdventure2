using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CharaController charaController = null;
    private GameObject targetObj = null;
    private Vector2 beforeTargetPos;

    public float outViewLine = 0.25f;

    // Start is called before the first frame update

    void Start()
    {
        if (charaController)
        {
            targetObj = charaController.gameObject;
            transform.position = new Vector3(targetObj.transform.position.x, targetObj.transform.position.y, -10);
            beforeTargetPos = targetObj.transform.position;
        }
    }

    void LateUpdate()
    {
        if (targetObj)
        {
            //現在のターゲットの位置を取得
            var targetPos = targetObj.transform.position;

            //ターゲットのカメラによる位置を取得
            var viewPos = Camera.main.WorldToViewportPoint(targetObj.transform.position);

            var deltaViewLineX = viewPos.x - 0.5f;
            var deltaViewLineY = viewPos.y - 0.5f;

            var deltaPos = (Vector2)targetPos - beforeTargetPos;

            //xに置いて外側に出てしまったら
            if (Mathf.Abs(deltaViewLineX) >= outViewLine)
            {
                transform.Translate(Vector3.right * deltaPos.x);
            }
            else if(Mathf.Abs(deltaViewLineY) >= outViewLine)
            {
                transform.Translate(Vector3.up * deltaPos.y);
            }

            beforeTargetPos = targetPos;
        }

    }
}
