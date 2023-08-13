using System.Collections.Generic;
using UnityEditor.AnimatedValues;
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

        GameObject.Destroy(orginBall);
        GameObject.Destroy(originGaugeBar);
        GameObject.Destroy(originDamageText);
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

    public UnitData GetUnitData(Defines.EUnit _eUnit)
    {
        if (dicUnitData.ContainsKey(_eUnit) == false) return null;

        return dicUnitData[_eUnit];
    }

    public void CreateUnit(Defines.EUnit _eUnit, Defines.ELayer _eTeamLayer, Defines.ELayer _eTargetLayer, Vector3 _position, List<CHTargetTracker> _liTargetTracker, List<LayerMask> _liTargetMask, bool onHpBar = true)
    {
        CHMMain.Resource.InstantiateBall((ball) =>
        {
            unitList.Add(ball);
            if (_eTargetLayer == Defines.ELayer.Red)
            {
                ball.name = $"{_eUnit}Unit(My) {redIndex++}";
            }
            else
            {
                ball.name = $"{_eUnit}Unit(Enemy) {blueIndex++}";
            }

            SetUnit(ball, _eUnit);
            SetColor(ball, _eUnit);
            SetLayer(ball, _eTeamLayer);
            SetTargetMask(ball, _eTargetLayer);

            var unitBase = ball.GetComponent<CHUnitBase>();
            if (unitBase != null)
            {
                unitBase.onHpBar = onHpBar;
            }

            ball.transform.position = _position;

            var targetTracker = ball.GetComponent<CHTargetTracker>();
            if (targetTracker != null)
            {
                if (_liTargetTracker != null)
                    _liTargetTracker.Add(targetTracker);
                if (_liTargetMask != null)
                    _liTargetMask.Add(targetTracker.targetMask);

                // 유닛 생성 시 바로 공격하지 않도록 비활성화
                //targetTracker.targetMask = 0;
            }
        });
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
