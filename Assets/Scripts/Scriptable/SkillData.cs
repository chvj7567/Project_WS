using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Defines;
using static Infomation;

[CreateAssetMenu(menuName = "Scriptable Object/Skill Data", order = 0)]
public class SkillData : ScriptableObject
{
    public ESkillID eSkillID = ESkillID.None; // ��ų ���̵�
    public int skillDesc = -1; // ��ų ����
    public bool isTargeting = false; // Ÿ���� or ��Ÿ����
    public float distance = -1f; // �����Ÿ�
    public float coolTime = -1f; // ��Ÿ��
    public ESkillCost eSkillCost = ESkillCost.None; // ��ų ����� ���� ��������
    public float cost = -1f; // ��ų ���
    public List<EffectInfo> liEffectInfo = new List<EffectInfo>(); // ��ų �� ��ü
}
