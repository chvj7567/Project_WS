using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static Infomation;

public class SceneStage : SceneBase
{
    [Header("수동 설정")] 
    [SerializeField] Button btnExit;
    [SerializeField] Button btnCreateUnit;
    [SerializeField] CHTMPro txtRemainCount;
    [SerializeField] Button btnWarStart;
    
    [Header("자동 설정")]
    [SerializeField, ReadOnly] List<Vector3> liMyPosition = new List<Vector3>();
    [SerializeField, ReadOnly] List<Vector3> liEnemyPosition = new List<Vector3>();
    [SerializeField, ReadOnly] List<CHTargetTracker> liMyTargetTracker = new List<CHTargetTracker>();
    [SerializeField, ReadOnly] List<CHTargetTracker> liEnemyTargetTracker = new List<CHTargetTracker>();
    [SerializeField, ReadOnly] List<LayerMask> liMyTargetMask = new List<LayerMask>();
    [SerializeField, ReadOnly] List<LayerMask> liEnemyTargetMask= new List<LayerMask>();

    int myPositionIndex = 0;
    int remainCount = 0;
    int stage = 1;

    void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

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
            if (myPositionIndex >= liMyPosition.Count) return;

            int randomUnit = Random.Range(0, (int)Defines.EUnit.Max);

            var createUnitInfo = new Infomation.CreateUnitInfo
            {
                eUnit = (Defines.EUnit)randomUnit,
                createPos = liMyPosition[myPositionIndex++],
                eTeamLayer = Defines.ELayer.Blue,
                eTargetLayer = Defines.ELayer.Red
            };

            CHMMain.Unit.CreateUnit(createUnitInfo.eUnit, createUnitInfo.eTeamLayer, createUnitInfo.eTargetLayer, createUnitInfo.createPos, liMyTargetTracker, liMyTargetMask);

            --remainCount;
            txtRemainCount.SetText(remainCount);
        });

        btnWarStart.OnClickAsObservable().Subscribe(_ =>
        {
            WarStart();
        });
    }

    void SetStage(int _stage)
    {
        myPositionIndex = 0;
        liMyPosition.Clear();
        liEnemyPosition.Clear();

        liMyPosition.AddRange(CHMMain.Json.GetPositionListFromStageInfo(_stage, 1));
        remainCount = liMyPosition.Count;
        txtRemainCount.SetText(remainCount);

        liEnemyPosition.AddRange(CHMMain.Json.GetPositionListFromStageInfo(_stage, 2));

        var positionInfoList = CHMMain.Json.GetStageInfoList(_stage, 2);

        foreach (var posInfo in positionInfoList)
        {
            Infomation.CreateUnitInfo createUnitInfo;
            if (posInfo.eUnit == Defines.EUnit.None)
            {
                int randomUnit = Random.Range(0, (int)Defines.EUnit.Max);

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
        for (int i = 0; i < liMyTargetMask.Count; ++i)
        {
            liMyTargetTracker[i].targetMask = liMyTargetMask[i];
        }

        for (int i = 0; i < liEnemyTargetMask.Count; ++i)
        {
            liEnemyTargetTracker[i].targetMask = liEnemyTargetMask[i];
        }

        bool gameEnd = false;
        do
        {
            await Task.Delay(1000);
            gameEnd = CheckGameEnd();
        } while (gameEnd == false);

        CHMMain.Unit.RemoveUnitAll();
        Debug.Log("Game End");

        await Task.Delay(3000);

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
