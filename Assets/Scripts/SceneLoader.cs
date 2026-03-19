using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private AsyncOperation scene;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task LoadScene(int sceneName)
    {
        scene = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("SceneLoadStart");
        scene.allowSceneActivation = false;

        // loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(1);
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
    }

    public async void LoadFinished()
    {
        await Task.Delay(100);
        Debug.Log("SceneLoadDone");

    }
}
