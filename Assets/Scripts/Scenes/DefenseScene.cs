using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DefenseScene : SceneBase
{
    [SerializeField] Button infoBtn;
    [SerializeField] CHTMPro goldText;

    Data.Player playerData;

    private async void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        infoBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UIInfo, new UIInfoArg());
        });

        playerData = CHMData.Instance.GetPlayerData(Defines.EData.Player.ToString());

        // 스테이지 1로 가정
        PlayerPrefs.SetInt(Defines.EPlayerPrefs.Stage.ToString(), 1);

        if (playerData != null)
        {
            var stageData = CHMMain.Json.GetStageInfo(PlayerPrefs.GetInt(Defines.EPlayerPrefs.Stage.ToString()));
            playerData.gold += stageData.playerGold;
        }
    }

    void Update()
    {
        if (playerData != null)
        {
            goldText.SetText(playerData.gold);
        }

        if (Input.GetMouseButtonDown(0) && CHMMain.UI.CheckShowUI() == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1.0f);

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
