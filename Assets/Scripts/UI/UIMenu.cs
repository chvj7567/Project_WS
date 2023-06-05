using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static Infomation;

public class UIMenuArg : CHUIArg
{
    public SceneStage scStage;
    public List<CreateUnitInfo> liCreateUnitInfo = new List<CreateUnitInfo>();
}

public class UIMenu : UIBase
{
    UIMenuArg arg;

    [SerializeField] Button btnExitReady;
    [SerializeField] Button btnCreateUnit;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIMenuArg;
    }

    private void Start()
    {
        btnExitReady.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();

            Debug.Log("Exit Ready Ok");
        });

        btnCreateUnit.OnClickAsObservable().Subscribe(_ =>
        {
            foreach (var createUnit in arg.liCreateUnitInfo)
            {
                arg.scStage.CreateUnit(createUnit.eUnit, createUnit.eTeamLayer, createUnit.eTargetLayer, createUnit.trCreate.position);
            }
        });
    }
}
