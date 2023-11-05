
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UICountArg : CHUIArg
{
    public CHSpawner spawner;
    public float delay = 1;
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
        countText.gameObject.SetActive(false);
        await Task.Delay((int)(arg.delay * 1000));

        countText.gameObject.SetActive(true);
        for (int i = arg.count; i > 0; --i)
        {
            countText.SetText(i);
            countText.text.rectTransform.localScale = Vector3.zero;
            countText.text.rectTransform.DOScale(Vector2.one, 1f);

            await Task.Delay(1000);
        }

        countText.SetText("Start");
        await Task.Delay(500);

        arg.spawner.StartSpawn();
        CHMMain.UI.CloseUI(gameObject);
    }
}
