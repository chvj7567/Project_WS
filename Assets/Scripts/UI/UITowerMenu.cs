using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UITowerMenuArg : CHUIArg
{
    public CHUnitBase unit;
}

public class UITowerMenu : UIBase
{
    UITowerMenuArg arg;

    [SerializeField] List<Button> unitBtnList = new List<Button>();

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UITowerMenuArg;
    }

    CancellationTokenSource delayTokenSource;

    private async void Start()
    {
        for (int i = 0; i < unitBtnList.Count; ++i)
        {
            int unitIndex = i;
            unitBtnList[unitIndex].OnClickAsObservable().Subscribe(_ =>
            {
                arg.unit.gameObject.SetActive(true);
                arg.unit.unit = (Defines.EUnit)unitIndex;
                arg.unit.InitUnitData();

                CHMMain.UI.CloseUI(gameObject);
            });
        }
    }
}
