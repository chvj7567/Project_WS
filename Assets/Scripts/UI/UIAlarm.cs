using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAlarmArg : UIArg
{
    public int descStringID;
}

public class UIAlarm : UIBase
{
    UIAlarmArg arg;

    public CHTMPro textDesc;

    public override void InitUI(UIArg _uiArg)
    {
        arg = _uiArg as UIAlarmArg;
    }

    private void Start()
    {
        textDesc.SetText("AA");
    }
}
