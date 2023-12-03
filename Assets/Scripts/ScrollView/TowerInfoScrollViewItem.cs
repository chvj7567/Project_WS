using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using System;

public class TowerInfoScrollViewItem : MonoBehaviour
{
    [SerializeField] Image unitImg;
    [SerializeField] CHTMPro damageText;
    [SerializeField] CHTMPro coolTimeText;
    [SerializeField] CHTMPro skillDescText;

    [SerializeField] CHTMPro plusDamageText;
    [SerializeField] CHTMPro minusCoolTimeText;
    [SerializeField] Button damageUpBtn;
    [SerializeField] Button coolTimeDownBtn;

    public void Init(int index, SkillData info, Sprite sprite)
    {
        if (info == null)
            return;

        unitImg.sprite = sprite;

        float damage = 0f;
        for (int i = 0; i < info.liEffectData.Count; ++i)
        {
            damage += info.liEffectData[i].damage;
        }

        damageText.SetText(damage);
        coolTimeText.SetText(info.coolTime);
        skillDescText.SetText(info.skillDesc);

        var unitData = CHMData.Instance.GetUnitData((Defines.EUnit)index);
        if (unitData != null)
        {
            if (unitData.plusDamage != 0) plusDamageText.gameObject.SetActive(true);
            else plusDamageText.gameObject.SetActive(false);

            if (unitData.minusCoolTime != 0) minusCoolTimeText.gameObject.SetActive(true);
            else minusCoolTimeText.gameObject.SetActive(false);

            plusDamageText.SetText(unitData.plusDamage);
            minusCoolTimeText.SetText(unitData.minusCoolTime);
        }

        damageUpBtn.OnClickAsObservable().Subscribe(_ =>
        {
            var unitData = CHMData.Instance.GetUnitData((Defines.EUnit)index);
            if (unitData != null)
            {
                unitData.plusDamage += 1;
                plusDamageText.SetText(unitData.plusDamage);
                plusDamageText.gameObject.SetActive(true);
            }

            CHMData.Instance.SaveData("Defense");
        });

        coolTimeDownBtn.OnClickAsObservable().Subscribe(_ =>
        {
            var unitData = CHMData.Instance.GetUnitData((Defines.EUnit)index);
            if (unitData != null)
            {
                unitData.minusCoolTime -= 0.1f;
                unitData.minusCoolTime = (float)Math.Round((double)unitData.minusCoolTime, 2);
                minusCoolTimeText.SetText(unitData.minusCoolTime);
                minusCoolTimeText.gameObject.SetActive(true);
            }

            CHMData.Instance.SaveData("Defense");
        });
    }
}
