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

            CHMMain.Resource.LoadUnitMaterial((Defines.EUnit)i, (mat) =>
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

    public UnitData GetUnitData(Defines.EUnit _eUnit)
    {
        if (dicUnitData.ContainsKey(_eUnit) == false) return null;

        return dicUnitData[_eUnit];
    }

    public void SetUnit(GameObject _objUnit, Defines.EUnit _eUnit)
    {
        if (_objUnit == null) return;

        var unitBase = _objUnit.GetComponent<CHUnitBase>();
        if (unitBase != null)
        {
            unitBase.unit = _eUnit;
        }
    }

    public void SetColor(GameObject _objUnit, Defines.EUnit _eUnit)
    {
        if (_objUnit == null) return;

        var index = (int)_eUnit;
        var meshRenderer = _objUnit.GetComponent<MeshRenderer>();
        if (meshRenderer != null && index < liMaterial.Count)
        {
            meshRenderer.material = liMaterial[index];
        }
    }

    public void SetLayer(GameObject _objUnit, Defines.ELayer _eLayer)
    {
        if (_objUnit == null) return;

        _objUnit.layer = (int)_eLayer;
    }

    public void SetTargetMask(GameObject _objUnit, Defines.ELayer _eLayer)
    {
        if (_objUnit == null) return;

        var targetTracker = _objUnit.GetComponent<CHTargetTracker>();

        if (targetTracker != null)
        {
            targetTracker.targetMask = 1 << (int)_eLayer;
        }
    }
}
