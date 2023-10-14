using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseScene : SceneBase
{
    public CHUnitBase i;
    private void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject selectedObject = hit.collider.gameObject;

                if (selectedObject != null && selectedObject.name.Contains("Ball"))
                {
                    Debug.Log("Selected object: " + selectedObject.name);

                    var platformTD = selectedObject.GetComponent<PlatformTD_Ball>();
                    if (platformTD == null)
                        return;

                    if (CHMMain.UI.CheckShowUI(Defines.EUI.UITowerMenu) == false)
                    {
                        CHMMain.UI.ShowUI(Defines.EUI.UITowerMenu, new UITowerMenuArg
                        {
                            unit = platformTD.ball,
                        });
                    }
                }
            }
        }
    }
}
