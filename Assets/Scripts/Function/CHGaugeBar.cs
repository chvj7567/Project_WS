using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CHGaugeBar : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Image imgBackGaugeBar;
    [SerializeField] Image imgGaugeBar;

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void Init(float _posY)
    {
        canvas.worldCamera = Camera.main;
        transform.localPosition = new Vector3(0f, _posY, 0f);
    }

    public void SetGaugeBar(float _maxValue, float _curValue)
    {
        if (imgBackGaugeBar) imgBackGaugeBar.DOFillAmount(_curValue / _maxValue, 1.5f);
        if (imgGaugeBar) imgGaugeBar.DOFillAmount(_curValue / _maxValue, 1f);
    }

    public void ResetGaugeBar()
    {
        if (imgBackGaugeBar) imgBackGaugeBar.DOFillAmount(1f, 0.1f);
        if (imgGaugeBar) imgGaugeBar.DOFillAmount(1f, 0.1f);
    }
}
