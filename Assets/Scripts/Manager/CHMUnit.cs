using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CHMUnit
{
    Dictionary<Defines.EUnit, UnitData> dicUnitData = new Dictionary<Defines.EUnit, UnitData>();
    List<Material> liMaterial = new List<Material>();
    List<Shader> liShader = new List<Shader>();
    List<GameObject> unitList = new List<GameObject>();
    GameObject orginBall = null;
    GameObject originGaugeBar = null;
    GameObject originDamageText = null;

    int redIndex = 0;
    int blueIndex = 0;

    public void Init()
    {
        for (int i = 0; i < (int)Defines.EShader.Max; ++i)
        {
            CHMMain.Resource.LoadShader((Defines.EShader)i, (mat) =>
            {
                liShader.Add(mat);
            });
        }

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

        CHMMain.Resource.LoadOriginBall((ball) =>
        {
            orginBall = ball;
        });

        CHMMain.Resource.LoadOriginGaugeBar((gaugeBar) =>
        {
            originGaugeBar = gaugeBar;
        });

        CHMMain.Resource.LoadOriginDamageText((damageText) =>
        {
            originDamageText = damageText;
        });
    }

    public void Clear()
    {
        dicUnitData.Clear();
        liMaterial.Clear();
        liShader.Clear();

        orginBall = null;
        originGaugeBar = null;
        originDamageText = null;

        foreach (var unit in unitList)
        {
            GameObject.Destroy(unit);
        }

        unitList.Clear();
    }
    
    public void RemoveUnitAll()
    {
        foreach (var unit in unitList)
        {
            CHMMain.Resource.Destroy(unit);
        }

        unitList.Clear();
    }

    public GameObject GetOriginBall()
    {
        return orginBall;
    }

    public GameObject GetOriginGaugeBar()
    {
        return originGaugeBar;
    }

    public GameObject GetOriginDamageText()
    {
        return originDamageText;
    }

    public UnitData GetUnitData(Defines.EUnit eUnit)
    {
        if (dicUnitData.ContainsKey(eUnit) == false)
            return null;

        return dicUnitData[eUnit];
    }

    public List<UnitData> GetUnitDataAll()
    {
        return dicUnitData.Values.ToList();
    }

    public void CreateRandomUnit(Defines.ELayer eTeamLayer, Defines.ELayer eTargetLayer, Vector3 position, List<CHTargetTracker> targetTrackerList = null, List<LayerMask> targetMaskList = null, bool onHpBar = true, bool onMpBar = false, bool onCoolTimeBar = false)
    {
        var randomUnit = (Defines.EUnit)Random.Range((int)Defines.EUnit.White, (int)Defines.EUnit.Monster1);
        CreateUnit(randomUnit, eTeamLayer, eTargetLayer, position, targetTrackerList, targetMaskList, onHpBar, onMpBar, onCoolTimeBar);
    }

    public void CreateUnit(Defines.EUnit eUnit, Defines.ELayer eTeamLayer, Defines.ELayer eTargetLayer, Vector3 position, List<CHTargetTracker> targetTrackerList = null, List<LayerMask> targetMaskList = null, bool onHpBar = true, bool onMpBar = false, bool onCoolTimeBar = false)
    {
        CHMMain.Resource.InstantiateBall((ball) =>
        {
            unitList.Add(ball);
            if (eTargetLayer == Defines.ELayer.Red)
            {
                ball.name = $"{eUnit}Unit(My) {redIndex++}";
            }
            else
            {
                ball.name = $"{eUnit}Unit(Enemy) {blueIndex++}";
            }

            SetUnit(ball, eUnit);
            SetLayer(ball, eTeamLayer);
            SetTargetMask(ball, eTargetLayer);

            var unitBase = ball.GetComponent<CHUnitBase>();
            if (unitBase != null)
            {
                unitBase.onHpBar = onHpBar;
                unitBase.onMpBar = onMpBar;
                unitBase.onCoolTimeBar = onCoolTimeBar;
            }

            ball.transform.position = position;

            var targetTracker = ball.GetComponent<CHTargetTracker>();
            if (targetTracker != null)
            {
                if (targetTrackerList != null)
                    targetTrackerList.Add(targetTracker);
                if (targetMaskList != null)
                    targetMaskList.Add(targetTracker.targetMask);

                // 유닛 생성 시 바로 공격하지 않도록 비활성화
                targetTracker.targetMask = 0;
            }
        });
    }

    public void SetUnit(GameObject unit, Defines.EUnit eUnit)
    {
        if (unit == null)
            return;

        var unitBase = unit.GetComponent<CHUnitBase>();
        if (unitBase != null)
        {
            unitBase.unit = eUnit;
            SetColor(unit, eUnit);
        }
    }

    void SetColor(GameObject unit, Defines.EUnit eUnit)
    {
        if (unit == null)
            return;

        var index = (int)eUnit;
        var meshRenderer = unit.GetComponent<MeshRenderer>();
        if (meshRenderer != null && index < liMaterial.Count)
        {
            meshRenderer.material = liMaterial[index];
        }
    }

    public void SetLayer(GameObject unit, Defines.ELayer eLayer)
    {
        if (unit == null)
            return;

        unit.layer = (int)eLayer;
    }

    public void SetTargetMask(GameObject unit, Defines.ELayer eLayer)
    {
        if (unit == null)
            return;

        var targetTracker = unit.GetComponent<CHTargetTracker>();

        if (targetTracker != null)
        {
            targetTracker.targetMask = 1 << (int)eLayer;
        }
    }

    public void SetTargetMask(CHTargetTracker targetTracker, Defines.ELayer eLayer)
    {
        if (targetTracker == null)
            return;

        targetTracker.targetMask = 1 << (int)eLayer;
    }
}
