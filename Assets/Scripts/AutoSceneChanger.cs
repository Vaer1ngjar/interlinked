// AutoSceneChanger.cs
using UnityEngine;
using System.Collections;

public class AutoSceneChanger : MonoBehaviour
{
    public float delayInSeconds = 2.0f;
    public string nextSceneName;

    void Start()
    {
        StartCoroutine(WaitAndChangeScene());
    }

    private IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(delayInSeconds);

        // Обращаемся к новому свойству Instance
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(nextSceneName);
        }
        else
        {
            Debug.LogError("SceneFader не найден на сцене! Переход невозможен.");
            // UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}