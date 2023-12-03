using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

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

    Action onClose;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UIStartArg;
    }

    private void Start()
    {
        PlayerPrefs.SetInt(Defines.EPlayerPrefs.Stage.ToString(), arg.stage);

        startBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UIGameSelect, new UIGameSelectArg
            {
                stage = arg.stage,
                spawner = arg.spawner,
                onStage = arg.onStage,
                onClose = onClose
            });
        });

        optionBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UITowerInfo, new UITowerInfoArg());
        });

        onClose += () =>
        {
            CHMMain.UI.CloseUI(gameObject);
        };
    }
}
