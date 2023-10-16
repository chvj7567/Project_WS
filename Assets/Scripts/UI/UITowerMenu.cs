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

    [SerializeField] List<TowerMenuItem> towerMenuItemList = new List<TowerMenuItem>();

    Data.Player playerData;

    public override void InitUI(CHUIArg _uiArg)
    {
        arg = _uiArg as UITowerMenuArg;
    }

    private void Start()
    {
        playerData = CHMData.Instance.GetPlayerData(Defines.EData.Player.ToString());

        for (int i = 0; i < towerMenuItemList.Count; ++i)
        {
            int unitIndex = i;

            var shopData = CHMData.Instance.GetShopData(towerMenuItemList[unitIndex].GetShopEnum());
            var shopInfo = CHMMain.Json.GetShopInfo(shopData.shopID, shopData.step);
            if (shopInfo == null)
                return;

            towerMenuItemList[unitIndex].goldText.SetText(shopInfo.gold);
            towerMenuItemList[unitIndex].button.OnClickAsObservable().Subscribe(_ =>
            {
                if (playerData == null)
                    return;

                if (playerData.gold >= shopInfo.gold)
                {
                    playerData.gold -= shopInfo.gold;

                    arg.unit.gameObject.SetActive(true);
                    arg.unit.unit = (Defines.EUnit)unitIndex;
                    arg.unit.InitUnitData();

                    CHMMain.UI.ShowUI(Defines.EUI.UIAlarm, new UIAlarmArg
                    {
                        stringID = 5,
                    });
                }
                else
                {
                    CHMMain.UI.ShowUI(Defines.EUI.UIAlarm, new UIAlarmArg
                    {
                        stringID = 6,
                    });
                }
                

                CHMMain.UI.CloseUI(gameObject);
            });

            
        }
    }
}
