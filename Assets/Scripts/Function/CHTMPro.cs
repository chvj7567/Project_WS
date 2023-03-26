using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Text))]
public class CHTMPro : MonoBehaviour
{
    public int stringID = -1;
    public TMP_Text text;
    [ReadOnly]
    public object[] argArr;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        if (text)
        {
            if (stringID != -1)
            {
                text.text = CHMMain.String.GetString(stringID);
            }
        }
    }

    public void SetText(params object[] _arrArg)
    {
        this.argArr = _arrArg;
        if (text)
        {
            text.text = string.Format(CHMMain.String.GetString(stringID), _arrArg);
        }
    }

    public void SetStringID(int _stringID)
    {
        this.argArr = null;
        stringID = _stringID;
        if (text)
        {
            text.text = CHMMain.String.GetString(stringID);
        }
    }

    public void SetPlusString(string _plusString)
    {
        if (text && string.IsNullOrEmpty(_plusString) == false)
        {
            text.text = text.text + " + " + _plusString;
        }
    }
}
