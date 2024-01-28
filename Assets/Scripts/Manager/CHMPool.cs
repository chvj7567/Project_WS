using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CHMPool

{
    #region Pool
    class CHPool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<CHPoolable> stPool = new Stack<CHPoolable>();

        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}Root";

            for (int i = 0; i < count; ++i)
            {
                CHPoolable poolable = Create();
                poolable.transform.SetParent(Root, false);
                poolable.isUse = false;
                poolable.gameObject.SetActive(false);
                Push(poolable);
            }
        }

        CHPoolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<CHPoolable>();
        }

        public void Push(CHPoolable poolable)
        {
            if (poolable == null) return;

            poolable.transform.SetParent(Root, false);
            poolable.isUse = false;
            poolable.gameObject.SetActive(false);

            stPool.Push(poolable);
        }

        public CHPoolable Pop(Transform parent)
        {
            CHPoolable poolable;

            if (stPool.Count > 0)
            {
                do
                {
                    if (stPool.Count <= 0)
                    {
                        poolable = Create();
                        break;
                    }

                    poolable = stPool.Pop();

                } while (poolable.isUse);
            }
            else
            {
                poolable = Create();
            }

            poolable.transform.SetParent(parent, false);
            poolable.isUse = true;
            poolable.gameObject.SetActive(true);

            return poolable;
        }
    }
    #endregion

    Dictionary<string, CHPool> poolDic = new Dictionary<string, CHPool>();
    GameObject rootObject;

    public void Init()
    {
        rootObject = GameObject.Find("@CHMPool");
        if (rootObject == null)
        {
            rootObject = new GameObject { name = "@CHMPool" };
        }

        Object.DontDestroyOnLoad(rootObject);
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        CHPool pool = new CHPool();
        pool.Init(original, count);
        pool.Root.parent = rootObject.transform;

        poolDic.Add(original.name, pool);
    }

    public void Push(CHPoolable poolable)
    {
        if (poolable == null)
            return;

        if (poolDic.ContainsKey(poolable.gameObject.name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        poolDic[poolable.gameObject.name].Push(poolable);
    }

    public CHPoolable Pop(GameObject original, Transform parent = null)
    {
        if (poolDic.ContainsKey(original.name) == false)
            CreatePool(original);

        return poolDic[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (poolDic.ContainsKey(name) == false)
            return null;
        return poolDic[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in rootObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        poolDic.Clear();
    }
}
