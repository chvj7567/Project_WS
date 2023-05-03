using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Defines;
using static Infomation;

[CreateAssetMenu(menuName = "Scriptable Object/Skill Data", order = 0)]
public class SkillData : ScriptableObject
{
    public ESkillID eSkillID = ESkillID.None; // 스킬 아이디
    public int skillDesc = -1; // 스킬 설명
    public bool isTargeting = false; // 타겟팅 or 논타겟팅
    public float distance = -1f; // 사정거리
    public float coolTime = -1f; // 쿨타임
    public ESkillCost eSkillCost = ESkillCost.None; // 스킬 비용을 뭘로 지불할지
    public float cost = -1f; // 스킬 비용
    public List<EffectInfo> liEffectInfo = new List<EffectInfo>(); // 스킬 그 자체
}
