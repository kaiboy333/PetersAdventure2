using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragEvent2 : FragEvent
{
    [SerializeField] private LogManager logManager = null;
    [SerializeField] private ControllManager controllManager = null;

    protected override IEnumerator Event()
    {
        yield return logManager.PrintLog(new List<string>() { "今日は勇者が魔王を倒してから200年。", "今日まで平和に暮らせてきた。", "この幸せを皆と分かち合い、かつての勇者に敬礼を！"});

        yield return new WaitForSeconds(2);

        yield return logManager.PrintLog(new List<string>() { "今日は勇者の子孫として「フィアン」がきておる。", "さあ、こっちに。" });

        yield return new WaitForSeconds(2);

        yield return logManager.PrintLog(new List<string>() { "勇者「フィアン」。", "私は勇者の子孫として恥の無いように", "この街を守ることをこれからも誓います。" });

        yield return logManager.PrintLog(new List<string>() { "うむ。よろしく頼む。", "これにて記念祭の儀式は終わった。", "皆のものは存分に宴を楽しむが良い。" });
    }
}
