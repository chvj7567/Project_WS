using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class First : MonoBehaviour
{
    [SerializeField] Button startBtn;
    [SerializeField] CHLoadingBarFromAssetBundle loadingScript;
    bool canStart = false;

    private async void Start()
    {
        await CHMData.Instance.LoadLocalData("Defense");
        startBtn.gameObject.SetActive(true);

        if (startBtn)
        {
            startBtn.OnClickAsObservable().Subscribe(_ =>
            {
                if (canStart)
                {
                    SceneManager.LoadScene(1);
                }
            });
        }

        loadingScript.bundleLoadSuccess += () =>
        {
            Debug.Log("Can Start");
            canStart = true;
        };

        loadingScript.Init();
    }
}
