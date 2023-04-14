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

    public void LoadJson(Defines.EJsonType _jsonType, Action<TextAsset> _callback)
    {
        LoadAsset<TextAsset>($"{Defines.EResourceType.Json.ToString()}", $"{_jsonType.ToString()}", _callback);
    }

    public void InstantiateAsObservable<T>(string _bundleName, string _assetName, Action<T> _callback = null) where T : UnityEngine.Object
    {
        Action<T> _callbackTemp = original =>
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
                    if (_callback != null) _callback(t);
                }
                else
                {
                    if (_callback != null) _callback(GameObject.Instantiate(original));
                }
            }
        };

        LoadAsset<T>(_bundleName, _assetName, _callbackTemp);
    }

    public void InstantiateCharacter(Defines.ECharacter _character, Action<GameObject> _callback = null, bool _random = false, int _index = 1)
    {
        if (_random == false)
        {
            InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Character.ToString()}/{_character.ToString()}", $"{_character.ToString()}{_index}", _callback);
        }
        else
        {
            InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Character.ToString()}/{_character.ToString()}", $"{_character.ToString()}{GetRandomCharacterNumber(_character)}", _callback);
        }
    }

    public void InstantiateMajor(Defines.EMajor _major, Action<GameObject> _callback = null)
    {
        InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Major.ToString()}", $"{_major.ToString()}", _callback);
    }

    public void InstantiateUI(Defines.EUI _ui, Action<GameObject> _callback = null)
    {
        InstantiateAsObservable<GameObject>($"{Defines.EResourceType.UI.ToString()}", $"{_ui.ToString()}", _callback);
    }

    public void InstantiateEffect(Defines.EEffect _effect, Action<GameObject> _callback = null)
    {
        InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Effect.ToString()}", $"{_effect.ToString()}", _callback);
    }

    public void InstantiateDecal(Defines.EDecal _decal, Action<GameObject> _callback = null)
    {
        InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Decal.ToString()}", $"{_decal.ToString()}", _callback);
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

    int GetRandomCharacterNumber(Defines.ECharacter _character)
    {
        switch (_character)
        {
            case Defines.ECharacter.Slime:
                return UnityEngine.Random.Range(1, 9);
        }

        return -1;
    }
}
