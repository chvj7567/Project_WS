using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Unit Data", order = 0)]
public class UnitData : ScriptableObject
{
    [Header("���� ����")]
    [Tooltip("���� �̸�")] public Defines.EUnit eUnit = Defines.EUnit.None;
    [Tooltip("���� �̸� ��Ʈ��ID")] public int nameStringID = -1;
    [Tooltip("�ִ� HP")] public float maxHp = -1f;
    [Tooltip("HP �ʴ� ȸ����")] public float hpRegenPerSecond = -1f;
    [Tooltip("�ִ� MP")] public float maxMp = -1f;
    [Tooltip("MP �ʴ� ȸ����")] public float mpRegenPerSecond = -1f;
    [Tooltip("���ݷ�")] public float attackPower = -1f;
    [Tooltip("����")] public float defensePower = -1f;
    [Tooltip("������ �� ���ǵ�")] public float moveSpeed = -1f;
    [Tooltip("ȸ���� �� ���ǵ�")] public float rotateSpeed = -1f;
    [Tooltip("Ÿ���� ���� ��� Ÿ�� ���� ����")] public float range = -1f;
    [Tooltip("Ÿ���� ���� ��� Ȯ��Ǵ� ���� ��� ����")] public float rangeMulti = -1f;
    [Tooltip("Ÿ�� ���� ����")] public float viewAngle = -1f;

    [Header("��ų ����")]
    [Tooltip("��ų1 �̸�")] public Defines.ESkill eSkill1 = Defines.ESkill.None;
    [Tooltip("��ų2 �̸�")] public Defines.ESkill eSkill2 = Defines.ESkill.None;
    [Tooltip("��ų3 �̸�")] public Defines.ESkill eSkill3 = Defines.ESkill.None;
    [Tooltip("��ų4 �̸�")] public Defines.ESkill eSkill4 = Defines.ESkill.None;

    [Header("������ ����")]
    [Tooltip("������1 �̸�")] public Defines.EItem eItem1 = Defines.EItem.None;

    public UnitData Clone()
    {
        UnitData newUnitInfo = new UnitData();

        newUnitInfo.eUnit = this.eUnit;
        newUnitInfo.nameStringID = this.nameStringID;
        newUnitInfo.maxHp = this.maxHp;
        newUnitInfo.hpRegenPerSecond = this.hpRegenPerSecond;
        newUnitInfo.maxMp = this.maxMp;
        newUnitInfo.mpRegenPerSecond = this.mpRegenPerSecond;
        newUnitInfo.attackPower = this.attackPower;
        newUnitInfo.defensePower = this.defensePower;
        newUnitInfo.moveSpeed = this.moveSpeed;
        newUnitInfo.rotateSpeed = this.rotateSpeed;
        newUnitInfo.range = this.range;
        newUnitInfo.rangeMulti = this.rangeMulti;
        newUnitInfo.viewAngle = this.viewAngle;
        newUnitInfo.eSkill1 = this.eSkill1;
        newUnitInfo.eSkill2 = this.eSkill2;
        newUnitInfo.eSkill3 = this.eSkill3;
        newUnitInfo.eSkill4 = this.eSkill4;
        newUnitInfo.eItem1 = this.eItem1;

        return newUnitInfo;
    }
}
