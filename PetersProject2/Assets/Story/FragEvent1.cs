using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragEvent1 : FragEvent
{
    [SerializeField] private LogManager logManager = null;
    [SerializeField] private ControllManager controllManager = null;

    protected override IEnumerator Event()
    {
        controllManager.AllTransform(transform.position);

        yield return logManager.PrintLog(new List<string>() { "おはよう、ピーター。", "よく眠れた？", "", "今日は勇者記念日でしょ。", "遅れないように支度しなさい。"});
    }
}
