using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] List<Transform> liTransform = new List<Transform>();

    void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);
        CHMMain.UI.ShowUI(Defines.EUI.UIMenu, new UIMenuArg());

        CreateUnit(Defines.EUnit.Pink);
    }

    public void CreateUnit(Defines.EUnit _eUnit)
    {
        CHMMain.Resource.InstantiateBall((ball) =>
        {
            CHMMain.Unit.SetColor(ball, _eUnit);
        });
    }
}
