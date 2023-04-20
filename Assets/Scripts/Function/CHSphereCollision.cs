using System;
using UniRx;
using UnityEngine;

public class CHSphereCollision : MonoBehaviour
{
    SphereCollider sphereCollider;

    Subject<Transform> subjectEnter = new Subject<Transform>();
    public IObservable<Transform> OnEnter => subjectEnter;

    IDisposable disposable;

    public void Init(float _sphereRadius, IDisposable _disposable)
    {
        sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = _sphereRadius;
        gameObject.layer = 2;

        // 풀링 사용시 구독 중독 방지
        if (disposable != null)
        {
            _disposable.Dispose();
        }
        else
        {
            disposable = _disposable;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var unitBase = other.gameObject.GetComponent<CHUnitBase>();
        if (unitBase != null)
        {
            subjectEnter.OnNext(unitBase.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
