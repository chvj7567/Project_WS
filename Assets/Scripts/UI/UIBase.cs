using Unity.Collections;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [ReadOnly]
    public Defines.EUI uiType;
    [ReadOnly]
    public int uid = 0;

    private void Awake()
    {
        GameObject _canvas = GameObject.FindGameObjectWithTag("UICanvas");
        if (_canvas)
        {
            transform.SetParent(_canvas.transform, false);
        }
    }

    public virtual void InitUI(UIArg _uiArg) { }

    public virtual void CloseUI() { }
}
