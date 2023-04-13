using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHMSkill
{
    Vector3 Angle(Transform _trOriginPos, float _angle)
    {
        _angle += _trOriginPos.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    public List<TargetInfo> GetTargetInfoListInRange(Transform _trOriginPos, LayerMask _lmTarget, LayerMask _lmIgnore, float _range, float _viewAngle = 360f)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapSphere(_trOriginPos.position, _range, _lmTarget);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _trOriginPos.position).normalized;

            // 시야각에 걸리는지 확인
            if (Vector3.Angle(_trOriginPos.forward, targetDir) < _viewAngle / 2)
            {
                float targetDis = Vector3.Distance(_trOriginPos.position, targetTr.position);

                // 장애물이 있는지 확인
                if (Physics.Raycast(_trOriginPos.position, targetDir, targetDis, ~(_lmTarget | _lmIgnore)) == false)
                {
                    targetInfoList.Add(new TargetInfo
                    {
                        targetObj = target.gameObject,
                        direction = targetDir,
                        distance = targetDis,
                    });
                }
            }
        }

        return targetInfoList;
    }

    public TargetInfo GetClosestTargetInfo(Transform _trOriginPos, LayerMask _lmTarget, LayerMask _lmIgnore, float _range, float _viewAngle = 360f)
    {
        TargetInfo closestTargetInfo = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange(_trOriginPos, _lmTarget, _lmIgnore, _range, _viewAngle);

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (TargetInfo targetInfo in targetInfoList)
            {
                if (targetInfo.distance < minDis)
                {
                    minDis = targetInfo.distance;
                    closestTargetInfo = targetInfo;
                }
            }
        }

        return closestTargetInfo;
    }

    public List<TargetInfo> GetTargetInfoListInRange(Transform _trOriginPos, LayerMask _lmTarget, LayerMask _lmIgnore, Vector3 _size, Quaternion _quater)
    {
        List<TargetInfo> targetInfoList = new List<TargetInfo>();

        // 범위내에 있는 타겟들 확인
        Collider[] targets = Physics.OverlapBox(_trOriginPos.position, _size / 2f, _quater, _lmTarget);

        foreach (Collider target in targets)
        {
            Transform targetTr = target.transform;
            Vector3 targetDir = (targetTr.position - _trOriginPos.position).normalized;
            float targetDis = Vector3.Distance(_trOriginPos.position, targetTr.position);

            // 장애물이 있는지 확인
            if (Physics.Raycast(_trOriginPos.position, targetDir, targetDis, ~(_lmTarget | _lmIgnore)) == false)
            {
                targetInfoList.Add(new TargetInfo
                {
                    targetObj = target.gameObject,
                    direction = targetDir,
                    distance = targetDis,
                });
            }
        }

        return targetInfoList;
    }

    public TargetInfo GetClosestTargetInfo(Transform _trOriginPos, LayerMask _lmTarget, LayerMask _lmIgnore, Vector3 _size, Quaternion _quater)
    {
        TargetInfo closestTargetInfo = null;
        List<TargetInfo> targetInfoList = GetTargetInfoListInRange(_trOriginPos, _lmTarget, _lmIgnore, _size, _quater);

        if (targetInfoList.Count > 0)
        {
            float minDis = Mathf.Infinity;

            foreach (TargetInfo targetInfo in targetInfoList)
            {
                if (targetInfo.distance < minDis)
                {
                    minDis = targetInfo.distance;
                    closestTargetInfo = targetInfo;
                }
            }
        }

        return closestTargetInfo;
    }

    
}
