using UnityEngine;
using static Defines;

[CreateAssetMenu(menuName = "Scriptable Object/Item Data", order = 2)]
public class ItemData : ScriptableObject
{
    [Header("아이템")]
    [Tooltip("아이템 이름")] public EItem eItem = EItem.None;
    [Tooltip("아이템 설명"), Multiline(5)] public string itemDesc = null;

    [Header("아이템 유닛 효과")]
    [Tooltip("추가 최대 HP")] public float maxHp = 0f;
    [Tooltip("추가 HP 초당 회복량")] public float hpRegenPerSecond = 0f;
    [Tooltip("추가 최대 MP")] public float maxMp = 0f;
    [Tooltip("추가 MP 초당 회복량")] public float mpRegenPerSecond = 0f;
    [Tooltip("추가 공격력")] public float attackPower = 0f;
    [Tooltip("추가 방어력")] public float defensePower = 0f;
    [Tooltip("추가 움직일 때 스피드")] public float moveSpeed = 0f;
    [Tooltip("추가 회전할 때 스피드")] public float rotateSpeed = 0f;
    [Tooltip("추가 타겟이 없을 경우 타겟 감지 범위")] public float range = 0f;
    [Tooltip("추가 타겟이 있을 경우 확장되는 감지 배수 범위")] public float rangeMulti = 0f;
    [Tooltip("추가 타겟 감지 각도")] public float viewAngle = 0f;

    [Header("아이템 스킬 효과")]
    [Tooltip("추가 스킬 사정거리")] public float distance = 0f;
    [Tooltip("추가 스킬 쿨타임")] public float coolTime = 0f;
    [Tooltip("추가 스킬 데미지")] public float damage = 0f;

    public ItemData Clone()
    {
        ItemData clone = ScriptableObject.CreateInstance<ItemData>();
        clone.eItem = this.eItem;
        clone.itemDesc = this.itemDesc;
        clone.attackPower = this.attackPower;
        clone.defensePower = this.defensePower;
        clone.maxHp = this.maxHp;
        clone.maxMp = this.maxMp;
        clone.hpRegenPerSecond = this.hpRegenPerSecond;
        clone.mpRegenPerSecond = this.mpRegenPerSecond;
        clone.moveSpeed = this.moveSpeed;
        clone.rotateSpeed = this.rotateSpeed;
        clone.coolTime = this.coolTime;
        clone.damage = this.damage;

        return clone;
    }
}
