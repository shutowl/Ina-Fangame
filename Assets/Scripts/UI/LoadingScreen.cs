using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image progressBar;

    // The name of the scene to load
    public string sceneName;

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // Update the progress bar UI
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.fillAmount = progress;

            yield return null;
        }
    }
}
