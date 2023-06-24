using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static Infomation;

public class SceneStage : SceneBase
{
    [Header("���� ����")] 
    [SerializeField] Button exitBtn;
    [SerializeField] Button createUnitBtn;
    [SerializeField] CHTMPro remainCountText;
    [SerializeField] Button warStartBtn;
    [SerializeField] List<Transform> myPositionList = new List<Transform>();
    [SerializeField] List<CHTargetTracker> enemyTargetTrackerList = new List<CHTargetTracker>();

    [Header("�ڵ� ����")]
    [SerializeField, ReadOnly] List<CreateUnitInfo> myCreateUnitInfoList = new List<CreateUnitInfo>();
    [SerializeField, ReadOnly] List<CHTargetTracker> myTargetTrackerList = new List<CHTargetTracker>();
    [SerializeField, ReadOnly] List<LayerMask> myTargetMaskList = new List<LayerMask>();
    [SerializeField, ReadOnly] List<LayerMask> enemyTargetMaskList= new List<LayerMask>();

    int myPositionIndex = 0;
    int remainCount = 0;

    void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        remainCount = myPositionList.Count;
        remainCountText.SetText(remainCount);

        // �� ���� �ٷ� �������� �ʵ��� ��Ȱ��ȭ
        foreach (var targetTracker in enemyTargetTrackerList)
        {
            enemyTargetMaskList.Add(targetTracker.targetMask);
            targetTracker.targetMask = 0;
        }

        exitBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        });

        createUnitBtn.OnClickAsObservable().Subscribe(_ =>
        {
            if (myPositionIndex >= myPositionList.Count) return;

            int randomUnit = Random.Range(0, (int)Defines.EUnit.Max);

            var createUnit = new CreateUnitInfo
            {
                eUnit = (Defines.EUnit)randomUnit,
                trCreate = myPositionList[myPositionIndex++],
                eTeamLayer = Defines.ELayer.Blue,
                eTargetLayer = Defines.ELayer.Red
            };

            CHMMain.Unit.CreateUnit(createUnit.eUnit, createUnit.eTeamLayer, createUnit.eTargetLayer, createUnit.trCreate.position, myTargetTrackerList, myTargetMaskList);

            myCreateUnitInfoList.Add(createUnit);

            --remainCount;
            remainCountText.SetText(remainCount);
        });

        warStartBtn.OnClickAsObservable().Subscribe(_ =>
        {
            WarStart();
        });
    }

    public void WarStart()
    {
        for (int i = 0; i < myTargetMaskList.Count; ++i)
        {
            myTargetTrackerList[i].targetMask = myTargetMaskList[i];
        }

        for (int i = 0; i < enemyTargetMaskList.Count; ++i)
        {
            enemyTargetTrackerList[i].targetMask = enemyTargetMaskList[i];
        }
    }
}
