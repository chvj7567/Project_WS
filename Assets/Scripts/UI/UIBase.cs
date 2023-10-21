using System;
using UniRx;
using Unity.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    [ReadOnly]
    public Defines.EUI eUIType;
    [ReadOnly]
    public int uid = 0;

    [SerializeField] Button backgroundBtn;
    [SerializeField] Button backBtn;

    protected Action backAction;

    private void Awake()
    {
        GameObject _canvas = GameObject.FindGameObjectWithTag("UICanvas");
        if (_canvas)
        {
            transform.SetParent(_canvas.transform, false);
        }

        backAction += () =>
        {
            Debug.Log($"{eUIType} exit");
        };

        if (backgroundBtn)
        {
            backgroundBtn.OnClickAsObservable().Subscribe(_ =>
            {
                backAction.Invoke();
                CHMMain.UI.CloseUI(gameObject);
            });
        }

        if (backBtn)
        {
            backBtn.OnClickAsObservable().Subscribe(_ =>
            {
                backAction.Invoke();
                CHMMain.UI.CloseUI(gameObject);
            });
        }
    }

    public virtual void InitUI(CHUIArg _uiArg) { }

    public virtual void CloseUI() { }
}
