using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHMPool

{
    #region Pool
    class CHPool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<CHPoolable> poolStack = new Stack<CHPoolable>();

        public void Init(GameObject _original, int _count = 5)
        {
            Original = _original;
            Root = new GameObject().transform;
            Root.name = $"{_original.name}Root";

            for (int i = 0; i < _count; ++i)
            {
                Push(Create());
            }
        }

        CHPoolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return Util.GetOrAddComponent<CHPoolable>(go);
        }

        public void Push(CHPoolable _poolable)
        {
            if (_poolable == null) return;

            _poolable.transform.parent = Root;
            _poolable.gameObject.SetActive(false);

            poolStack.Push(_poolable);
        }

        public CHPoolable Pop(Transform _parent)
        {
            CHPoolable poolable;

            if (poolStack.Count > 0)
            {
                poolable = poolStack.Pop();
            }
            else
            {
                poolable = Create();
            }

            poolable.transform.parent = _parent;
            poolable.gameObject.SetActive(true);

            return poolable;
        }
    }
    #endregion

    Dictionary<string, CHPool> poolDic = new Dictionary<string, CHPool>();
    Transform root;

    public void Init()
    {
        if (root == null)
        {
            root = new GameObject { name = "@PoolRoot" }.transform;
            Object.DontDestroyOnLoad(root);
        }
    }

    public void CreatePool(GameObject _original, int _count = 5)
    {
        CHPool pool = new CHPool();
        pool.Init(_original, _count);
        pool.Root.parent = root;

        poolDic.Add(_original.name, pool);
    }

    public void Push(CHPoolable poolable)
    {
        if (poolDic.ContainsKey(poolable.gameObject.name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        poolDic[poolable.gameObject.name].Push(poolable);
    }

    public CHPoolable Pop(GameObject _original, Transform _parent = null)
    {
        if (poolDic.ContainsKey(_original.name) == false)
            CreatePool(_original);

        return poolDic[_original.name].Pop(_parent);
    }

    public GameObject GetOriginal(string _name)
    {
        if (poolDic.ContainsKey(_name) == false)
            return null;
        return poolDic[_name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in root)
        {
            GameObject.Destroy(child.gameObject);
        }

        poolDic.Clear();
    }
}
