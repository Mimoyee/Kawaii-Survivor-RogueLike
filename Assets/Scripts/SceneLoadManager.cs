using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public string sceneNames;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(sceneNames);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void LoadGameScene()
    {
        StartCoroutine(LoadGameSceneCoroutine());
    }

    private IEnumerator LoadGameSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 确保场景完全加载后再初始化
        yield return GameManager.Instance.StartCoroutine(GameManager.Instance.Init());
        // GameManager.Instance.uiManager.Init();
        
    }
}
