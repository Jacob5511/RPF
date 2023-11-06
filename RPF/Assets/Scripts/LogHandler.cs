using UnityEngine;
using UnityEngine.SceneManagement;

public class LogHandler : MonoBehaviour
{
    public Animator anim; // Ссылка на компонент Animator для управления анимацией
    private GameObject loading_screen; // Ссылка на объект экрана загрузки

    // Вызывается при включении объекта
    private void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>(); // Получаем компонент Animator
        Application.logMessageReceived += HandleException; // Регистрируем функцию HandleException для обработки сообщений об ошибках
    }

    // Вызывается при выключении объекта
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleException; // Отменяем регистрацию функции HandleException
    }

    // Функция для обработки сообщений об ошибках и исключениях
    void HandleException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            anim.SetTrigger("must"); // Запускаем анимацию ошибки
            if (logString.ToLower().Contains("gateway 502"))
            {
                string current_scene = SceneManager.GetActiveScene().name; // Получаем имя текущей сцены
                SceneManager.LoadScene(current_scene); // Перезагружаем текущую сцену
            }
            try
            {
                loading_screen = GameObject.FindGameObjectWithTag("loading_screen"); // Пытаемся найти объект экрана загрузки
                if (loading_screen.activeSelf)
                {
                    string current_scene = SceneManager.GetActiveScene().name; // Получаем имя текущей сцены
                    SceneManager.LoadScene(current_scene); // Перезагружаем текущую сцену
                }
            }
            catch
            {
                // Пустой блок catch для обработки исключений, если что-то пошло не так
            }
        }
    }
}