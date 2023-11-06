using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class MyAccount : MonoBehaviour
{
    public GameObject main_group, content_fridge_object, ingredient_prefab, scroll_fridge, scroll_responses, prefab, scroll_about_chief, request_prefab, scroll_my_requests_to_chief;
    public RectTransform content_fridge, parent_for_ingredient_prefab, parentObject;
    public List<GameObject> ingredietns = new List<GameObject>();
    string save_url = "http://22.cshse.beget.tech/save_fridge";
    string get_url = "http://22.cshse.beget.tech/get_fridge";
    string get_responses_url = "http://22.cshse.beget.tech/get_responses";
    public ScrollRect scrollRect;
    string url = "http://22.cshse.beget.tech/add_announcement";
    string get_my_requests_to_chief_url = "http://22.cshse.beget.tech/get_my_requests_to_chief";
    public RawImage chief_image;
    public TMP_Text chief_name, views, score, short_about, phone_number, metro, user_name;
    public RectTransform content, my_requests_to_chief_content;
    public AspectRatioFitter image_aspect_ratio;
    public Animator there_is_no_requests, there_is_no_responses, ingredient_saved, there_is_no_ingredient, all_fields_must;

    // Определение множества переменных для различных компонентов интерфейса и URL-адресов сервера.
    public void Load()
    {
        // Загрузка данных о шеф-поваре и отображение их в интерфейсе.
        // Включение и отключение соответствующих панелей интерфейса.
        chief_name.text = BetweenScenes.name;
        chief_image.texture = BetweenScenes.texture;
        image_aspect_ratio.aspectRatio = BetweenScenes.aspect_ratio;
        scroll_about_chief.SetActive(true);
        main_group.SetActive(false);
        scroll_responses.SetActive(false);
        scroll_fridge.SetActive(false);
        content_fridge_object.SetActive(false);
        StartCoroutine(Load_About_Chief(chief_name.text));
    }

    public IEnumerator Load_About_Chief(string ch_name)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", ch_name);
        form.AddField("is_add", "False");
        form.AddField("action", "False");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string loaded = www.downloadHandler.text;
                Chief answer_data = JsonUtility.FromJson<Chief>(loaded);

                chief_name.text = answer_data.name;
                views.text = answer_data.views;
                score.text = answer_data.score.ToString().Replace(",", ".");
                short_about.text = answer_data.short_about_chief;
                phone_number.text = answer_data.phone_number;
                metro.text = answer_data.metro;
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose();
        }
    }
    private void Start()
    {
        // Устанавливаем целевую частоту кадров приложения.
        // Получаем информацию о текущем пользователе из файла.
        Application.targetFrameRate = 60;
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        user_name.text = username;
    }
    public IEnumerator GetFridge(string username)
    {
        // Загрузка холодильника пользователя с сервера.
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(get_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if (answer != "no_results")
                {
                    string[] ing_text = answer.Split("|&");
                    for(int i = 0; i < ing_text.Length; i++)
                    {
                        string[] sp = ing_text[i].Split("&|");
                        GameObject inst = Instantiate(ingredient_prefab, parent_for_ingredient_prefab);
                        TMP_InputField name = inst.transform.GetChild(0).GetComponent<TMP_InputField>();
                        TMP_InputField quantity = inst.transform.GetChild(1).GetComponent<TMP_InputField>();
                        TMP_Text units = inst.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
                        inst.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

                        name.text = sp[0];
                        quantity.text = sp[1];
                        units.text = sp[2];
                        ingredietns.Add(inst);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(parent_for_ingredient_prefab);
                    }
                }
                else
                {
                    there_is_no_ingredient.SetTrigger("must");
                    print("У вас пока нет ни одного ингредиента");
                }
                
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }
    public void MyRequests()
    {
        // Отображение страницы "Мои запросы к шеф-повару".
        // Загрузка соответствующих запросов.
        scroll_my_requests_to_chief.SetActive(true);
        scroll_about_chief.SetActive(false);
        main_group.SetActive(false);
        scroll_responses.SetActive(false);
        scroll_fridge.SetActive(false);
        content_fridge_object.SetActive(false);
        StartCoroutine(Get_My_Requests_To_Chief(false));
    }
    public void BackFromMyRequests()
    {
        // Возврат назад из раздела "Мои запросы к шеф-повару".
        scroll_my_requests_to_chief.SetActive(false);
        scroll_about_chief.SetActive(false);
        main_group.SetActive(true);
        scroll_responses.SetActive(false);
        scroll_fridge.SetActive(false);
        content_fridge_object.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent_for_ingredient_prefab);
    }
    public IEnumerator Get_My_Requests_To_Chief(bool is_delete, string id = "-1")
    {
        // Загрузка запросов пользователя к шеф-повару с сервера.
        WWWForm form = new WWWForm();
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        form.AddField("username", user.username);
        form.AddField("id", id);
        form.AddField("is_delete", is_delete.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(get_my_requests_to_chief_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                int childCount = my_requests_to_chief_content.childCount;
                for (int i = childCount - 1; i >= 1; i--)
                {
                    Destroy(my_requests_to_chief_content.GetChild(i).gameObject);
                    print(i);
                }
                string answer = www.downloadHandler.text;
                if (answer == "no")
                {
                    print("Запросов нет");
                    there_is_no_requests.SetTrigger("must");
                }
                else
                {
                    RootObjectRequests requestData = JsonUtility.FromJson<RootObjectRequests>("{\"requests\":" + answer + "}");
                    for (int i = 0; i < requestData.requests.Length; i++)
                    {
                        RecipeDataRequests request = requestData.requests[i];
                        // Создаем экземпляр префаба
                        GameObject instantiatedPrefab = Instantiate(request_prefab, my_requests_to_chief_content);
                        instantiatedPrefab.name = request.id;
                        TMP_Text recipe_name_request = instantiatedPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        TMP_Text metro_request = instantiatedPrefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        TMP_Text price_request = instantiatedPrefab.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

                        recipe_name_request.text = request.request_name;
                        metro_request.text = request.metro;
                        price_request.text = request.price + " руб";

                        // Обновляем размер префаба в родительском объекте
                        LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
                    }
                    // Устанавливаем размер маски в соответствии с содержимым
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 1f;
                }
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }
    public void MyFridge()
    {
        // Отображение страницы "Мой холодильник".
        // Загрузка холодильника пользователя.
        scroll_my_requests_to_chief.SetActive(false);
        scroll_about_chief.SetActive(false);
        main_group.SetActive(false);
        scroll_responses.SetActive(false);
        scroll_fridge.SetActive(true);
        content_fridge_object.SetActive(true);
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        StartCoroutine(GetFridge(username));
        /*if (parent_for_ingredient_prefab.childCount == 0)
            there_is_no_ingredient.SetTrigger("must");
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent_for_ingredient_prefab);*/
    }
    public void BackFromMyFridge()
    {
        // Отображение страницы "Мой холодильник".
        // Загрузка холодильника пользователя.
        for (int i = 0; i < parent_for_ingredient_prefab.childCount; i++)
        {
            Destroy(parent_for_ingredient_prefab.GetChild(i).gameObject);
        }
        ingredietns.Clear();
        scroll_my_requests_to_chief.SetActive(false);
        scroll_about_chief.SetActive(false);
        main_group.SetActive(true);
        content_fridge_object.SetActive(false);
        scroll_responses.SetActive(false);
        scroll_fridge.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(parent_for_ingredient_prefab);
    }
    public void AddIngredient()
    {
        // Добавление нового ингредиента в холодильник.
        GameObject inst = Instantiate(ingredient_prefab, parent_for_ingredient_prefab.transform);
        ingredietns.Add(inst);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent_for_ingredient_prefab);
    }
    public void DeleteIngredient()
    {
        // Удаление последнего ингредиента из холодильника.
        Destroy(parent_for_ingredient_prefab.GetChild(parent_for_ingredient_prefab.childCount - 1).gameObject);
        ingredietns.Remove(parent_for_ingredient_prefab.GetChild(parent_for_ingredient_prefab.childCount - 1).gameObject);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent_for_ingredient_prefab);
    }
    public void Save()
    {
        // Сохранение холодильника пользователя на сервере.
        List<string> ing_text = new List<string>();
        foreach(GameObject ing in ingredietns)
        {
            TMP_InputField name = ing.transform.GetChild(0).GetComponent<TMP_InputField>();
            TMP_InputField quantity = ing.transform.GetChild(1).GetComponent<TMP_InputField>();
            TMP_Text units = ing.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
            if (name.text == "" || quantity.text == "" || units.text == "")
            {
                print("Все поля должны быть заполнены");
                all_fields_must.SetTrigger("must");
                return;
            }
            ing_text.Add(name.text + "&|" + quantity.text + "&|" + units.text);
        }
        StartCoroutine(SaveFridge(ing_text));
    }
    public void Back_From_About_Chief()
    {
        // Возврат назад из раздела информации о шеф-поваре.
        scroll_my_requests_to_chief.SetActive(false);
        scroll_fridge.SetActive(false);
        scroll_responses.SetActive(true);
        main_group.SetActive(false);
        content_fridge_object.SetActive(false);
        scroll_about_chief.SetActive(false);
    }
    public void Chief_Responses()
    {
        // Отображение страницы "Отклики к шеф-повару".
        // Загрузка соответствующих откликов.
        scroll_my_requests_to_chief.SetActive(false);
        scroll_fridge.SetActive(false);
        scroll_responses.SetActive(true);
        main_group.SetActive(false);
        content_fridge_object.SetActive(false);
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        StartCoroutine(Get_Responses(username));
        /*if (parentObject.childCount == 1)
            there_is_no_responses.SetTrigger("must");*/
        //StartCoroutine(Get_Responses());
    }
    public IEnumerator Get_Responses(string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(get_responses_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if (answer == "no")
                {
                    print("Откликов пока нет");
                    there_is_no_responses.SetTrigger("must");
                }
                else
                {
                    int childCount = parentObject.childCount;
                    for (int i = childCount - 1; i >= 1; i--)
                    {
                        Destroy(parentObject.GetChild(i).gameObject);
                    }
                    Load(answer);
                }
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }
    private IEnumerator LoadImage(string imageURL, RawImage image, Animator circle)
    {
        // Загрузка изображения по URL и установка его в компонент RawImage.
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
                circle.SetBool("is_circling", false);
                circle.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
            www.Dispose();
        }
    }
    private void Load(string json)
    {
        // Загрузка данных о шеф-поварах и отображение их в интерфейсе.
        FindChiefObject itemseData = JsonUtility.FromJson<FindChiefObject>("{\"items\":" + json + "}");
        for (int i = 0; i < itemseData.items.Length; i++)
        {
            FindChiefData item = itemseData.items[i];
            // Создаем экземпляр префаба
            GameObject instantiatedPrefab = Instantiate(prefab, parentObject);

            // Получаем доступ к компонентам RawImage и TextMeshProUGUI
            RawImage rawImageComponent = instantiatedPrefab.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
            Animator circle = instantiatedPrefab.transform.GetChild(7).GetComponent<Animator>();
            circle.SetBool("is_circling", true);
            StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(item.image_path)}", rawImageComponent, circle));
            TMP_Text chief_name = instantiatedPrefab.transform.GetChild(2).GetComponent<TMP_Text>();
            TMP_Text views = instantiatedPrefab.transform.GetChild(5).GetComponent<TMP_Text>();
            TMP_Text score = instantiatedPrefab.transform.GetChild(6).GetComponent<TMP_Text>();
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
    private IEnumerator SaveFridge(List<string> ing_text)
    {
        // Сохранение холодильника пользователя на сервере.
        WWWForm form = new WWWForm();

        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        string ingredients_string = string.Join("|&", ing_text);
        form.AddField("username", username);
        form.AddField("ingredients", ingredients_string);

        using (UnityWebRequest www = UnityWebRequest.Post(save_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if(answer == "ok")
                {
                    Debug.Log("Ингредиенты успешно сохранены");
                    ingredient_saved.SetTrigger("must");
                }
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }
}
