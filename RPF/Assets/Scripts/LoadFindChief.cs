using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LoadFindChief : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/find_chief";
    public TMP_InputField search_bar, rating_from_one, rating_from_two, rating_to_one, rating_to_two, views_from, views_to, metro;
    public GameObject prefab, scroll, loading_screen;
    public Animator empty_request_text, no_results, no_results_text, loading_screen_anim, open_panel;
    public Animation animation_script;
    public RectTransform parentObject;
    RawImage rawImageComponent;
    TMP_Text chief_name, views, score, phone_number;

    public ScrollRect scrollRect;

    private void Start()
    {
        Application.targetFrameRate = 60;

        // Устанавливаем начальные значения полей поиска на основе предыдущих результатов
        search_bar.text = BetweenScenes.search_bar_find_chief;
        rating_from_one.text = BetweenScenes.rating_from_one_find_chief;
        rating_from_two.text = BetweenScenes.rating_from_two_find_chief;
        rating_to_one.text = BetweenScenes.rating_to_one_find_chief;
        rating_to_two.text = BetweenScenes.rating_to_two_find_chief;
        views_from.text = BetweenScenes.views_from_find_chief;
        views_to.text = BetweenScenes.views_to_find_chief;
        metro.text = BetweenScenes.metro_find_chief;
        StartCoroutine(Main_Load_Ricipes());
    }

    private IEnumerator Main_Load_Ricipes()
    {
        // Основной метод для загрузки данных о шеф-поварах
        WWWForm form = new WWWForm();
        form.AddField("rating_from_one", rating_from_one.text);
        form.AddField("rating_from_two", rating_from_two.text);
        form.AddField("rating_to_one", rating_to_one.text);
        form.AddField("rating_to_two", rating_to_two.text);
        form.AddField("views_from", views_from.text);
        form.AddField("views_to", views_to.text);
        form.AddField("search_data", search_bar.text);
        form.AddField("metro", metro.text);
        // Создание формы для отправки критериев поиска на сервер

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                // Если запрос был успешным, получаем JSON-данные

                print(json);
                if (json == "no_results")
                {
                    no_results.Play("No_Results");
                    no_results_text.Play("No_Results_text");
                    // Если нет результатов, проигрываем анимации отсутствия результатов
                }
                else
                {
                    int childCount = parentObject.childCount;
                    for (int i = childCount - 1; i >= 0; i--)
                    {
                        Destroy(parentObject.GetChild(i).gameObject);
                    }
                    // Удаляем существующие элементы списка
                    loading_screen.SetActive(false);
                    loading_screen_anim.SetTrigger("must_close");
                    scroll.SetActive(true);
                    // Отключаем экран загрузки и отображаем элементы списка
                    Load(json);
                    // Загружаем данные о шеф-поварах из JSON
                }
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose();
        }
    }
    public void Filter()
    {
        // Запускаем анимацию открытия панели с фильтрами
        animation_script.Open_Panel();
        //open_panel.SetBool("is_open", false);

        // Выполняем загрузку шеф-поваров с учетом фильтров
        StartCoroutine(Main_Load_Ricipes());
    }
    public void Send_Data()
    {
        // Удаляем начальные и конечные пробелы из строки поиска
        search_bar.text = search_bar.text.Trim();
        if (search_bar.text.Length == 0)
        {
            // Если строка поиска пуста, проигрываем анимацию "Пустой запрос"
            empty_request_text.Play("Empty_Request");
        }
        if (animation_script.sort_panel_anim.GetBool("is_open"))
        {
            // Если строка поиска пуста, проигрываем анимацию "Пустой запрос"
            animation_script.Open_Panel();
        }

        // Выполняем загрузку шеф-поваров
        StartCoroutine(Main_Load_Ricipes());
    }

    // Метод для загрузки изображения по URL
    private IEnumerator LoadImage(string imageURL, RawImage image, Animator circle)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Если загрузка изображения прошла успешно, устанавливаем текстуру и соотношение сторон
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                float c = (texture.width * 1.0f) / texture.height;
                AspectRatioFitter image_aspect = image.GetComponent<AspectRatioFitter>();
                image_aspect.aspectRatio = c;
                image.texture = texture;
                circle.SetBool("is_circling", false);
                circle.gameObject.SetActive(false);
            }
            else
            {
                // В случае ошибки выводим сообщение в лог
                Debug.LogError("Failed to load image: " + www.error);
            }
            // В случае ошибки выводим сообщение в лог
            www.Dispose();
        }
    }
    // Метод для загрузки данных о шеф-поварах и создания соответствующих элементов
    private void Load(string json)
    {
        FindChiefObject itemseData = JsonUtility.FromJson<FindChiefObject>("{\"items\":" + json + "}");
        for (int i = 0; i < itemseData.items.Length; i++)
        {
            FindChiefData item = itemseData.items[i];
            // Создаем экземпляр префаба
            GameObject instantiatedPrefab = Instantiate(prefab, parentObject);

            // Получаем доступ к компонентам RawImage и TextMeshProUGUI
            rawImageComponent = instantiatedPrefab.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
            Animator circle = instantiatedPrefab.transform.GetChild(7).GetComponent<Animator>();
            circle.SetBool("is_circling", true);
            StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(item.image_path)}", rawImageComponent, circle));
            chief_name = instantiatedPrefab.transform.GetChild(2).GetComponent<TMP_Text>();
            views = instantiatedPrefab.transform.GetChild(5).GetComponent<TMP_Text>();
            score = instantiatedPrefab.transform.GetChild(6).GetComponent<TMP_Text>();
            chief_name.text = item.chief_name;
            score.text = item.score;
            views.text = item.views;

            // Обновляем размер префаба в родительском объекте
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
        }
        // Устанавливаем размер маски в соответствии с содержимым
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}

[Serializable]
public class FindChiefData
{
    public string image_path; // Путь к изображению шеф-повара
    public string chief_name; // Имя шеф-повара
    public string views; // Количество просмотров профиля шеф-повара
    public string score; // Оценка шеф-повара
}

[Serializable]
public class FindChiefObject
{
    public FindChiefData[] items; // Массив данных о шеф-поварах
}
