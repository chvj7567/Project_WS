using DG.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CHGaugeBar : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] CHTMPro textDamage;
    [SerializeField] Image imgBackGaugeBar;
    [SerializeField] Image imgGaugeBar;

    CancellationTokenSource cts;
    float originPosYText;

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Init(float _posY)
    {
        textDamage.gameObject.SetActive(false);
        textDamage.SetStringID(1);
        canvas.worldCamera = Camera.main;
        transform.localPosition = new Vector3(0f, _posY, 0f);
        originPosYText = _posY;
    }

    public void SetGaugeBar(float _maxValue, float _curValue, float _damage)
    {
        if (imgBackGaugeBar) imgBackGaugeBar.DOFillAmount(_curValue / _maxValue, 1.5f);
        if (imgGaugeBar) imgGaugeBar.DOFillAmount(_curValue / _maxValue, 1f);

        ShowDamageText(_damage);
    }

    public void ResetGaugeBar()
    {
        if (imgBackGaugeBar) imgBackGaugeBar.DOFillAmount(1f, 0.1f);
        if (imgGaugeBar) imgGaugeBar.DOFillAmount(1f, 0.1f);
    }

    void ShowDamageText(float _damage)
    {
        cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        if (textDamage)
        {
            var copyTextDamage = CHMMain.Resource.Instantiate(textDamage.gameObject, transform).GetComponent<CHTMPro>();
            copyTextDamage.gameObject.SetActive(true);
            copyTextDamage.SetText(_damage);

            if (_damage < 0)
            {
                copyTextDamage.SetColor(Color.red);
            }
            else if (_damage > 0)
            {
                copyTextDamage.SetColor(Color.green);
            }
            else
            {
                copyTextDamage.SetColor(Color.gray);
            }

            copyTextDamage.text.DOFade(0, 2f).OnComplete(() =>
            {
                Debug.Log("@@ Fade end");
            });

            var rtTextDamage = copyTextDamage.GetComponent<RectTransform>();
            if (rtTextDamage)
            {
                rtTextDamage.DOAnchorPosY(originPosYText + 10f, 1.5f).OnComplete(() =>
                {
                    copyTextDamage.text.alpha = 1f;
                    rtTextDamage.anchoredPosition = new Vector2(rtTextDamage.anchoredPosition.x, originPosYText);
                    CHMMain.Resource.Destroy(copyTextDamage.gameObject);
                });
            }
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
    }
}
