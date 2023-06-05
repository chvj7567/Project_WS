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

        foreach (var tr in liTransform)
        {
            CreateUnit(Defines.EUnit.Red, Defines.ELayer.Blue, Defines.ELayer.Red, tr.position);
        }
    }

    public void CreateUnit(Defines.EUnit _eUnit, Defines.ELayer _eTeamLayer, Defines.ELayer _eTargetLayer, Vector3 _position)
    {
        CHMMain.Resource.InstantiateBall((ball) =>
        {
            CHMMain.Unit.SetUnit(ball, _eUnit);
            CHMMain.Unit.SetColor(ball, _eUnit);
            CHMMain.Unit.SetLayer(ball, _eTeamLayer);
            CHMMain.Unit.SetTargetMask(ball, _eTargetLayer);

            ball.transform.position = _position;
        });
    }
}
