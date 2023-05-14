using UnityEngine;
using static Defines;

[CreateAssetMenu(menuName = "Scriptable Object/Item Data", order = 2)]
public class ItemData : ScriptableObject
{
    [Header("������")]
    [Tooltip("������ �̸�")] public EItem eItem = EItem.None;
    [Tooltip("������ ����"), Multiline(5)] public string itemDesc = null;

    [Header("������ ���� ȿ��")]
    [Tooltip("�߰� �ִ� HP")] public float maxHp = 0f;
    [Tooltip("�߰� HP �ʴ� ȸ����")] public float hpRegenPerSecond = 0f;
    [Tooltip("�߰� �ִ� MP")] public float maxMp = 0f;
    [Tooltip("�߰� MP �ʴ� ȸ����")] public float mpRegenPerSecond = 0f;
    [Tooltip("�߰� ���ݷ�")] public float attackPower = 0f;
    [Tooltip("�߰� ����")] public float defensePower = 0f;
    [Tooltip("�߰� ������ �� ���ǵ�")] public float moveSpeed = 0f;
    [Tooltip("�߰� ȸ���� �� ���ǵ�")] public float rotateSpeed = 0f;
    [Tooltip("�߰� Ÿ���� ���� ��� Ÿ�� ���� ����")] public float range = 0f;
    [Tooltip("�߰� Ÿ���� ���� ��� Ȯ��Ǵ� ���� ��� ����")] public float rangeMulti = 0f;
    [Tooltip("�߰� Ÿ�� ���� ����")] public float viewAngle = 0f;

    [Header("������ ��ų ȿ��")]
    [Tooltip("�߰� ��ų �����Ÿ�")] public float distance = 0f;
    [Tooltip("�߰� ��ų ��Ÿ��")] public float coolTime = 0f;
    [Tooltip("�߰� ��ų ������")] public float damage = 0f;

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
