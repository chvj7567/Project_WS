using UnityEngine;
using System;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class CHMResource
{
    private void LoadAsset<T>(string _bundleName, string _assetName, Action<T> _callback) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        CHMMain.Bundle.LoadAssetOnEditor<T>(_bundleName, _assetName, _callback);
#else
        CHMMain.Bundle.LoadAsset<T>(_bundleName, _assetName, _callback);
#endif
    }

    public void LoadJson(Defines.EJsonType _jsonType, Action<TextAsset> _callback)
    {
        LoadAsset<TextAsset>($"{Defines.EResourceType.Json.ToString()}", $"{_jsonType.ToString()}", _callback);
    }

    public void LoadMaterial(Defines.EMaterial _material, Action<Material> _callback)
    {
        LoadAsset<Material>($"{Defines.EAssetPiece.Materials.ToString()}", $"{_material.ToString()}", _callback);
    }

    public void LoadSkillData(Defines.ESkill _skill, Action<SkillData> _callback)
    {
        LoadAsset<SkillData>($"{Defines.EResourceType.Scriptable.ToString()}", $"{_skill.ToString()}", _callback);
    }

    public void LoadUnitData(Defines.EUnit _unit, Action<UnitData> _callback)
    {
        LoadAsset<UnitData>($"{Defines.EResourceType.Scriptable.ToString()}", $"{_unit.ToString()}", _callback);
    }

    public void LoadLevelData(Defines.EUnit _unit, Defines.ELevel _level, Action<LevelData> _callback)
    {
        LoadAsset<LevelData>($"{Defines.EResourceType.Scriptable.ToString()}", $"{_unit.ToString()}{_level.ToString()}", _callback);
    }

    public void LoadItemData(Defines.EItem _item, Action<ItemData> _callback)
    {
        LoadAsset<ItemData>($"{Defines.EResourceType.Scriptable.ToString()}", $"{_item.ToString()}", _callback);
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

    public void InstantiateMajor(Defines.EMajor _major, Action<GameObject> _callback = null)
    {
        InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Major.ToString()}", $"{_major.ToString()}", _callback);
    }

    public void InstantiateUnit(Defines.EUnit _unit, Action<GameObject> _callback = null)
    {
        InstantiateAsObservable<GameObject>($"{Defines.EResourceType.Unit.ToString()}", $"{_unit.ToString()}", _callback);
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
            return CHMMain.Pool.Pop(_object, _parent).gameObject;
        }
        else
        {
            GameObject go = GameObject.Instantiate(_object, _parent);
            return go;
        }
    }

    public async void Destroy(GameObject _object, float _time = 0)
    {
        if (_object == null) return;

        CHPoolable poolable = _object.GetComponent<CHPoolable>();
        if (poolable != null)
        {
            await Task.Delay((int)(_time * 1000f));
            CHMMain.Pool.Push(poolable);
        }
        else
        {
            UnityEngine.Object.Destroy(_object, _time);
        }
    }
}
