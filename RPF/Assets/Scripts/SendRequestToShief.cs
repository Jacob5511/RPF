using UnityEngine;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SendRequestToShief : MonoBehaviour
{
    // Ссылка на сервер, куда будет отправлен запрос.
    private string serverURL = "http://22.cshse.beget.tech/add_request";

    // Ссылки на элементы интерфейса для ввода данных запроса.
    public TMP_InputField request_name, metro, price;

    // Аниматоры для отображения различных состояний запроса.
    public Animator not_allowed_same, success, all_field_must;

    // Вызывается при запуске объекта.
    private void Start()
    {
        // Устанавливаем целевую частоту кадров на 60 FPS (кадров в секунду).
        Application.targetFrameRate = 60;
    }

    // Метод для отправки запроса.
    public void UploadRequest()
    {
        // Проверяем, что все необходимые поля заполнены.
        if (request_name.text == "" || metro.text == "" || price.text == "")
        {
            // Если какое-либо поле не заполнено, запускаем анимацию "all_field_must".
            all_field_must.SetTrigger("must");
            return; // Прерываем выполнение метода.
        }

        // Если все поля заполнены, отправляем запрос.
        StartCoroutine(SendRequest());
    }

    // Метод для отправки запроса на сервер.
    private IEnumerator SendRequest()
    {
        // Создаем форму для отправки данных.
        WWWForm form = new WWWForm();

        // Загружаем информацию о пользователе из файла.
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;

        // Добавляем поля данных в форму.
        form.AddField("username", username);
        form.AddField("request_name", request_name.text);
        form.AddField("metro", metro.text);
        form.AddField("price", price.text);

        // Создаем и отправляем UnityWebRequest.
        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if (answer == "this_request_already_exist")
                {
                    // Если запрос уже существует, запускаем анимацию "not_allowed_same".
                    not_allowed_same.SetTrigger("must");
                }
                else
                {
                    // Если запрос успешно отправлен, запускаем анимацию "success" и переходим в другую сцену через 1.5 секунды.
                    success.SetTrigger("must");
                    yield return new WaitForSeconds(1.5f);
                    SceneManager.LoadScene("MainScene");
                }
                Debug.Log("Request upload successful");
            }
            else
            {
                // В случае ошибки при отправке запроса выводим сообщение в консоль.
                Debug.LogError("Server error: " + www.error + ", or you've already submitted this request");
            }
            www.Dispose();
        }
    }

}
