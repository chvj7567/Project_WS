using System.Collections.Generic;

public class CHMUnit
{
    Dictionary<Defines.EUnit, UnitData> dicUnitData = new Dictionary<Defines.EUnit, UnitData>();

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
    }

    public void Clear()
    {
        dicUnitData.Clear();
    }

    public UnitData GetUnitData(Defines.EUnit _unit)
    {
        if (dicUnitData.ContainsKey(_unit) == false) return null;

        return dicUnitData[_unit];
    }
}
