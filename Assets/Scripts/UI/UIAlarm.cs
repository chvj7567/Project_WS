using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIAlarmArg : CHUIArg
{
    public float closeTime = 2f;
    public string alarmText;
}

public class UIAlarm : UIBase
{
    UIAlarmArg arg;

    [SerializeField] CHTMPro alarmText;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIAlarmArg;
    }

    CancellationTokenSource delayTokenSource;

    private async void Start()
    {

    }
}
