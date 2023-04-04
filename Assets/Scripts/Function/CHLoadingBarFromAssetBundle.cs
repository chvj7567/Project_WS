using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CHLoadingBarFromAssetBundle : MonoBehaviour
{
    [SerializeField] Slider loadingBar;

    int remainLoadAssetBundleCount = -1;
    int totalAssetBundleCount = -1;
    float loadingPercent;

    string url = "";

    private void Awake()
    {
        loadingBar.value = 0f;
        StartCoroutine(DownloadAllAssetBundle());
    }

    IEnumerator DownloadAllAssetBundle()
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle($"{url}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Error : {request.error}");
        }
        else
        {
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] arrAssetName = manifest.GetAllAssetBundles();

            totalAssetBundleCount = remainLoadAssetBundleCount = arrAssetName.Length;

            assetBundle.Unload(false);
            foreach (string name in arrAssetName)
            {
                yield return DownloadAssetBundle(name);

                --remainLoadAssetBundleCount;

                loadingPercent = (float)(totalAssetBundleCount - remainLoadAssetBundleCount) / totalAssetBundleCount;
                loadingBar.value = loadingPercent;
            }
        }

        if (loadingPercent == 1)
        {
            Debug.Log("Loading Complete");
        }
    }

    IEnumerator DownloadAssetBundle(string _assetBundleName)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle($"{url}/{_assetBundleName}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Error : {request.error}");
        }
        else
        {
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);

            Debug.Log($"Success : {_assetBundleName}");

            assetBundle.Unload(false);
        }
    }
}
