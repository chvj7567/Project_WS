using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIStageArg : CHUIArg
{

}

public class UIStage : UIBase
{
    [SerializeField] Button btnUnitSelect;

    private void Start()
    {
        btnUnitSelect.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();

            Debug.Log("Exit Ready Ok");
        });
    }
}
