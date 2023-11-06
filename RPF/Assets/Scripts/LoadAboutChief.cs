using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LoadAboutChief : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/add_announcement";
    public RawImage chief_image; // Поле для отображения изображения шеф-повара
    public TMP_Text chief_name, views, score, short_about, phone_number, metro; // Поля для текстовой информации о шеф-поваре
    public RectTransform content; // RectTransform, используемый для управления макетом контента
    public AspectRatioFitter image_aspect_ratio; // Компонент для управления соотношением сторон изображения

    private void Start()
    {
        Application.targetFrameRate = 60;
        chief_name.text = BetweenScenes.name; // Устанавливаем имя шеф-повара
        chief_image.texture = BetweenScenes.texture; // Устанавливаем изображение шеф-повара
        image_aspect_ratio.aspectRatio = BetweenScenes.aspect_ratio; // Устанавливаем соотношение сторон изображения
        Load(); // Вызываем метод Load() для загрузки информации о шеф-поваре
    }

    public void Load()
    {
        StartCoroutine(Load_About_Chief()); // Запускаем загрузку информации о шеф-поваре
    }

    private IEnumerator Load_About_Chief()
    {
        // Создаем форму для отправки данных на сервер
        WWWForm form = new WWWForm();
        form.AddField("username", chief_name.text); // Отправляем имя шеф-повара
        form.AddField("is_add", "False");
        form.AddField("action", "False");

        // Отправляем POST-запрос на указанный URL
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest(); // Ждем завершения запроса

            if (www.result == UnityWebRequest.Result.Success)
            {
                string loaded = www.downloadHandler.text; // Получаем ответ от сервера и разбираем его в объект Chief

                Chief answer_data = JsonUtility.FromJson<Chief>(loaded);

                // Обновляем текстовую информацию о шеф-поваре
                chief_name.text = answer_data.name;
                views.text = answer_data.views;
                score.text = answer_data.score.ToString().Replace(",", ".");
                short_about.text = answer_data.short_about_chief;
                phone_number.text = answer_data.phone_number;
                metro.text = answer_data.metro;

                LayoutRebuilder.ForceRebuildLayoutImmediate(content); // Перестраиваем макет контента
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose(); // Освобождаем ресурсы, связанные с запросом
        }
    }
}
