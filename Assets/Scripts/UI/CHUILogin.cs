/*using System.Collections;
using System.Collections.Generic;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CHUILogin : UIBase
{
    public TMP_InputField emailField;
    public TMP_InputField passwardField;
    public Button loginBtn;

    void Start()
    {
        emailField.Select();
        loginBtn.OnClickAsObservable().Subscribe(_ =>
        {
            SHMFirebase.Instance.Login(emailField.text, passwardField.text);
        });
    }

    public override void InitUI(UIArg _uiArg)
    {
        base.InitUI(_uiArg);
    }
}
*/