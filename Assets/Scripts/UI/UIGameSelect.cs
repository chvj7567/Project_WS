using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;

public class UIGameSelectArg : CHUIArg
{
    public int stage;
    public Action<int> onStage;
    public CHSpawner spawner;
    public Action onClose;
}

public class UIGameSelect : UIBase
{
    UIGameSelectArg arg;

    [SerializeField] Button defenseBtn;
    [SerializeField] Button warSimulatorBtn;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIGameSelectArg;
    }

    private void Start()
    {
        PlayerPrefs.SetInt(Defines.EPlayerPrefs.Stage.ToString(), arg.stage);

        defenseBtn.OnClickAsObservable().Subscribe(_ =>
        {
            /*if (arg.onStage != null)
                arg.onStage.Invoke(PlayerPrefs.GetInt(Defines.EPlayerPrefs.Stage.ToString()));

            CHMMain.UI.ShowUI(Defines.EUI.UICount, new UICountArg
            {
                spawner = arg.spawner
            });

            arg.onClose.Invoke();
            CHMMain.UI.CloseUI(gameObject);*/

            SceneManager.LoadScene(3);
        });

        warSimulatorBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UIWarSimulatorSelect, new UIWarSimulatorSelectArg());
        });
    }
}
