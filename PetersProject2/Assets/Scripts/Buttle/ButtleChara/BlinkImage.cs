using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    [SerializeField] private float blinkSecPer = 0.05f;
    [SerializeField] private float addColorValue = 0.2f;
    private Image image = null;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator BlinkEnemyImage(float blinkSec, bool isAlpha)
    {
        var originalColor = image.color;
        var beforeTime = Time.time;
        var time = Time.time;
        int a = 1;
        while (time - beforeTime < blinkSec)
        {
            yield return new WaitForSeconds(isAlpha ? blinkSecPer : blinkSecPer * 2);
            time = Time.time;
            if (isAlpha) {
                image.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            }
            else
            {
                image.color = originalColor - new Color(1, 1, 1, 0) * a * addColorValue;
            }
            a ^= 1;
        }
        //元に戻す
        image.color = originalColor;
    }

    //public void Blink(bool isAlpha)
    //{
    //    StartCoroutine(BlinkEnemyImage(isAlpha));
    //}
}
