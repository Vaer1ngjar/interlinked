// InitializationLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationLoader : MonoBehaviour
{
    void Start()
    {
        // Сразу же после запуска этой сцены, загружаем Главное Меню
        SceneManager.LoadScene("Menu"); // Убедись, что имя сцены точное!
    }
}