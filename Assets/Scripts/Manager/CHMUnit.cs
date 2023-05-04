using System.Collections.Generic;
using UnityEngine;

public class CHMUnit
{
    Dictionary<Defines.EUnit, UnitData> dicUnitData = new Dictionary<Defines.EUnit, UnitData>();
    List<Material> liMaterial = new List<Material>();

    public void Init()
    {
        for (int i = 0; i < (int)Defines.EUnit.Max; ++i)
        {
            var unit = (Defines.EUnit)i;

            CHMMain.Resource.LoadUnitData(unit, (_) =>
            {
                if (_ == null) return;

                dicUnitData.Add(unit, _);
            });
        }

        for (int i = 0; i < (int)Defines.EMaterial.Max; ++i)
        {
            CHMMain.Resource.LoadMaterial((Defines.EMaterial)i, (mat) =>
            {
                liMaterial.Add(mat);
            });
        }
    }

    public void Clear()
    {
        dicUnitData.Clear();
        liMaterial.Clear();
    }

    public UnitData GetUnitData(Defines.EUnit _unit)
    {
        if (dicUnitData.ContainsKey(_unit) == false) return null;

        return dicUnitData[_unit];
    }

    public Material GetUnitMaterial(Defines.EMaterial _material)
    {
        if ((int)_material >= liMaterial.Count) return null;

        return liMaterial[(int)_material];
    }
}
