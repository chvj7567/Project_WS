using System.Collections.Generic;

public class CHMLevel
{
    Dictionary<Defines.EUnit, List<LevelData>> dicLevelData = new Dictionary<Defines.EUnit, List<LevelData>>();

    public void Init()
    {
        for (int i = 0; i < (int)Defines.EUnit.Max; ++i)
        {
            List<LevelData> liLevelData = new List<LevelData>();
            var unit = (Defines.EUnit)i;

            for (int j = 0; j < (int)Defines.ELevel.Max; ++j)
            {
                var level = (Defines.ELevel)j;

                CHMMain.Resource.LoadLevelData(unit, level, (_) =>
                {
                    if (_ == null) return;

                    liLevelData.Add(_);
                });
            }

            dicLevelData.Add(unit, liLevelData);
        }
    }

    public void Clear()
    {
        dicLevelData.Clear();
    }

    public LevelData GetLevelData(Defines.EUnit _unit, Defines.ELevel _level)
    {
        if (dicLevelData.ContainsKey(_unit) == false) return null;

        return dicLevelData[_unit][(int)_level];
    }
}
