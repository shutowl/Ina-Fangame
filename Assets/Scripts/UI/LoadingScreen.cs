using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI loadText;

    // The name of the scene to load
    public string sceneName;

    void Start()
    {
        progressBar.value = 0;
        loadText.text = "Loading...";
        sceneName = PlayerPrefs.GetString("nextScene");
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        //asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Update the progress bar UI
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
    }
}
