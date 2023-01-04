using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FragEvent : CellEvent
{
    [SerializeField] private Frag frag = null;
    [SerializeField] private FragManager fragManager = null;

    public enum TriggerType
    {
        Normal,
        Collider,
    }
    [SerializeField] private TriggerType triggerType = TriggerType.Normal;

    public static bool isEvent { get; private set; }

    // Start is called before the first frame update
    protected override void Start()
    {
        //持っているフラグがフラグたちの先頭なら
        if(fragManager.eventFrags[0] == frag)
        {
            //初期化
            fragManager.Init();
        }

        base.Start();

        triggerType = TriggerType.Normal;
        var collider2d = GetComponent<Collider2D>();
        //コライダーがあるなら
        if (collider2d)
        {
            //isTriggerなら
            if (collider2d.isTrigger)
            {
                triggerType = TriggerType.Collider;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(frag.GetIsOn() && !frag.GetIsFinished() && triggerType == TriggerType.Normal)
        {
            frag.Off();
            StartCoroutine(StartEvent());
        }
    }

    private IEnumerator StartEvent()
    {
        isEvent = true;

        yield return Event();

        isEvent = false;

        frag.Finish();

        fragManager.NextFragOn();
    }

    protected abstract IEnumerator Event();

    public override IEnumerator CallEvent()
    {
        if (frag.GetIsOn() && !frag.GetIsFinished() && triggerType == TriggerType.Collider)
        {
            frag.Off();
            yield return StartCoroutine(StartEvent());
        }
    }
}
