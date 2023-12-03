using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIWarSimulatorSelectArg : CHUIArg
{

}

public class UIWarSimulatorSelect : UIBase
{
    UIWarSimulatorSelectArg arg;

    [SerializeField] Button btn1vs1;
    [SerializeField] Button btn3vs3;
    [SerializeField] Button btn10vs10;
    [SerializeField] Button btn30vs30;
    [SerializeField] Button btn100vs100;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIWarSimulatorSelectArg;
    }

    private void Start()
    {
        btn1vs1.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerPrefs.SetInt("WarSimulator", 1);
            SceneManager.LoadScene(2);
        });

        btn3vs3.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerPrefs.SetInt("WarSimulator", 3);
            SceneManager.LoadScene(2);
        });

        btn10vs10.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerPrefs.SetInt("WarSimulator", 10);
            SceneManager.LoadScene(2);
        });

        btn30vs30.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerPrefs.SetInt("WarSimulator", 30);
            SceneManager.LoadScene(2);
        });

        btn100vs100.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerPrefs.SetInt("WarSimulator", 100);
            SceneManager.LoadScene(2);
        });
    }
}
