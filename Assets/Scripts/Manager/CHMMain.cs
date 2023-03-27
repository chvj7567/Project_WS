using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CHMMain : MonoBehaviour
{
    static CHMMain m_instance;
    static CHMMain Instance { get { Init(); return m_instance; } }

    #region Core
    CHMPool m_pool = new CHMPool();
    CHMResource m_resource = new CHMResource();
    CHMUI m_ui = new CHMUI();
    CHMContents m_contents = new CHMContents();
    CHMString m_string = new CHMString();

    public static CHMPool Pool { get { return Instance.m_pool; } }
    public static CHMResource Resource { get { return Instance.m_resource; } }
    public static CHMUI UI { get { return Instance.m_ui; } }
    public static CHMContents Contents { get { return Instance.m_contents; } }
    public static CHMString String { get { return Instance.m_string; } }

    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        UI.UpdateUI();
    }

    static void Init()
    {
        if (m_instance == null)
        {
            GameObject go = GameObject.Find("@CHMMain");
            if (go == null)
                go = new GameObject { name = "@CHMMain" };

            DontDestroyOnLoad(go);
            m_instance = go.GetOrAddComponent<CHMMain>();

            m_instance.m_pool.Init();
            m_instance.m_contents.Init();
        }
    }
}
