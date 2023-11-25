using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DefenseScene : SceneBase
{
    [SerializeField] List<GameObject> balls = new List<GameObject>();
    [SerializeField] Button infoBtn;
    [SerializeField] CHTMPro goldText;
    [SerializeField] CHTMPro lifeText;
    [SerializeField] int maxLife;
    [SerializeField] int life;
    [SerializeField] CHSpawner spawner;
    [SerializeField] int killCount;

    public Action<int> onStage;

    bool gameEnd = false;
    Data.Player playerData;

    int curStage = 1;

    private async void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        CHMData.Instance.SaveData("Defense");

        spawner.arrived += () =>
        {
            --life;

            if (life < 0)
            {
                GameEnd();
            }
            else
            {
                lifeText.SetText(life);
            }
        };

        spawner.died += () =>
        {
            ++killCount;
        };

        spawner.end += () =>
        {
            GameEnd();
        };

        onStage += (stage) =>
        {
            infoBtn.gameObject.SetActive(true);
            goldText.gameObject.SetActive(true);
            lifeText.gameObject.SetActive(true);

            SetStage(stage);
        };

        CHMMain.UI.ShowUI(Defines.EUI.UIStart, new UIStartArg
        {
            stage = curStage,
            spawner = spawner,
            onStage = onStage
        });

        infoBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UIInfo, new UIInfoArg());
        });

        playerData = CHMData.Instance.GetPlayerData(Defines.EData.Player.ToString());

        infoBtn.gameObject.SetActive(false);
        goldText.gameObject.SetActive(false);
        lifeText.gameObject.SetActive(false);
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

    void SetStage(int stage)
    {
        for(int i = 0; i < balls.Count; ++i)
        {
            balls[i].SetActive(false);
        }

        if (playerData != null)
        {
            var stageData = CHMMain.Json.GetStageInfo(PlayerPrefs.GetInt(Defines.EPlayerPrefs.Stage.ToString()));
            playerData.gold = stageData.playerGold;
        }

        maxLife = life = (int)CHMMain.Json.GetConstValue(Defines.EConstValue.StageLife);
        lifeText.SetText(maxLife);
    }

    void GameEnd()
    {
        if (gameEnd)
            return;

        gameEnd = true;

        bool clear = false;

        if (life >= 0)
        {
            if (spawner.GetMaxSpawnCount() <= killCount + maxLife - life)
            {
                clear = true;
            }
        }

        var arg = new UIAlarmArg();
        arg.closeTime = 10f;
        arg.close += () =>
        {
            infoBtn.gameObject.SetActive(false);
            goldText.gameObject.SetActive(false);
            lifeText.gameObject.SetActive(false);

            spawner.arrivedCount = 0;
            spawner.diedCount = 0;

            gameEnd = false;

            var stageInfo = CHMMain.Json.GetStageInfo(curStage + 1);
            if (stageInfo != null)
                ++curStage;

            CHMMain.UI.ShowUI(Defines.EUI.UIStart, new UIStartArg
            {
                stage = curStage,
                spawner = spawner,
                onStage = onStage
            });
        };

        if (clear)
        {
            arg.text = "Game Clear";
            CHMMain.UI.ShowUI(Defines.EUI.UIAlarm, arg);
        }
        else
        {
            arg.text = "Game Over";
            CHMMain.UI.ShowUI(Defines.EUI.UIAlarm, arg);
        }
    }
}
