using System.Collections.Generic;
using UnityEngine;

public class CHMItem
{
    Dictionary<Defines.EItem, ItemData> dicItemData = new Dictionary<Defines.EItem, ItemData>();

    public void Init()
    {
        for (int i = 0; i < (int)Defines.EItem.Max; ++i)
        {
            var item = (Defines.EItem)i;

            CHMMain.Resource.LoadItemData(item, (_) =>
            {
                if (_ == null) return;

                dicItemData.Add(item, _);
            });
        }
    }

    public void Clear()
    {
        dicItemData.Clear();
    }

    public ItemData GetItemData(Defines.EItem _item)
    {
        if (dicItemData.ContainsKey(_item) == false) return null;

        return dicItemData[_item];
    }
}
