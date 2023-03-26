using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHObjectTracker : MonoBehaviour
{
    [SerializeField] GameObject obj;
    
    [SerializeField, ReadOnly] SphereCollider collider;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        collider = gameObject.AddComponent<SphereCollider>();
        
    }

    public void SetObject(GameObject _obj)
    {
        obj = _obj;
    }

    void Update()
    {
        Vector3 dir = obj.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
