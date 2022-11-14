using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaManager : EventTask
{
    private Image image = null;

    private float time = 0;
    private readonly int firstAlpha, lastAlpha;

    //掛ける時間
    [SerializeField] private float seconds = 1f;

    public AlphaManager(Image image, bool isBrighter)
    {
        this.image = image;

        firstAlpha = isBrighter ? 1 : 0;
        lastAlpha = firstAlpha ^ 1;
    }

    protected override bool Event()
    {
        time += Time.deltaTime;

        //透明度を求める
        float alpha = Mathf.Lerp(firstAlpha, lastAlpha, time / seconds);

        if (image)
        {
            //透明度を適用
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        //最終的なものになったらtrue
        return alpha == lastAlpha;
    }
}
