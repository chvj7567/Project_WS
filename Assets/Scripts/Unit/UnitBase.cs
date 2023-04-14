using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField, ReadOnly] protected Infomation.UnitInfo unitInfoOrigin;
    [SerializeField, ReadOnly] protected Infomation.UnitInfo unitInfoCurrent;

    Subject<bool> subjectSetUnitInfo = new Subject<bool>();
    private void Awake()
    {
        Init();
    }

    protected abstract void Init();

    #region OriginUnitInfoGetter
    public Defines.EUnitID GetOriginUnitID() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.eUnitID; }
    public int GetOriginNameStringID() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.nameStringID; }
    public float GetOriginHp() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.hp; }
    public float GetOriginMp() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.mp; }
    public float GetOriginAttackDelay() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.attackDelay; }
    public float GetOriginAttackDistance() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.attackDistance; }
    public float GetOriginMoveSpeed() { if (unitInfoOrigin == null) Init(); return unitInfoOrigin.moveSpeed; }
    #endregion

    #region CurrentUnitInfoGetter
    public Defines.EUnitID GetCurrentUnitID() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.eUnitID; }
    public int GetCurrentNameStringID() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.nameStringID; }
    public float GetCurrentHp() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.hp; }
    public float GetCurrentMp() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.mp; }
    public float GetCurrentAttackDelay() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.attackDelay; }
    public float GetCurrentAttackDistance() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.attackDistance; }
    public float GetCurrentMoveSpeed() { if (unitInfoCurrent == null) Init(); return unitInfoOrigin.moveSpeed; }
    #endregion

    public void PlusHp(float _value)
    {
        if (unitInfoCurrent == null) Init();

        Debug.Log($"Hp : {unitInfoCurrent.hp} -> Hp : {unitInfoCurrent.hp + _value}");
        unitInfoCurrent.hp += _value;
    }

    public void MinusHp(float _value)
    {
        if (unitInfoCurrent == null) Init();

        Debug.Log($"Hp : {unitInfoCurrent.hp} -> Hp : {unitInfoCurrent.hp - _value}");
        unitInfoCurrent.hp -= _value;
    }

    public void PlusMp(float _value)
    {
        if (unitInfoCurrent == null) Init();

        Debug.Log($"Mp : {unitInfoCurrent.mp} -> Mp : {unitInfoCurrent.mp + _value}");
        unitInfoCurrent.mp += _value;
    }

    public void MinusMp(float _value)
    {
        if (unitInfoCurrent == null) Init();

        Debug.Log($"Mp : {unitInfoCurrent.mp} -> Mp : {unitInfoCurrent.mp - _value}");
        unitInfoCurrent.mp -= _value;
    }
}
