using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField, ReadOnly] protected float hp;
    [SerializeField, ReadOnly] protected float mp;

    void Start()
    {
        Init();
    }

    protected abstract void Init();

    public void PlusHp(float _value)
    {
        Debug.Log($"Hp : {hp} -> Hp : {hp + _value}");
        hp += _value;
    }

    public void MinusHp(float _value)
    {
        Debug.Log($"Hp : {hp} -> Hp : {hp - _value}");
        hp -= _value;
    }

    public void PlusMp(float _value)
    {
        Debug.Log($"Mp : {mp} -> Mp : {mp + _value}");
        mp += _value;
    }

    public void MinusMp(float _value)
    {
        Debug.Log($"Mp : {mp} -> Mp : {mp - _value}");
        mp -= _value;
    }
}
