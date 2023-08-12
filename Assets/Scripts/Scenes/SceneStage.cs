using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SceneStage : SceneBase
{
    [Header("수동 설정")] 
    [SerializeField] Button btnExit;
    [SerializeField] Button btnCreateUnit;
    [SerializeField] CHTMPro txtRemainCount;
    [SerializeField] Button btnWarStart;
    [SerializeField] CHTMPro stageText;
    [SerializeField] CHTMPro resultText;
    [SerializeField] Button stagePlusBtn;
    [SerializeField] Button stageMinusBtn;
    [SerializeField] CHSpawner spawner;

    [Header("자동 설정")]
    [SerializeField, ReadOnly] List<Vector3> liMyPosition = new List<Vector3>();
    [SerializeField, ReadOnly] List<Defines.EUnit> liMyUnit = new List<Defines.EUnit>();
    [SerializeField, ReadOnly] List<Vector3> liEnemyPosition = new List<Vector3>();
    [SerializeField, ReadOnly] List<CHTargetTracker> liMyTargetTracker = new List<CHTargetTracker>();
    [SerializeField, ReadOnly] List<CHTargetTracker> liEnemyTargetTracker = new List<CHTargetTracker>();
    [SerializeField, ReadOnly] List<LayerMask> liMyTargetMask = new List<LayerMask>();
    [SerializeField, ReadOnly] List<LayerMask> liEnemyTargetMask= new List<LayerMask>();

    [SerializeField, ReadOnly] int myIndex = 0;
    [SerializeField, ReadOnly] int remainCount = 0;
    [SerializeField, ReadOnly] int stage = 1;
    [SerializeField, ReadOnly] bool warEnd = false;

    async void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        warEnd = true;
        resultText.gameObject.SetActive(false);
        SetStage(stage);

        btnExit.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        });
        btnCreateUnit.OnClickAsObservable().Subscribe(_ =>
        {
            if (myIndex >= liMyPosition.Count) return;

            Infomation.CreateUnitInfo createUnitInfo;

            if (liMyUnit[myIndex] == Defines.EUnit.None)
            {
                int randomUnit = UnityEngine.Random.Range(0, (int)Defines.EUnit.Max);

                createUnitInfo = new Infomation.CreateUnitInfo
                {
                    eUnit = (Defines.EUnit)randomUnit,
                    createPos = liMyPosition[myIndex],
                    eTeamLayer = Defines.ELayer.Blue,
                    eTargetLayer = Defines.ELayer.Red
                };
            }
            else
            {
                createUnitInfo = new Infomation.CreateUnitInfo
                {
                    eUnit = liMyUnit[myIndex],
                    createPos = liMyPosition[myIndex],
                    eTeamLayer = Defines.ELayer.Blue,
                    eTargetLayer = Defines.ELayer.Red
                };
            }

            ++myIndex;

            CHMMain.Unit.CreateUnit(createUnitInfo.eUnit, createUnitInfo.eTeamLayer, createUnitInfo.eTargetLayer, createUnitInfo.createPos, liMyTargetTracker, liMyTargetMask);

            --remainCount;
            txtRemainCount.SetText(remainCount);
        });

        btnWarStart.OnClickAsObservable().Subscribe(_ =>
        {
            WarStart();
        });

        stagePlusBtn.OnClickAsObservable().Subscribe(_ =>
        {
            if (warEnd == true)
            {
                stage = Mathf.Min(++stage, 9);

                CHMMain.Unit.RemoveUnitAll();

                liMyTargetTracker.Clear();
                liEnemyTargetTracker.Clear();
                liMyTargetMask.Clear();
                liEnemyTargetMask.Clear();

                SetStage(stage);
            }
        });

        stageMinusBtn.OnClickAsObservable().Subscribe(_ =>
        {
            if (warEnd == true)
            {
                stage = Mathf.Max(--stage, 1);

                CHMMain.Unit.RemoveUnitAll();

                liMyTargetTracker.Clear();
                liEnemyTargetTracker.Clear();
                liMyTargetMask.Clear();
                liEnemyTargetMask.Clear();

                SetStage(stage);
            }
        });

        await Task.Delay(1000);

        spawner.StartSpawn();
    }

    void SetStage(int _stage)
    {
        if (stageText)
            stageText.SetText(_stage);

        myIndex = 0;
        liMyPosition.Clear();
        liMyUnit.Clear();
        liEnemyPosition.Clear();

        liMyPosition = CHMMain.Json.GetPositionListFromStageInfo(_stage, 1);
        liMyUnit = CHMMain.Json.GetUnitListFromStageInfo(_stage, 1);
        remainCount = liMyPosition.Count;
        txtRemainCount.SetText(remainCount);

        liEnemyPosition = CHMMain.Json.GetPositionListFromStageInfo(_stage, 2);

        var positionInfoList = CHMMain.Json.GetStageInfoList(_stage, 2);

        foreach (var posInfo in positionInfoList)
        {
            Infomation.CreateUnitInfo createUnitInfo;
            if (posInfo.eUnit == Defines.EUnit.None)
            {
                int randomUnit = UnityEngine.Random.Range(0, (int)Defines.EUnit.Max);

                createUnitInfo = new Infomation.CreateUnitInfo
                {
                    eUnit = (Defines.EUnit)randomUnit,
                    createPos = CHMMain.Json.GetPositionFromStageInfo(posInfo),
                    eTeamLayer = Defines.ELayer.Red,
                    eTargetLayer = Defines.ELayer.Blue
                };
            }
            else
            {
                createUnitInfo = new Infomation.CreateUnitInfo
                {
                    eUnit = posInfo.eUnit,
                    createPos = CHMMain.Json.GetPositionFromStageInfo(posInfo),
                    eTeamLayer = Defines.ELayer.Red,
                    eTargetLayer = Defines.ELayer.Blue
                };
            }

            CHMMain.Unit.CreateUnit(createUnitInfo.eUnit, createUnitInfo.eTeamLayer, createUnitInfo.eTargetLayer, createUnitInfo.createPos, liEnemyTargetTracker, liEnemyTargetMask);
        }
    }

    public async void WarStart()
    {
        warEnd = false;

        for (int i = 0; i < liMyTargetMask.Count; ++i)
        {
            liMyTargetTracker[i].targetMask = liMyTargetMask[i];
        }

        for (int i = 0; i < liEnemyTargetMask.Count; ++i)
        {
            liEnemyTargetTracker[i].targetMask = liEnemyTargetMask[i];
        }

        do
        {
            await Task.Delay(1000);
            warEnd = CheckGameEnd();
        } while (warEnd == false);

        bool alive = false;
        foreach (var targetTracker in liMyTargetTracker)
        {
            if (targetTracker != null)
            {
                var unitBase = targetTracker.GetComponent<CHUnitBase>();
                if (unitBase != null && unitBase.GetCurrentHp() > 0)
                {
                    alive = true;
                    break;
                }
            }
        }

        CHMMain.Unit.RemoveUnitAll();

        liMyTargetTracker.Clear();
        liEnemyTargetTracker.Clear();
        liMyTargetMask.Clear();
        liEnemyTargetMask.Clear();

        Debug.Log("Game End");

        resultText.gameObject.SetActive(true);

        if (alive)
        {
            ++stage;
            resultText.SetStringID(2);
        }
        else
        {
            resultText.SetStringID(3);
        }

        await Task.Delay(3000);

        resultText.gameObject.SetActive(false);

        if (Application.isPlaying == false) return;

        SetStage(stage);
    }

    public bool CheckGameEnd()
    {
        bool gameEnd = true;

        foreach (var targetTracker in liMyTargetTracker)
        {
            if (targetTracker != null)
            {
                var targetInfo = targetTracker.GetClosestTargetInfo();
                if (targetInfo.objTarget != null)
                {
                    if (targetInfo.objTarget.activeSelf == false)
                    {
                        continue;
                    }
                    gameEnd = false;
                    break;
                }
            }
        }

        foreach (var targetTracker in liEnemyTargetTracker)
        {
            if (targetTracker != null)
            {
                var targetInfo = targetTracker.GetClosestTargetInfo();
                if (targetInfo.objTarget != null)
                {
                    if (targetInfo.objTarget.activeSelf == false)
                    {
                        continue;
                    }
                    gameEnd = false;
                    break;
                }
            }
        }

        return gameEnd;
    }
}
