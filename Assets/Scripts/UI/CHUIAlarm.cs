using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CHUIAlarmArg : CHUIArg
{
    public int descStringID;
}

public class CHUIAlarm : CHUIBase
{
    CHUIAlarmArg arg;

    public CHTMPro textDesc;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as CHUIAlarmArg;
    }

    private void Start()
    {
        textDesc.text.text = "AA";
    }
}
