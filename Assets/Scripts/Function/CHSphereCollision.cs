using System;
using UniRx;
using UnityEngine;

public class CHSphereCollision : MonoBehaviour
{
    SphereCollider sphereCollider;
    float stayTickTime = -1f;
    float stayTickLastTime = -1f;

    Subject<Transform> subjectEnter = new Subject<Transform>();
    Subject<Transform> subjectStay = new Subject<Transform>();
    Subject<Transform> subjectExit = new Subject<Transform>();
    public IObservable<Transform> OnEnter => subjectEnter;
    public IObservable<Transform> OnStay => subjectStay;
    public IObservable<Transform> OnExit => subjectExit;

    IDisposable disposeEnter;
    IDisposable disposeStay;
    IDisposable disposeExit;

    bool useStay = false;

    public void Init(float _sphereRadius, float _stayTickTime = 0f)
    {
        sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = _sphereRadius;
        gameObject.layer = 2;

        if (_stayTickTime >= 0f)
        {
            stayTickTime = _stayTickTime;
            stayTickLastTime = -1f;
            useStay = true;
        }
        else
        {
            useStay = false;
        }
    }

    public void SetCollisionCenter(float _x, float _y, float _z)
    {
        sphereCollider.center = new Vector3(_x, _y, _z);
    }

    private void Update()
    {
        if (useStay)
        {
            if (stayTickLastTime >= 0f && stayTickLastTime < stayTickTime)
            {
                stayTickLastTime += Time.deltaTime;
            }
            else
            {
                stayTickLastTime = -1f;
            }
        }
    }

    public void TriggerEnterCallback(IDisposable _disposable)
    {
        // 풀링 사용시 구독 중독 방지
        if (disposeEnter != null)
        {
            disposeEnter.Dispose();
            disposeEnter = _disposable;
        }
        else
        {
            disposeEnter = _disposable;
        }
    }

    public void TriggerStayCallback(IDisposable _disposable)
    {
        // 풀링 사용시 구독 중독 방지
        if (disposeStay != null)
        {
            disposeStay.Dispose();
            disposeStay = _disposable;
        }
        else
        {
            disposeStay = _disposable;
        }
    }

    public void TriggerExitCallback(IDisposable _disposable)
    {
        // 풀링 사용시 구독 중독 방지
        if (disposeExit != null)
        {
            disposeExit.Dispose();
            disposeExit = _disposable;
        }
        else
        {
            disposeExit = _disposable;
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

    private void OnTriggerStay(Collider other)
    {
        
        if (useStay && stayTickLastTime < 0f)
        {
            stayTickLastTime = 0.0001f;

            var unitBase = other.gameObject.GetComponent<CHUnitBase>();
            if (unitBase != null)
            {
                subjectStay.OnNext(unitBase.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var unitBase = other.gameObject.GetComponent<CHUnitBase>();
        if (unitBase != null)
        {
            subjectExit.OnNext(unitBase.transform);
        }
    }
}
