using System;
using UniRx;
using UnityEngine;
using static Infomation;

public class CHSphereCollision : MonoBehaviour
{
    public SphereCollider sphereCollider;
    float stayTickTime = -1f;
    float stayTickLastTime = -1f;

    Subject<Collider> subjectEnter = new Subject<Collider>();
    Subject<Collider> subjectStay = new Subject<Collider>();
    Subject<Collider> subjectExit = new Subject<Collider>();
    public IObservable<Collider> OnEnter => subjectEnter;
    public IObservable<Collider> OnStay => subjectStay;
    public IObservable<Collider> OnExit => subjectExit;

    IDisposable disposeEnter;
    IDisposable disposeStay;
    IDisposable disposeExit;

    bool useStay = false;

    Transform trCaster;
    EffectInfo effectInfo;

    public void Init(Transform _trCaster, EffectInfo _effectInfo)
    {
        trCaster = _trCaster;
        effectInfo = _effectInfo;

        sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = _effectInfo.sphereRadius;

        SetCollisionCenter();

        gameObject.layer = 2;

        if (_effectInfo.stayTickTime >= 0f)
        {
            stayTickTime = _effectInfo.stayTickTime;
            stayTickLastTime = -1f;
            useStay = true;
        }
        else
        {
            useStay = false;
        }
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
        subjectEnter.OnNext(other);
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (useStay && stayTickLastTime < 0f)
        {
            stayTickLastTime = 0.0001f;

            subjectStay.OnNext(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        subjectExit.OnNext(other);
    }

    void SetCollisionCenter()
    {
        switch (effectInfo.eEffect)
        {
            case Defines.EEffect.FX_Arrow_impact:
                {
                    sphereCollider.center = new Vector3(0f, 4f, 0f);
                }
                break;
            case Defines.EEffect.FX_Arrow_impact2:
                {
                    sphereCollider.center = new Vector3(0f, -23f, 0f);
                }
                break;
            default:
                {
                    sphereCollider.center = new Vector3(0f, 0f, 0f);
                }
                break;
        }
    }


}
