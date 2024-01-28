using System;
using UnityEngine;
using UnityEngine.UI;

public class Infomation
{
    [Serializable]
    public class StringInfo
    {
        public int stringID = -1;
        public string value = "";
    }

    [Serializable]
    public class StageInfo
    {
        public int stage = -1;
        public int wave = -1;
        public float waveTime = -1f;
        public float spawnDelay = -1f;
        public bool isBoss = false;
        public float bossMultiHp = -1f;
    }

    [Serializable]
    public class StageMonsterInfo
    {
        public int stage = -1;
        public int wave = -1;
        public Defines.EUnit monsterUnit = Defines.EUnit.None;
        public int monsterCount = -1;
    }

    [Serializable]
    public class UnitInfo
    {
        public Defines.EUnit eUnit = Defines.EUnit.None;
        public int nameStringID = -1;
        public float maxHp = -1f;
        public float hp = -1f;
        public float hpRegenPerSecond = -1f;
        public float maxMp = -1f;
        public float mp = -1f;
        public float mpRegenPerSecond = -1f;
        public float attackPower = -1f;
        public float defensePower = -1f;
        public float attackDelay = -1f;
        public float attackDistance = -1f;
        public float moveSpeed = -1f;
        public float rotateSpeed = -1f;
        public float range = -1f;
        public float rangeMulti = -1f;
        public float viewAngle = -1f;
        public Defines.ESkill eSkill1ID = Defines.ESkill.None;
        public Defines.ESkill eSkill2ID = Defines.ESkill.None;
        public Defines.ESkill eSkill3ID = Defines.ESkill.None;
        public Defines.ESkill eSkill4ID = Defines.ESkill.None;

        public UnitInfo Clone()
        {
            UnitInfo newUnitInfo = new UnitInfo();

            newUnitInfo.eUnit = this.eUnit;
            newUnitInfo.nameStringID = this.nameStringID;
            newUnitInfo.maxHp = this.maxHp;
            newUnitInfo.hp = this.hp;
            newUnitInfo.hpRegenPerSecond = this.hpRegenPerSecond;
            newUnitInfo.maxMp = this.maxMp;
            newUnitInfo.mp = this.mp;
            newUnitInfo.mpRegenPerSecond = this.mpRegenPerSecond;
            newUnitInfo.attackPower = this.attackPower;
            newUnitInfo.defensePower = this.defensePower;
            newUnitInfo.attackDelay = this.attackDelay;
            newUnitInfo.attackDistance = this.attackDistance;
            newUnitInfo.moveSpeed = this.moveSpeed;
            newUnitInfo.rotateSpeed = this.rotateSpeed;
            newUnitInfo.range = this.range;
            newUnitInfo.rangeMulti = this.rangeMulti;
            newUnitInfo.viewAngle = this.viewAngle;
            newUnitInfo.eSkill1ID = this.eSkill1ID;
            newUnitInfo.eSkill2ID = this.eSkill2ID;
            newUnitInfo.eSkill3ID = this.eSkill3ID;
            newUnitInfo.eSkill4ID = this.eSkill4ID;

            return newUnitInfo;
        }
    }

    [Serializable]
    public class TargetInfo
    {
        public GameObject objTarget = null;
        public float distance = -1f;
    }

    [Serializable]
    public class CreateUnitInfo
    {
        public Defines.EUnit eUnit;
        public Vector3 createPos;
        public Defines.ELayer eTeamLayer;
        public Defines.ELayer eTargetLayer;
    }

    [Serializable]
    public class ShopInfo
    {
        public Defines.EShop shopID = Defines.EShop.None;
        public int step = 0;
        public int gold = 0;
    }

    [Serializable]
    public class ConstValueInfo
    {
        public Defines.EConstValue eConst = Defines.EConstValue.None;
        public float value = -1f;
    }
}
