using UnityEngine;

public class CHMMain : MonoBehaviour
{
    static CHMMain m_instance;
    static CHMMain Instance { get { Init(); return m_instance; } }

    #region Core
    CHMPool m_pool = new CHMPool();
    CHMResource m_resource = new CHMResource();
    CHMUI m_ui = new CHMUI();
    CHMJson m_json = new CHMJson();
    CHMString m_string = new CHMString();
    CHMParticle m_particle = new CHMParticle();
    CHMSkill m_skill = new CHMSkill();
    CHMUnit m_unit = new CHMUnit();
    CHMLevel m_level = new CHMLevel();
    CHMItem m_item = new CHMItem();

    public static CHMPool Pool { get { return Instance.m_pool; } }
    public static CHMResource Resource { get { return Instance.m_resource; } }
    public static CHMUI UI { get { return Instance.m_ui; } }
    public static CHMJson Json { get { return Instance.m_json; } }
    public static CHMString String { get { return Instance.m_string; } }
    public static CHMParticle Particle { get { return Instance.m_particle; } }
    public static CHMSkill Skill { get { return Instance.m_skill; } }
    public static CHMUnit Unit { get { return Instance.m_unit; } }
    public static CHMLevel Level { get { return Instance.m_level; } }
    public static CHMItem Item { get { return Instance.m_item; } }
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
            {
                go = new GameObject { name = "@CHMMain" };
            }

            if (Application.isPlaying)
            {
                Object.DontDestroyOnLoad(go);
            }

            m_instance = go.GetOrAddComponent<CHMMain>();

            m_instance.m_skill.Init();
            m_instance.m_unit.Init();
            m_instance.m_item.Init();
            m_instance.m_level.Init();
            m_instance.m_pool.Init();
            m_instance.m_json.Init();
            m_instance.m_particle.Init();
        }
    }

    private void OnApplicationQuit()
    {
        if (m_instance != null)
        {
            m_instance.m_skill.Clear();
            m_instance.m_unit.Clear();
            m_instance.m_item.Clear();
            m_instance.m_level.Clear();
            m_instance.m_json.Clear();
            m_instance.m_particle.Clear();

            m_instance.m_pool.Clear();
            Destroy(this);
        }
    }
}
