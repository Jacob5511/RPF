using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;

public class Authorization : MonoBehaviour
{
    private const string enterURL = "http://22.cshse.beget.tech/enter"; // URL для входа пользователя.
    private const string registerURL = "http://22.cshse.beget.tech/registration"; // URL для регистрации пользователя.
    private const string getHashURL = "http://22.cshse.beget.tech/get_hash"; // URL для получения хеша пароля.

    public TMP_InputField username_reg, password_reg, repeat_password_reg, email_reg, username_ent, password_ent; // Поля для ввода имени пользователя, пароля и т. д.
    public GameObject error; // Игровой объект для отображения ошибок.
    public Animator all_fields_must, wrong_user, wrong_password, this_user_exist; // Аниматоры для анимации ошибок и уведомлений.
    public int count; // Счетчик попыток входа.
    public Toggle is_chief; // Переключатель для указания, является ли пользователь шеф-поваром.
    string got_hash; // Полученный хеш пароля.
    

    private void Start()
    {
        Application.targetFrameRate = 60;
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        count = user.count;
        if (count < 0)
            StartCoroutine(SendRequest(user.username, user.hash, enterURL));
    }

    public void Register()
    {
        // Регистрация пользователя.
        if (username_reg.text.Trim() != "" && password_reg.text.Trim() != "" && repeat_password_reg.text.Trim() != "" && email_reg.text.Trim() != "")
        {
            if (password_reg.text != repeat_password_reg.text)
                error.SetActive(true);
            else
            {
                string email_name = email_reg.text.Split('@')[0];
                if (email_reg.text.Trim() == "" || email_name == email_reg.text || "_.-".Contains(email_name[0]) || "_.-".Contains(email_name[email_name.Length - 1]))
                    print("Некорректно заполнены данные");
                else
                    StartCoroutine(SendRequest(username_reg.text, HashPassword(password_reg.text), registerURL));
            }
        }
        else
        {
            all_fields_must.SetTrigger("must");
        }
    }

    public void Enter()
    {
        // Вход пользователя.
        if (username_ent.text.Trim() == "" || password_ent.text.Trim() == "")
        {
            all_fields_must.SetTrigger("must");
        }
        else
        {
            StartCoroutine(GetHash(username_ent.text));
        }
    }

    public IEnumerator GetHash(string username)
    {
        // Получение хеша пароля для входа.
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(getHashURL, form))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                var answer = www.downloadHandler.text;

                if (answer == "no")
                {
                    wrong_user.SetTrigger("open");
                }
                else
                {
                    got_hash = answer;

                    if (HashPassword(password_ent.text) != got_hash)
                    {
                        wrong_password.SetTrigger("open");
                    }
                    else
                    {
                        StartCoroutine(SendRequest(username_ent.text, HashPassword(password_ent.text), enterURL));
                    }
                }
            }
            else
            {
                print(www.error);
            }
            www.Dispose();
        }
    }

    public IEnumerator SendRequest(string username, string hash, string url)
    {
        // Отправка запроса на сервер Flask для регистрации или входа.
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("hash", hash);
        form.AddField("email", email_reg.text);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var answer = www.downloadHandler.text;
                if (answer == "no")
                {
                    print("Что-то пошло не так. Попробуйте заново");
                    this_user_exist.SetTrigger("must");
                }
                else
                {
                    User user = new User();
                    user.username = username;
                    user.hash = hash;
                    user.email = email_reg.text;
                    if (count >= 50) count = 0;
                    user.count = count + 1;
                    string json = JsonUtility.ToJson(user);
                    File.WriteAllText(Path.Combine(Application.persistentDataPath, "user.txt"), json);
                    BetweenScenes.name = username;
                    if (is_chief.isOn)
                        SceneManager.LoadScene("ForChief");
                    else
                        SceneManager.LoadScene("MainScene");
                }
            }
            else
            {
                Debug.LogError("Authorization failed: " + www.error);
            }
            www.Dispose();
        }
    }

    public static string HashPassword(string password)
    {
        // Хеширование пароля с использованием SHA-256.
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

}

