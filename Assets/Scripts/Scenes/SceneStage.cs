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

    async void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

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

            CHMMain.Unit.CreateUnit(createUnitInfo.eUnit, createUnitInfo.eTeamLayer, createUnitInfo.eTargetLayer, createUnitInfo.createPos, liMyTargetTracker, liMyTargetMask, false);

            --remainCount;
            txtRemainCount.SetText(remainCount);
        });

        btnWarStart.OnClickAsObservable().Subscribe(_ =>
        {
            WarStart();
        });

        stagePlusBtn.OnClickAsObservable().Subscribe(_ =>
        {
            stage = Mathf.Min(++stage, 8);

            CHMMain.Unit.RemoveUnitAll();

            liMyTargetTracker.Clear();
            liEnemyTargetTracker.Clear();
            liMyTargetMask.Clear();
            liEnemyTargetMask.Clear();

            SetStage(stage);
        });

        stageMinusBtn.OnClickAsObservable().Subscribe(_ =>
        {
            stage = Mathf.Max(--stage, 1);

            CHMMain.Unit.RemoveUnitAll();

            liMyTargetTracker.Clear();
            liEnemyTargetTracker.Clear();
            liMyTargetMask.Clear();
            liEnemyTargetMask.Clear();

            SetStage(stage);
        });
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

    public void WarStart()
    {
        spawner.StartSpawn();
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
