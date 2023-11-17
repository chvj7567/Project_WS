
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class UIAlarmArg : CHUIArg
{
    public float closeTime = .5f;
    public int stringID;
    public string text;
    public Action close;
}

public class UIAlarm : UIBase
{
    UIAlarmArg arg;

    [SerializeField] CHTMPro alarmText;

    CancellationTokenSource cts;
    CancellationToken token;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIAlarmArg;
    }

    private async void Start()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;

        backAction += () =>
        {
            cts.Cancel();

            if (arg.close != null)
                arg.close.Invoke();
        };

        if (arg.text != null)
        {
            alarmText.text.text = arg.text;
        }
        else
        {
            alarmText.SetStringID(arg.stringID);
        }

        await Task.Delay((int)(arg.closeTime * 1000));

        if (cts != null && cts.IsCancellationRequested)
            return;

        backAction.Invoke();

        CHMMain.UI.CloseUI(gameObject);
    }
}
