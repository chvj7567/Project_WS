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
    [SerializeField] Image loadingBar;
    [SerializeField] TMP_Text loadingText;
    [SerializeField] Button startBtn;
    [SerializeField] List<string> liDownloadKey = new List<string>();

    bool canStart = false;
    int downloadCount = 0;

    private void Start()
    {
        startBtn.gameObject.SetActive(true);

        if (loadingBar) loadingBar.fillAmount = 0f;
        if (startBtn)
        {
            startBtn.OnClickAsObservable().Subscribe(_ =>
            {
                if (loadingBar.fillAmount == 1f)
                {
                    SceneManager.LoadScene(1);
                }
            });
        }

        if (CHMAssetBundle.Instance.firstDownload == true)
        {
            foreach (var key in liDownloadKey)
            {
                StartCoroutine(LoadAssetBundle(key));
            }
        }
        else
        {
            canStart = true;
            loadingBar.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(false);
        }
    }

    IEnumerator LoadAssetBundle(string _bundleName)
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, _bundleName);

        Debug.Log(bundlePath);

        // 에셋 번들 로드
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);

        // 다운로드 표시
        float downloadProgress = 0;

        while (!bundleRequest.isDone)
        {
            downloadProgress = bundleRequest.progress;

            if (loadingBar) loadingBar.fillAmount = downloadProgress;
            if (loadingText) loadingText.text = downloadProgress / liDownloadKey.Count * downloadCount * 100f+ "%";

            yield return null;
        }

        downloadProgress = bundleRequest.progress;

        AssetBundle assetBundle = bundleRequest.assetBundle;

        CHMAssetBundle.Instance.LoadAssetBundle(_bundleName, assetBundle);

        ++downloadCount;

        if (loadingBar) loadingBar.fillAmount = downloadProgress;
        if (loadingText) loadingText.text = downloadProgress / liDownloadKey.Count * downloadCount * 100f + "%";

        if (downloadCount == liDownloadKey.Count)
        {
            CHMMain.UI.CreateEventSystemObject();
            yield return CHMData.Instance.LoadLocalData("AA");

            canStart = true;
            CHMAssetBundle.Instance.firstDownload = false;
        }
    }
}
