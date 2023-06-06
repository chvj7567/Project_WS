using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;
using static Infomation;

public class SceneStage : SceneBase
{
    [SerializeField] Button btnExitReady;
    [SerializeField] Button btnCreateUnit;
    [SerializeField] Button btnWarStart;
    [SerializeField] List<CreateUnitInfo> liMyUnitInfo = new List<CreateUnitInfo>();
    [SerializeField] List<CHTargetTracker> liEnemyTargetTracker = new List<CHTargetTracker>();

    [SerializeField, ReadOnly] List<CHTargetTracker> liMyTargetTracker = new List<CHTargetTracker>();

    [SerializeField, ReadOnly] List<LayerMask> liMyTargetMask = new List<LayerMask>();
    [SerializeField, ReadOnly] List<LayerMask> liEnemyTargetMask= new List<LayerMask>();

    void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        // �� ���� �ٷ� �������� �ʵ��� ��Ȱ��ȭ
        foreach (var targetTracker in liEnemyTargetTracker)
        {
            liEnemyTargetMask.Add(targetTracker.targetMask);
            targetTracker.targetMask = 0;
        }

        btnExitReady.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();

            Debug.Log("Exit Ready Ok");
        });

        btnCreateUnit.OnClickAsObservable().Subscribe(_ =>
        {
            foreach (var createUnit in liMyUnitInfo)
            {
                CHMMain.Unit.CreateUnit(createUnit.eUnit, createUnit.eTeamLayer, createUnit.eTargetLayer, createUnit.trCreate.position, liMyTargetTracker, liMyTargetMask);
            }
        });

        btnWarStart.OnClickAsObservable().Subscribe(_ =>
        {
            WarStart();
        });
    }

    /*public void CreateUnit(Defines.EUnit _eUnit, Defines.ELayer _eTeamLayer, Defines.ELayer _eTargetLayer, Vector3 _position)
    {
        CHMMain.Resource.InstantiateBall((ball) =>
        {
            CHMMain.Unit.SetUnit(ball, _eUnit);
            CHMMain.Unit.SetColor(ball, _eUnit);
            CHMMain.Unit.SetLayer(ball, _eTeamLayer);
            CHMMain.Unit.SetTargetMask(ball, _eTargetLayer);

            ball.transform.position = _position;

            var targetTracker = ball.GetComponent<CHTargetTracker>();
            if (targetTracker != null)
            {
                // ���� ���� �� �ٷ� �������� �ʵ��� ��Ȱ��ȭ
                targetTracker.enabled = false;
                liMyTargetTracker.Add(targetTracker);
            }
        });
    }*/

    public void WarStart()
    {
        for (int i = 0; i < liMyTargetMask.Count; ++i)
        {
            liMyTargetTracker[i].targetMask = liMyTargetMask[i];
        }

        for (int i = 0; i < liEnemyTargetMask.Count; ++i)
        {
            liEnemyTargetTracker[i].targetMask = liEnemyTargetMask[i];
        }
    }
}
