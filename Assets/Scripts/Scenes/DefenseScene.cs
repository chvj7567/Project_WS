using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DefenseScene : BaseScene
{
    [SerializeField] List<GameObject> balls = new List<GameObject>();
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

        CHMIAP.Instance.Init();

        CHMGPGS.Instance.Login(async (success, localUser) =>
        {
            if (success)
            {
                Debug.Log($"GPGS Login Success : {localUser.userName}/{localUser.id}");
            }
            else
            {
                Debug.Log($"GPGS Login Failed {success.ToString()}");
            }
        });

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
            goldText.gameObject.SetActive(true);
            lifeText.gameObject.SetActive(true);

            InitStage(stage);
        };

        CHMMain.UI.ShowUI(Defines.EUI.UIStart, new UIStartArg
        {
            stage = curStage,
            spawner = spawner,
            onStage = onStage
        });

        playerData = CHMData.Instance.GetPlayerData(Defines.EData.Player.ToString());

        GameReset();
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

    void GameReset()
    // 게임 정보 리셋
    {
        goldText.gameObject.SetActive(false);
        lifeText.gameObject.SetActive(false);

        spawner.arrivedCount = 0;
        spawner.diedCount = 0;

        gameEnd = false;
    }

    void InitStage(int stage)
    // 스테이지 초기화
    {
        for(int i = 0; i < balls.Count; ++i)
        {
            balls[i].SetActive(false);
        }

        if (playerData != null)
        {
            var stageData = CHMMain.Json.GetStageInfo(stage);
            playerData.gold = stageData.playerGold;
        }

        maxLife = life = (int)CHMMain.Json.GetConstValue(Defines.EConstValue.StageLife);
        lifeText.SetText(maxLife);
    }

    void GameEnd()
    // 게임 종료 시 호출
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
            GameReset();

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
