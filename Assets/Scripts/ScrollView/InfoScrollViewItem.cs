using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

public class InfoScrollViewItem : MonoBehaviour
{
    [SerializeField] Image unitImg;
    [SerializeField] CHTMPro attackText;
    [SerializeField] CHTMPro coolTimeText;
    [SerializeField] CHTMPro skillDescText;

    public void Init(int index, SkillData info, Sprite sprite)
    {
        if (info == null)
            return;

        unitImg.sprite = sprite;
        attackText.SetText(info.liEffectData.Last().damage);
        coolTimeText.SetText(info.coolTime);
        skillDescText.SetText(info.skillDesc);
        
    }
}
