// SceneFader.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    // Убираем синглтон, он нам больше не нужен в таком виде
    // public static SceneFader instance; 

    [Tooltip("Панель (Image), которая будет затемнять экран")]
    public Image fadePanel;
    [Tooltip("Скорость затемнения и осветления в секундах")]
    public float fadeDuration = 1.0f;

    // --- НОВАЯ СТАТИЧЕСКАЯ ПЕРЕМЕННАЯ ДЛЯ ДОСТУПА ---
    public static SceneFader Instance { get; private set; }
    // --------------------------------------------------

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Командуем "бессмертием" самому главному родительскому объекту!
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            // Уничтожаем весь родительский объект, если дубликат
            Destroy(transform.root.gameObject);
            return;
        }
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeRoutine(sceneName));
    }

    private IEnumerator FadeRoutine(string sceneName)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 1);

        SceneManager.LoadScene(sceneName);

        // Добавим небольшую задержку перед осветлением, чтобы сцена успела прогрузиться
        yield return new WaitForSeconds(0.1f);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}