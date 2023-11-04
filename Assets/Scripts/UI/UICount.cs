
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class UICountArg : CHUIArg
{
    public int count = 3;
}

public class UICount : UIBase
{
    UICountArg arg;

    [SerializeField] CHTMPro countText;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UICountArg;
    }

    private async void Start()
    {
        for (int i = arg.count; i >= 0; --i)
        {
            countText.text.rectTransform.localScale = Vector3.zero;
            countText.text.rectTransform.DOScale(Vector2.one, 1f);
            countText.SetText(i);

            await Task.Delay(1000);
        }
    }
}
