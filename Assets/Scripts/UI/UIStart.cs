using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIStartArg : CHUIArg
{
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
        startBtn.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.UI.ShowUI(Defines.EUI.UICount, new UICountArg
            {
                spawner = arg.spawner
            });

            CHMMain.UI.CloseUI(gameObject);
        });
    }
}
