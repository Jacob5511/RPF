using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LoadRecipes : MonoBehaviour
{
    string main_url = "http://22.cshse.beget.tech/main_url";
    public GameObject prefab, loading_screen;
    public Animator empty_request_text, no_results, no_results_text, open_panel, what_you_can_filter, loading_screen_anim, no_internet, there_is_no_ingredient, there_is_no_downloaded_recipes;
    public RectTransform parentObject;
    RawImage rawImageComponent;
    TextMeshProUGUI recipe_name, score, views;
    public Toggle can_i_make_recipe, sort_by_views, is_my_recipes, downloaded_recipes;
    public TMP_InputField search_bar, rating_from_one, rating_from_two, rating_to_one, rating_to_two, views_from, views_to;
    //int displayed_recipes_count = 0;
    //string old_request = "";
    public ScrollRect scrollRect;
    int count;
    public GameManager game_manager;
    public Animation animation_script;
    private void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(Check_And_Load());
    }

    private IEnumerator Check_And_Load()
    {
        // Проверяем интернет-соединение
        yield return StartCoroutine(game_manager.CheckInternetConnection(isConnected =>
        {
            if (!isConnected)
            {
                Debug.Log("Internet Not Available");
                BetweenScenes.is_downloaded = true;
                no_internet.SetTrigger("must");
            }
        }));

        // Проверяем интернет-соединение
        search_bar.text = BetweenScenes.search_bar;
        rating_from_one.text = BetweenScenes.rating_from_one;
        rating_from_two.text = BetweenScenes.rating_from_two;
        rating_to_one.text = BetweenScenes.rating_to_one;
        rating_to_two.text = BetweenScenes.rating_to_two;
        views_from.text = BetweenScenes.views_from;
        views_to.text = BetweenScenes.views_to;
        can_i_make_recipe.isOn = BetweenScenes.can_i_make_recipe;
        sort_by_views.isOn = BetweenScenes.sort_by_views;
        is_my_recipes.isOn = BetweenScenes.is_my_recipes;
        downloaded_recipes.isOn = BetweenScenes.is_downloaded;

        if (downloaded_recipes.isOn)
        {
            loading_screen_anim.SetTrigger("must_close");
            loading_screen.SetActive(false);
        }

        // Загружаем рецепты
        StartCoroutine(Main_Load_Ricipes());
    }
    public void Change_Input()
    {
        // Если меняется ввод, сбрасываем сортировку по просмотрам
        sort_by_views.isOn = false;
    }
    public void Change_Toggle()
    {
        if(sort_by_views.isOn)
        {
            // Если включается сортировка по просмотрам, сбрасываем фильтры по просмотрам
            views_from.text = "";
            views_to.text = "";
        }
    }
    public IEnumerator Main_Load_Ricipes(string index_last_recipe = "-1")
    {
        string new_request = search_bar.text.Trim() + rating_from_one.text + rating_from_two.text + rating_to_one.text + rating_to_two.text + views_from.text + views_to.text + Convert.ToInt32(can_i_make_recipe.isOn) +
            +Convert.ToInt32(sort_by_views.isOn) + Convert.ToInt32(is_my_recipes.isOn) + Convert.ToInt32(downloaded_recipes.isOn);
        if (BetweenScenes.old_request != new_request)
        {
            // Если изменился запрос, сбрасываем счетчик страниц и позицию скролла
            count = 0;
            BetweenScenes.old_request = new_request;
            BetweenScenes.vertical_normalized_position = 1f;
        }
        if (index_last_recipe != "-1")
        {
            if (parentObject.childCount / 25 == count)
                count += 1;
            else if (parentObject.childCount / 25 > count)
                count = parentObject.childCount / 25;
        }
        if (downloaded_recipes.isOn)
        {
            if (index_last_recipe == "-1")
            {
                int childCount = parentObject.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    Destroy(parentObject.GetChild(i).gameObject);
                }
            }
            Load_Downloaded_Recipes(count, index_last_recipe);
        }
        else
        {
            // Запрос к серверу для получения рецептов
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
            User user = JsonUtility.FromJson<User>(json);
            WWWForm form = new WWWForm();
            form.AddField("rating_from_one", rating_from_one.text);
            form.AddField("rating_from_two", rating_from_two.text);
            form.AddField("rating_to_one", rating_to_one.text);
            form.AddField("rating_to_two", rating_to_two.text);
            form.AddField("views_from", views_from.text);
            form.AddField("views_to", views_to.text);
            form.AddField("can_i_make_recipe", Convert.ToInt32(can_i_make_recipe.isOn));
            form.AddField("sort_by_views", Convert.ToInt32(sort_by_views.isOn));
            form.AddField("my_recipes", Convert.ToInt32(is_my_recipes.isOn));
            form.AddField("search_data", search_bar.text);
            form.AddField("username", user.username);
            form.AddField("index_last_recipe", index_last_recipe);
            form.AddField("count", count);

            using (UnityWebRequest www = UnityWebRequest.Post(main_url, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string answer = www.downloadHandler.text;
                    if (answer == "no_results")
                    {
                        no_results.Play("No_Results");
                        no_results_text.Play("No_Results_text");
                    }
                    else if (answer == "you_have_not_ingredients")
                    {
                        print("у тебя нет ингредиентов в холодильнике");
                        there_is_no_ingredient.SetTrigger("must");
                    }
                    else
                    {
                        if (index_last_recipe == "-1")
                        {
                            int childCount = parentObject.childCount;
                            for (int i = childCount - 1; i >= 0; i--)
                            {
                                Destroy(parentObject.GetChild(i).gameObject);
                            }
                        }
                        loading_screen_anim.SetTrigger("must_close");
                        loading_screen.SetActive(false);
                        Load(answer, index_last_recipe);
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
    public void Filter()
    {
        count = 0;
        //open_panel.SetBool("is_open", false);
        animation_script.Open_Panel();
        if (downloaded_recipes.isOn)
            what_you_can_filter.SetTrigger("must");
        StartCoroutine(Main_Load_Ricipes());
    }
    public void Send_Data()
    {
        count = 0;
        search_bar.text = search_bar.text.Trim();
        if (search_bar.text.Length == 0)
        {
            empty_request_text.Play("Empty_Request");
        }
        if (open_panel.GetBool("is_open"))
        {
            animation_script.Open_Panel();
        }
        StartCoroutine(Main_Load_Ricipes());
    }
    private void Load_Downloaded_Recipes(int count, string index_last_recipe = "-1")
    {
        string path = Path.Combine(Application.persistentDataPath, "recipes");
        string images_save_path = Path.Combine(path, "images");
        string all_except_images_save_path = Path.Combine(path, "text_info.txt");
        if (!File.Exists(all_except_images_save_path))
        {
            print("У вас нет скачанных файлов");
            there_is_no_downloaded_recipes.SetTrigger("must");
            return;
        }
        string json = File.ReadAllText(all_except_images_save_path);
        DownloadRecipeObject download_recipe_object = JsonUtility.FromJson<DownloadRecipeObject>(json);

        if(download_recipe_object.recipes.Length == 0)
        {
            print("У вас нет скачанных файлов");
            there_is_no_downloaded_recipes.SetTrigger("must");
            return;
        }
        // Делаем фильтрацию по имени рецепта
        string prompt = search_bar.text.Trim();
        if (prompt.Length != 0)
        {
            List<DownloadRecipeData> recipeList = new List<DownloadRecipeData>();

            for (int i = 0; i < download_recipe_object.recipes.Length; i++)
            {
                DownloadRecipeData recipe = download_recipe_object.recipes[i];
                if (recipe.recipe_name.ToLower().Contains(prompt.ToLower()) || recipe.ingredients.ToLower().Contains(prompt.ToLower()))
                {
                    recipeList.Add(recipe);
                }
            }
            download_recipe_object.recipes = recipeList.ToArray();
            if (download_recipe_object.recipes.Length == 0)
            {
                no_results.Play("No_Results");
                no_results_text.Play("No_Results_text");
            }
        }

        for (int i = count * 25; i < (count + 1) * 25; i++)
        {
            if (i >= download_recipe_object.recipes.Length)
            {
                break;
            }
            DownloadRecipeData recipe = download_recipe_object.recipes[i];
            // Создаем экземпляр префаба
            GameObject instantiatedPrefab = Instantiate(prefab, parentObject);
            instantiatedPrefab.transform.GetChild(7).name = Path.GetFileName(recipe.main_picture);
            // Получаем доступ к компонентам RawImage и TextMeshProUGUI
            rawImageComponent = instantiatedPrefab.transform.GetChild(6).GetChild(0).GetComponent<RawImage>();
            GameObject circle_object = instantiatedPrefab.transform.GetChild(8).gameObject;
            circle_object.SetActive(false);
            string path_for_each_image = Path.Combine(images_save_path, Path.GetFileName(recipe.main_picture));
            StartCoroutine(LoadDownloadedImage(path_for_each_image, rawImageComponent));
            
            recipe_name = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
            views = instantiatedPrefab.GetComponentsInChildren<TextMeshProUGUI>()[1];
            score = instantiatedPrefab.GetComponentsInChildren<TextMeshProUGUI>()[2];
            instantiatedPrefab.name = recipe.recipe_id;
            recipe_name.text = recipe.recipe_name;
            score.text = recipe.score;
            views.text = recipe.views;
            // Обновляем размер префаба в родительском объекте
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
        }
        if (index_last_recipe == "-1")
        {
            scrollRect.verticalNormalizedPosition = BetweenScenes.vertical_normalized_position;
        }
    }
    private IEnumerator LoadDownloadedImage(string imagePath, RawImage image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + imagePath);
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
            Debug.LogError("Image load failed: " + www.error);
        }
        www.Dispose();
    }
    private IEnumerator LoadImage(string imageURL, RawImage image, GameObject circle_object, Animator circle)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                float c = (texture.width * 1.0f) / texture.height;
                try
                {
                    AspectRatioFitter image_aspect = image.GetComponent<AspectRatioFitter>();
                    image_aspect.aspectRatio = c;
                }
                catch
                {
                    print("Raw image бфл удален");
                }
                image.texture = texture;
                circle.SetBool("is_circling", false);
                circle_object.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
            www.Dispose();
        }
    }
    private void Load(string json, string index_last_recipe)
    {
        RootObject recipeData = JsonUtility.FromJson<RootObject>("{\"recipes\":" + json + "}");
        for (int i = 0; i < recipeData.recipes.Length; i++)
        {
            RecipeData recipe = recipeData.recipes[i];
            // Создаем экземпляр префаба
            GameObject instantiatedPrefab = Instantiate(prefab, parentObject);
            GameObject circle_object = instantiatedPrefab.transform.GetChild(8).gameObject;
            Animator circle = circle_object.GetComponent<Animator>();
            instantiatedPrefab.transform.GetChild(7).name = Path.GetFileName(recipe.picture);
            circle.SetBool("is_circling", true);
            // Получаем доступ к компонентам RawImage и TextMeshProUGUI
            rawImageComponent = instantiatedPrefab.transform.GetChild(6).GetChild(0).GetComponent<RawImage>();
            StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(recipe.picture)}", rawImageComponent, circle_object, circle));
            recipe_name = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
            views = instantiatedPrefab.GetComponentsInChildren<TextMeshProUGUI>()[1];
            score = instantiatedPrefab.GetComponentsInChildren<TextMeshProUGUI>()[2];
            instantiatedPrefab.name = recipe.id;
            recipe_name.text = recipe.recipe_name;
            score.text = recipe.score;
            views.text = recipe.views;
            // Обновляем размер префаба в родительском объекте
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject);
        }
        Canvas.ForceUpdateCanvases();
        if(index_last_recipe == "-1")
        {
            //scrollRect.verticalNormalizedPosition = 1f;
            scrollRect.verticalNormalizedPosition = BetweenScenes.vertical_normalized_position;
        }
        
    }
}



[Serializable]
public class RecipeData
{
    public string id;
    public string username;
    public string picture;
    public string recipe_name;
    public string score;
    public string views;
}

[Serializable]
public class RootObject
{
    public RecipeData[] recipes;
}

