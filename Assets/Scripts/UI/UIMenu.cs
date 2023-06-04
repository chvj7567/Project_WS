using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuArg : CHUIArg
{
    
}

public class UIMenu : UIBase
{
    [SerializeField] Button btnExitReady;

    private void Start()
    {
        btnExitReady.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();

            Debug.Log("Exit Ready Ok");
        });
    }
}
