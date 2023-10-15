using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenuItem : MonoBehaviour
{
    [SerializeField] public Defines.EUnit _eUnit;
    [SerializeField] public Button button;
    [SerializeField] public CHTMPro goldText;

    public Defines.EShop GetShopEnum()
    {
        switch (_eUnit)
        {
            case Defines.EUnit.White:
                return Defines.EShop.WhiteUnit;
            case Defines.EUnit.Brown:
                return Defines.EShop.BrownUnit;
            case Defines.EUnit.Orange:
                return Defines.EShop.OrangeUnit;
            case Defines.EUnit.Yellow:
                return Defines.EShop.YellowUnit;
            case Defines.EUnit.Green:
                return Defines.EShop.GreenUnit;
            case Defines.EUnit.Blue:
                return Defines.EShop.BlueUnit;
            case Defines.EUnit.Pink:
                return Defines.EShop.PinkUnit;
            case Defines.EUnit.Red:
                return Defines.EShop.RedUnit;
        }

        return Defines.EShop.None;
    }
}
