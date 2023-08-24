using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CHGaugeBar : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Image background;
    [SerializeField] Image imgBackGaugeBar;
    [SerializeField] Image imgGaugeBar;

    [SerializeField, ReadOnly] CHUnitBase unitBase;
    [SerializeField, ReadOnly] float originPosYText;

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Init(CHUnitBase _unitBase, float _posY, float _gaugeBarPosY)
    {
        unitBase = _unitBase;
        canvas.worldCamera = Camera.main;
        transform.localPosition = new Vector3(0f, _posY, 0f);
        originPosYText = _posY;
        background.rectTransform.anchoredPosition = new Vector2(background.rectTransform.anchoredPosition.x, _gaugeBarPosY);
        imgBackGaugeBar.rectTransform.anchoredPosition = new Vector2(imgBackGaugeBar.rectTransform.anchoredPosition.x, _gaugeBarPosY);
        imgGaugeBar.rectTransform.anchoredPosition = new Vector2(imgGaugeBar.rectTransform.anchoredPosition.x, _gaugeBarPosY);
    }

    public void SetGaugeBar(float _maxValue, float _curValue, float _damage, float _backGaugeTime, float _gaugeTime, bool viewDamage = true)
    {
        if (imgBackGaugeBar) imgBackGaugeBar.DOFillAmount(_curValue / _maxValue, _backGaugeTime);
        if (imgGaugeBar) imgGaugeBar.DOFillAmount(_curValue / _maxValue, _gaugeTime);

        if (_maxValue > _curValue + _damage)
        {
            if (viewDamage == true && Mathf.Approximately(_damage, 0f) == false)
            {
                ShowDamageText(_damage, .5f);
            }
        }
    }

    public void ResetGaugeBar()
    {
        if (imgBackGaugeBar) imgBackGaugeBar.DOFillAmount(1f, 0.1f);
        if (imgGaugeBar) imgGaugeBar.DOFillAmount(1f, 0.1f);
    }

    void ShowDamageText(float _damage, float _time)
    {
        var copyTextDamage = CHMMain.Resource.Instantiate(CHMMain.Unit.GetOriginDamageText(), transform).GetComponent<CHTMPro>();
        copyTextDamage.gameObject.SetActive(true);
        copyTextDamage.transform.localPosition = new Vector3(copyTextDamage.transform.localPosition.x, copyTextDamage.transform.position.y + originPosYText);
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

        copyTextDamage.text.DOFade(0, _time);

        var rtTextDamage = copyTextDamage.GetComponent<RectTransform>();
        if (rtTextDamage)
        {
            rtTextDamage.DOAnchorPosY(originPosYText + 6f, _time).OnComplete(() =>
            {
                copyTextDamage.text.alpha = 1f;
                rtTextDamage.anchoredPosition = new Vector2(rtTextDamage.anchoredPosition.x, originPosYText);
                CHMMain.Resource.Destroy(copyTextDamage.gameObject);
            });
        }
    }
}
