using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EResourceType
{
    Character,
    UI,
    Json,
}

public enum ECharacter
{
    Slime,
}

public class CHMResource
{
    private void LoadAsset<T>(string _bundleName, string _assetName, Action<T> _callback) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        CHMAssetBundle.LoadAssetOnEditor<T>(_bundleName, _assetName, _callback);
#else
        CHMAssetBundle.LoadAsset<T>(_bundleName, _assetName, _callback);
#endif
    }

    public void LoadString(EJsonType _stringType, Action<TextAsset> _callback)
    {
        LoadAsset<TextAsset>($"{EResourceType.Json.ToString()}", $"{_stringType.ToString()}", _callback);
    }

    public void InstantiateAsObservable<T>(string _bundleName, string _assetName, Action<T> _callback = null) where T : UnityEngine.Object
    {
        Action<T> _callbackTemp = original =>
        {
            if (_callback != null)
            {
                if (original == null)
                {
                    _callback(null);
                }
                else
                {
                    if (typeof(T) == typeof(GameObject))
                    {
                        GameObject go = original as GameObject;
                        T t = Instantiate(go) as T;
                        _callback(t);
                    }
                    else
                    {
                        _callback(GameObject.Instantiate(original));
                    }
                }
            }
        };

        LoadAsset<T>(_bundleName, _assetName, _callbackTemp);
    }

    public void InstantiateCharacter(ECharacter _character, Action<GameObject> _callback = null, bool _bRandom = false, int _iIndex = 1)
    {
        if (_bRandom == false)
        {
            InstantiateAsObservable<GameObject>($"{EResourceType.Character.ToString()}/{_character.ToString()}", $"{_character.ToString()}{_iIndex}", _callback);
        }
        else
        {
            InstantiateAsObservable<GameObject>($"{EResourceType.Character.ToString()}/{_character.ToString()}", $"{_character.ToString()}{GetRandomCharacterNumber(_character)}", _callback);
        }
    }

    public void InstantiateUI(EUI _ui, Action<GameObject> _event = null)
    {
        InstantiateAsObservable<GameObject>($"{EResourceType.UI.ToString()}", $"{_ui.ToString()}", _event);
    }

    public GameObject Instantiate(GameObject _object, Transform _parent = null)
    {
        if (_object == null) return null;

        CHPoolable poolable = _object.GetComponent<CHPoolable>();
        if (poolable != null)
        {
            if (poolable.GetIsUse())
            {
                return CHMMain.Pool.Pop(_object, _parent).gameObject;
            }
            else
            {
                return GameObject.Instantiate(_object, _parent);
            }
        }
        else
        {
            return GameObject.Instantiate(_object, _parent);
        }
    }

    public void Destroy(GameObject _object, float _time = 0)
    {
        if (_object == null) return;

        CHPoolable poolable = _object.GetComponent<CHPoolable>();
        if (poolable != null && poolable.GetIsUse())
        {
            CHMMain.Pool.Push(poolable);
        }
        else
        {
            UnityEngine.Object.Destroy(_object, _time);
        }
    }

    int GetRandomCharacterNumber(ECharacter _character)
    {
        switch (_character)
        {
            case ECharacter.Slime:
                return UnityEngine.Random.Range(1, 9);
        }

        return -1;
    }
}
