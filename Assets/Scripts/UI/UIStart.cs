using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEditor.SceneManagement;
using System.Threading;

public class UIStartArg : CHUIArg
{
    public int stage;
    public Action<int> onStage;
    public CHSpawner spawner;
}

public class UIStart : UIBase
{
    UIStartArg arg;

    [SerializeField] Button startBtn;
    [SerializeField] Button optionBtn;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIStartArg;
    }

    private void Start()
    {
        PlayerPrefs.SetInt(Defines.EPlayerPrefs.Stage.ToString(), arg.stage);

        startBtn.OnClickAsObservable().Subscribe(_ =>
        {
            if (arg.onStage != null)
                arg.onStage.Invoke(PlayerPrefs.GetInt(Defines.EPlayerPrefs.Stage.ToString()));

            CHMMain.UI.ShowUI(Defines.EUI.UICount, new UICountArg
            {
                spawner = arg.spawner
            });

            CHMMain.UI.CloseUI(gameObject);
        });

        optionBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UITowerInfo, new UITowerInfoArg());
        });
    }
}
