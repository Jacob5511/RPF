using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;
public class LoadShortChief : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/add_announcement";
    public RectTransform pattern_2, gameobject, content;
    public RawImage foto_1, foto_2;
    public RectTransform area;
    public TMP_Text chief_name_1, views_1, score_1, chief_name_2, views_2, score_2, short_about_chief, phone_number, metro;
    public Animator you_are_added, you_are_removed, loading_screen_anim;
    public ScrollRect scrollRect;
    public GameObject send_add, send_remove, loading_screen, scroll;

    private void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(Main_Load_Ricipes(false));
    }

    // Функция для обновления высоты паттерна и игрового объекта
    private void UpdatePatterHeight(float newHeight)
    {
        Vector2 newSize = pattern_2.sizeDelta;
        newSize.y = newHeight;
        pattern_2.sizeDelta = newSize;
        gameobject.sizeDelta = newSize;
    }

    // Загрузка изображения по URL и установка в компонент RawImage
    private IEnumerator LoadImage(string imageURL, RawImage image)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {

                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                float c = (texture.width * 1.0f) / texture.height;
                AspectRatioFitter image_aspect = image.GetComponent<AspectRatioFitter>();
                image_aspect.aspectRatio = c;
                image.texture = texture;
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
            www.Dispose();
        }
    }
    public void Send()
    {
        StartCoroutine(Main_Load_Ricipes(true, "1"));
    }
    public void Send_For_Remove()
    {
        StartCoroutine(Main_Load_Ricipes(true, "0"));
    }

    // Установка данных о шефе
    private void Assign(Chief answer_data)
    {
        chief_name_1.text = answer_data.name;
        views_1.text = answer_data.views;
        score_1.text = answer_data.score.ToString();
        chief_name_2.text = answer_data.name;
        views_2.text = answer_data.views;
        score_2.text = answer_data.score.ToString().Replace(",", ".");
        short_about_chief.text = answer_data.short_about_chief;
        phone_number.text = answer_data.phone_number;
        metro.text = answer_data.metro;
    }

    // Отправка данных на сервер и получение ответа
    private IEnumerator Main_Load_Ricipes(bool is_add, string action = "")
    {
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);

        WWWForm form = new WWWForm();
        form.AddField("username", user.username);
        form.AddField("is_add", is_add.ToString());
        form.AddField("action", action);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                loading_screen.SetActive(false);
                loading_screen_anim.SetTrigger("must_close");
                scroll.SetActive(true);
                if (answer == "add")
                {
                    if (action == "1")
                    {
                        you_are_added.SetTrigger("must");
                        send_add.SetActive(false);
                        send_remove.SetActive(true);
                    }
                    else if (action == "0")
                    {
                        you_are_removed.SetTrigger("must");
                        send_add.SetActive(true);
                        send_remove.SetActive(false);
                    }
                }
                else
                {
                    Chief answer_data = JsonUtility.FromJson<Chief>(answer);

                    StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(answer_data.foto_path)}", foto_1));
                    StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(answer_data.foto_path)}", foto_2));
                    Assign(answer_data);

                    LayoutRebuilder.ForceRebuildLayoutImmediate(area);
                    Canvas.ForceUpdateCanvases();

                    UpdatePatterHeight(area.rect.height);

                    LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                    Canvas.ForceUpdateCanvases();
                    if (answer_data.add_or_remove == "1")
                    {
                        send_add.SetActive(false);
                        send_remove.SetActive(true);
                    }
                    else
                    {
                        send_add.SetActive(true);
                        send_remove.SetActive(false);
                    }
                }
                
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose();
        }
    }
}


