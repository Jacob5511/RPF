using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;
public class LoadAllRequests : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/load_all_requests";
    public GameObject prefab, loading_screen, scroll;
    public Animator empty_request_text, no_results, no_results_text, loading_screen_anim, no_internet, open_panel;
    public RectTransform parentObject;
    TextMeshProUGUI recipe_name, metro, price;
    public TMP_InputField search_bar, price_from, price_to, metro_input;
    public GameManager game_manager;
    public ScrollRect scrollRect;
    public Animation animation_script;

    private void Start()
    {
        Application.targetFrameRate = 60;
        search_bar.text = BetweenScenes.search_bar;
        StartCoroutine(Check_And_Load());
    }

    private IEnumerator Check_And_Load()
    {
        // Проверка наличия интернет-соединения
        yield return StartCoroutine(game_manager.CheckInternetConnection(isConnected =>
        {
            if (!isConnected)
            {
                Debug.Log("Internet Not Available");
                no_internet.SetTrigger("must");
            }
        }));

        // Восстановление данных поиска
        search_bar.text = BetweenScenes.search_data;
        price_from.text = BetweenScenes.price_from;
        price_to.text = BetweenScenes.price_to;
        metro_input.text = BetweenScenes.metro;

        // Загрузка рецептов
        StartCoroutine(Main_Load_Ricipes());
    }

    private IEnumerator Main_Load_Ricipes()
    {
        WWWForm form = new WWWForm();
        form.AddField("search_data", search_bar.text);
        form.AddField("price_from", price_from.text);
        form.AddField("price_to", price_to.text);
        form.AddField("metro", metro_input.text);

        // Отправка POST-запроса для получения рецептов
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;

                if (json == "no_results")
                {
                    // Обработка случая, когда нет результатов
                    no_results.Play("No_Results");
                    no_results_text.Play("No_Results_text");
                }
                else
                {
                    // Очистка существующих рецептов
                    int childCount = parentObject.childCount;
                    for (int i = childCount - 1; i >= 0; i--)
                    {
                        Destroy(parentObject.GetChild(i).gameObject);
                    }
                    loading_screen.SetActive(false);
                    loading_screen_anim.SetTrigger("must_close");
                    scroll.SetActive(true);

                    // Загрузка и отображение новых рецептов
                    Load(json);
                }
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose();
        }
    }

    public void Send_Data()
    {
        // Отправка данных для поиска
        search_bar.text = search_bar.text.Trim();
        if (open_panel.GetBool("is_open"))
        {
            animation_script.Open_Panel();
        }
        if (search_bar.text.Length == 0)
        {
            empty_request_text.Play("Empty_Request");
        }
        StartCoroutine(Main_Load_Ricipes());
    }

    public void SendSort()
    {
        // Отправка данных для сортировки и загрузка рецептов
        //open_panel.SetBool("is_open", false);
        animation_script.Open_Panel();
        StartCoroutine(Main_Load_Ricipes());
    }

    private void Load(string json)
    {
        // Загрузка и отображение рецептов на основе полученных данных
        RootObjectRequests requestData = JsonUtility.FromJson<RootObjectRequests>("{\"requests\":" + json + "}");
        for (int i = 0; i < requestData.requests.Length; i++)
        {
            RecipeDataRequests request = requestData.requests[i];

            // Создаем экземпляр префаба
            GameObject instantiatedPrefab = Instantiate(prefab, parentObject);
            instantiatedPrefab.name = request.id;
            recipe_name = instantiatedPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            metro = instantiatedPrefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            price = instantiatedPrefab.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

            recipe_name.text = request.request_name;
            metro.text = request.metro;
            price.text = request.price + " руб";

            // Обновляем размер префаба в родительском объекте
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
        }

        // Устанавливаем размер маски в соответствии с содержимым
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

}



[Serializable]
public class RecipeDataRequests
{
    public string id;           // Уникальный идентификатор рецепта
    public string request_name; // Название рецепта
    public string metro;        // Метро, связанное с рецептом
    public string price;        // Стоимость рецепта
}

[Serializable]
public class RootObjectRequests
{
    public RecipeDataRequests[] requests; // Массив данных рецептов, содержащихся в корневом объекте
}

