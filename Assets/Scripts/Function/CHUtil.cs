using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute {}

public static class CHUtil
{
    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        if (list == null)
        {
            return true;
        }

        if (list.Count <= 0)
        {
            return true;
        }

        return false;
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    // �ڽ� ���ӿ�����Ʈ �߿� T�� �ش��ϴ� ������Ʈ�� �����´�.
    // �ڽĵ��� �� �Ʒ� �ڽĵ���� ã������ recursive�� true�� üũ�Ͽ� ��������� ã�´�.
    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    // ���� ������Ʈ�� ã�� ��� ���׸� ������ �ƴ� �Ϲ� �������� ȣ���� �� �ְ� �������̵��Ѵ�.
    public static GameObject FindChild(this GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static float GetParticleTime(this ParticleSystem _particle)
    {
        float time = -1;

        var arrParticle = _particle.GetComponentsInChildren<ParticleSystem>();

        foreach (var particle in arrParticle)
        {
            time = Mathf.Max(time, particle.duration);
        }

        return time;
    }
}
