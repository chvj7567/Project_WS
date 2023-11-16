
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UIAlarmArg : CHUIArg
{
    public float closeTime = .5f;
    public int stringID;
    public string text;
}

public class UIAlarm : UIBase
{
    UIAlarmArg arg;

    [SerializeField] CHTMPro alarmText;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIAlarmArg;
    }

    private async void Start()
    {
        if (arg.text != null)
        {
            alarmText.text.text = arg.text;
        }
        else
        {
            alarmText.SetStringID(arg.stringID);
        }

        await Task.Delay((int)(arg.closeTime * 1000));

        CHMMain.UI.CloseUI(gameObject);
    }
}
