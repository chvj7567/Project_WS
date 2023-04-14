using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    void Update()
    {
        if (CHMMain.Json.GetJsonLoadingPercent() >= 99.9999f)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
