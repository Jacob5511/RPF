using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class About_Recipe : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/about_recipe";
    // URL для загрузки информации о рецепте из сети.

    string send_estimate_url = "http://22.cshse.beget.tech/send_estimate";
    // URL для отправки оценки рецепта.

    string delete_recipe_url = "http://22.cshse.beget.tech/delete_recipe";
    // URL для удаления рецепта.

    int index_steps = 0;
    // Индекс текущего шага приготовления.

    int star_count = 0;
    // Количество выбранных звёзд для оценки рецепта.

    string recipe_name, description, ingredients, stepsDescriptions, linksToPictures, timers, recipe_data_ingredients;
    // Различные данные о рецепте, такие как название, описание, ингредиенты, описание шагов, ссылки на изображения, информация о таймерах.

    public string recipe_id;
    // Идентификатор рецепта (public для доступа извне).

    Texture texture;
    // Текстура изображения рецепта.

    public RawImage main_foto;
    // Главное изображение рецепта.

    public TMP_Text recipe_name_text, desc, ingr;
    // Текстовые поля для отображения информации о рецепте.

    public GameObject stepPrefab, ing_desc_con, timer_prefab, start_cook, cooking_1, home, previous_step, next_step, name_rawimage, finish, apply_object, buttons, name_object, buttons_down, loading_screen, scroll, download_recipe, delete_download_recipe;
    // Различные игровые объекты и интерфейсные элементы.

    public Animator estimate, apply, are_you_sure_to_delete, loading_screen_anim;
    // Аниматоры для различных действий и переходов в интерфейсе.

    GameObject timer, start_cook_destroy;
    // Игровые объекты, используемые для таймеров и перехода к началу приготовления.

    public RectTransform content;
    // RectTransform для управления контентом внутри ScrollRect.

    public ScrollRect scrollRect;
    // ScrollRect для прокрутки контента.

    string[] timers_list;
    // Массив строк для хранения информации о таймерах.

    public List<GameObject> step_prefab_list = new List<GameObject>();
    // Список игровых объектов, представляющих шаги приготовления.

    public List<GameObject> stars = new List<GameObject>();
    // Список игровых объектов, представляющих звёзды для оценки рецепта.

    public Sprite full_star, not_selected_star;
    // Спрайты для полных и не выбранных звёзд.

    public GameManager game_manager;
    // Ссылка на компонент GameManager, вероятно, для управления игровой логикой.



    public void Start()
    {
        // Устанавливаем целевую частоту кадров на 60 кадров в секунду.
        Application.targetFrameRate = 60;

        // Проверяем подключение к интернету перед продолжением выполнения кода.
        StartCoroutine(game_manager.CheckInternetConnection(isConnected =>
        {
            // Если нет подключения к интернету и ресурс еще не загружен, переходим на сцену "MainScene".
            if (!isConnected && !BetweenScenes.is_downloaded)
            {
                SceneManager.LoadScene("MainScene");
            }
        }));

        // Получаем ссылку на изображение и настраиваем соотношение сторон.
        RawImage cooking_1_image = cooking_1.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
        AspectRatioFitter cooking_1_image_aspect = cooking_1.transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>();
        texture = BetweenScenes.texture;
        float c = (texture.width * 1.0f) / texture.height;
        AspectRatioFitter image_aspect = main_foto.GetComponent<AspectRatioFitter>();
        image_aspect.aspectRatio = c;
        cooking_1_image_aspect.aspectRatio = c;
        cooking_1_image.texture = texture;
        main_foto.texture = texture;
        main_foto.name = BetweenScenes.recipe_file_name;

        // Устанавливаем имя рецепта и его текстовое описание.
        recipe_name = BetweenScenes.recipe_name;
        recipe_name_text.text = recipe_name;

        // Добавляем текущий шаг в список шагов.
        step_prefab_list.Add(cooking_1);

        // Задаем путь для загрузки рецептов.
        string download_path = Path.Combine(Application.persistentDataPath, "recipes");

        try
        {
            // Пытаемся прочитать информацию о загруженных рецептах.
            string download_json = File.ReadAllText(Path.Combine(download_path, "text_info.txt"));
            DownloadRecipeObject download_recipe_object = JsonUtility.FromJson<DownloadRecipeObject>(download_json);

            // Проверяем, существует ли рецепт с данным ID в списке загруженных, и активируем соответствующую кнопку.
            print(download_recipe_object.recipes.Length);
            for (int i = 0; i < download_recipe_object.recipes.Length; i++)
            {
                if (download_recipe_object.recipes[i].recipe_id == BetweenScenes.recipes_id)
                {
                    delete_download_recipe.SetActive(true);
                    break;
                }
            }
        }
        catch
        {
            // Если возникает ошибка при чтении файла, скрываем кнопку удаления рецепта.
            delete_download_recipe.SetActive(false);
        }

        // Если рецепт уже загружен, скрываем кнопку загрузки и выполняем необходимые действия.
        if (BetweenScenes.is_downloaded)
        {
            download_recipe.SetActive(false);
            loading_screen_anim.SetTrigger("must_close");
            loading_screen.SetActive(false);
            AboutDownloadedRecipe();
        }
        else
        {
            // В противном случае загружаем информацию о рецепте с удаленного сервера.
            StartCoroutine(Load_All_About_Recipe(url));
        }
    }
    public void Stars(GameObject go)
    {
        // Оцениваем рецепт, устанавливая звезды.
        int index = stars.IndexOf(go);
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].GetComponent<Image>().sprite = not_selected_star;
        }
        for (int i = 0; i <= index; i++)
        {
            stars[i].GetComponent<Image>().sprite = full_star;
        }
        star_count = index + 1;
    }

    public void Open_Apply()
    {
        // Открываем или закрываем окно подтверждения.
        if (apply.GetBool("is_open"))
            apply.SetBool("is_open", false);
        else
            apply.SetBool("is_open", true);
    }

    public void Open_Estimate()
    {
        // Открываем окно оценки.
        estimate.SetBool("is_open", true);
    }

    public void Send_Estimate()
    {
        // Отправляем оценку на сервер.
        StartCoroutine(Send());
    }

    private IEnumerator Send()
    {
        // Отправка оценки на сервер.
        WWWForm form = new WWWForm();
        form.AddField("recipe_id", recipe_id);
        form.AddField("star_count", star_count);

        using (UnityWebRequest www = UnityWebRequest.Post(send_estimate_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if (answer == "ok")
                {
                    // Если оценка отправлена успешно, переходим на главную сцену.
                    SceneManager.LoadScene("MainScene");
                }
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error);
            }
            www.Dispose();
        }
    }

    public void Start_Cooking()
    {
        // Начинаем приготовление рецепта.
        foreach (GameObject obj in step_prefab_list)
        {
            Timer[] t = FindObjectsOfType<Timer>();
            foreach (Timer time in t)
            {
                time.Change();
            }
            obj.SetActive(false);
        }
        buttons_down.SetActive(true);
        home.SetActive(true);
        cooking_1.SetActive(true);
        name_object.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        if (stepsDescriptions.Length != 0)
            next_step.SetActive(true);
        name_rawimage.SetActive(false);
        ing_desc_con.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
        Destroy(start_cook_destroy);
    }


    public void Next_Step()
    {
        // Переход к следующему шагу приготовления.
        if (index_steps + 1 < step_prefab_list.Count)
            index_steps += 1;
        if (index_steps >= 1)
            previous_step.SetActive(true);
        if (index_steps == step_prefab_list.Count - 1)
        {
            next_step.SetActive(false);
            finish.SetActive(true);
            apply_object.SetActive(true);
        }
        step_prefab_list[index_steps].SetActive(true);
        step_prefab_list[index_steps - 1].SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void Previous_Step()
    {
        // Возврат к предыдущему шагу приготовления.
        if (index_steps - 1 >= 0)
            index_steps -= 1;
        if (index_steps == 0)
            previous_step.SetActive(false);
        if (index_steps < step_prefab_list.Count - 1)
        {
            next_step.SetActive(true);
            finish.SetActive(false);
            apply_object.SetActive(false);
        }
        step_prefab_list[index_steps].SetActive(true);
        step_prefab_list[index_steps + 1].SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void Delete_Recipe()
    {
        // Удаление рецепта с запросом на подтверждение.
        if (are_you_sure_to_delete.GetBool("is_open"))
        {
            StartCoroutine(Delete_Recipe_Request());
            return;
        }
        are_you_sure_to_delete.SetBool("is_open", true);
    }

    public void I_Made_Mistake()
    {
        // Отмена удаления рецепта.
        are_you_sure_to_delete.SetBool("is_open", false);
    }

    private IEnumerator Delete_Recipe_Request()
    {
        // Отправка запроса на удаление рецепта на сервер.
        WWWForm form = new WWWForm();
        form.AddField("recipe_id", recipe_id);

        using (UnityWebRequest www = UnityWebRequest.Post(delete_recipe_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if (answer == "ok")
                {
                    // Если удаление рецепта прошло успешно, выводим сообщение и переходим на главную сцену.
                    print("Рецепт успешно удален");
                    SceneManager.LoadScene("MainScene");
                }
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error);
            }
            www.Dispose();
        }
    }

    public void Edit_Recipe()
    {
        // Редактирование рецепта. Передаем данные о рецепте в сцену "UpdateRecipe".
        BetweenScenes.texture = main_foto.texture;
        BetweenScenes.aspect_ratio = main_foto.GetComponent<AspectRatioFitter>().aspectRatio;
        BetweenScenes.recipe_name = recipe_name;
        BetweenScenes.description = description;
        BetweenScenes.ingredients = recipe_data_ingredients;
        BetweenScenes.timers = timers;
        BetweenScenes.steps_descriptions = stepsDescriptions;
        BetweenScenes.links_to_pictures = linksToPictures;
        BetweenScenes.is_from_about_recipe = true;
        BetweenScenes.recipe_file_name = main_foto.name;
        BetweenScenes.recipes_id = recipe_id;
        SceneManager.LoadScene("UpdateRecipe");
    }

    private IEnumerator Load_All_About_Recipe(string url)
    {
        // Загрузка информации о рецепте с удаленного сервера.
        WWWForm form = new WWWForm();
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        form.AddField("recipes_id", BetweenScenes.recipes_id);
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                RecipeDataAboutRecipe recipeData = JsonUtility.FromJson<RecipeDataAboutRecipe>(jsonResponse);

                string chief_name = recipeData.chief_name;
                if (chief_name.ToUpper() == username.ToUpper())
                {
                    buttons.SetActive(true);
                }
                Load(jsonResponse, false);

                Debug.Log("Recipe upload successful");
                scroll.SetActive(true);
                loading_screen_anim.SetTrigger("must_close");
                loading_screen.SetActive(false);

                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }

    public void AboutDownloadedRecipe()
    {
        // Отображение информации о скачанном рецепте.
        string path = Path.Combine(Application.persistentDataPath, "recipes");
        string images_save_path = Path.Combine(path, "images");
        string all_except_images_save_path = Path.Combine(path, "text_info.txt");
        string json = File.ReadAllText(all_except_images_save_path);
        RecipeObject download_recipe_object = JsonUtility.FromJson<RecipeObject>(json);
        for (int i = 0; i < download_recipe_object.recipes.Length; i++)
        {
            RecipeDataAboutRecipe recipe = download_recipe_object.recipes[i];
            if (recipe.recipe_name == recipe_name_text.text)
            {
                json = JsonUtility.ToJson(download_recipe_object.recipes[i]);
            }
        }
        scroll.SetActive(true);
        loading_screen_anim.SetTrigger("must_close");
        loading_screen.SetActive(false);
        Load(json, true, images_save_path);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }
    private void Load(string json, bool is_downloaded,string path = "")
    {
        // Загрузка данных о рецепте.
        RecipeDataAboutRecipe recipeData = JsonUtility.FromJson<RecipeDataAboutRecipe>(json);
        recipe_name = recipeData.recipe_name;
        description = recipeData.description;
        recipe_data_ingredients = recipeData.ingredients;
        // Обработка списка ингредиентов.
        string[] sp = recipeData.ingredients.Split("|&");
        foreach (string name_and_quantity in sp)
        {
            string[] sp_name_and_quantity = name_and_quantity.Split("&|");
            ingredients += sp_name_and_quantity[0] + " " + sp_name_and_quantity[1] + " (" + sp_name_and_quantity[2] + ")" + "\n";
        }

        stepsDescriptions = recipeData.steps_descriptions;
        linksToPictures = recipeData.links_to_pictures;
        timers = recipeData.timers;
        recipe_id = recipeData.recipe_id;
        // Обработка таймеров.
        timers_list = timers.Split('|');
        int length;
        int.TryParse(timers_list[timers_list.Length - 1].Split('&')[0], out length);
        List<List<(string, string)>> ind = new List<List<(string, string)>>();
        for (int i = 0; i < length; i++)
        {
            ind.Add(new List<(string, string)>());
        }
        int index = -100;
        bool is_index = true;
        // Разбор информации о таймерах.
        for (int i = 0; i < timers_list.Length; i++)
        {
            string[] l = timers_list[i].Split("&");
            bool success = int.TryParse(l[0], out index);
            string p = "";
            for (int k = 0; k < l.Length; k++)
            {
                p += l[k] + " ";
            }
            if (success)
            {
                index -= 1;
                ind[index].Add((l[l.Length - 2], l[l.Length - 1]));
            }
            else
            {
                Debug.Log("Failed to parse index: " + l[0]);
                is_index = false;
            }
        }
        desc.text = description;
        ingr.text = ingredients;
        cooking_1.transform.GetChild(1).GetComponent<TMP_Text>().text = recipe_name;
        cooking_1.transform.GetChild(1).GetComponent<TMP_Text>().text = recipe_name;
        cooking_1.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = description;
        cooking_1.transform.GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = ingredients;
        // Обработка шагов приготовления.
        if (stepsDescriptions.Length != 0)
        {
            string[] stepsArray = stepsDescriptions.Split("|&");
            string[] picturesArray = linksToPictures.Split(' ');
            for (int i = 0; i < stepsArray.Length; i++)
            {
                GameObject stepObject = Instantiate(stepPrefab, content);
                timer = stepObject.transform.GetChild(1).gameObject;
                TMP_Text step_text = stepObject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
                TMP_Text stepDescription = stepObject.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
                stepDescription.text = stepsArray[i];
                RawImage stepImage = stepObject.transform.GetChild(2).GetChild(0).GetComponent<RawImage>();
                step_text.text = $"Шаг {i + 1}";
                // Обработка таймеров для текущего шага.
                if (index != -100 && is_index && i < ind.Count)
                {
                    for (int j = 0; j < ind[i].Count; j++)
                    {
                        timer.SetActive(true);
                        GameObject object_timer = Instantiate(timer_prefab, timer.transform);
                        TMP_Text for_what_not_cooking = object_timer.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
                        TMP_Text duration_text_not_cooking = object_timer.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
                        TMP_Text for_what_cooking = object_timer.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
                        TMP_Text duration_text_cooking = object_timer.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
                        TMP_Text duration_text_cooking_play = object_timer.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>();
                        for_what_not_cooking.text = ind[i][j].Item1;
                        duration_text_not_cooking.text = ind[i][j].Item2;
                        for_what_cooking.text = ind[i][j].Item1;
                        duration_text_cooking.text = ind[i][j].Item2;
                        duration_text_cooking_play.text = ind[i][j].Item2;
                    }
                }

                step_prefab_list.Add(stepObject);
                // Загрузка изображений для шагов.
                if (is_downloaded)
                {
                    StartCoroutine(LoadDownloadedImage(Path.Combine(path, Path.GetFileName(picturesArray[i])), stepImage));
                }
                else
                    StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(picturesArray[i])}", stepImage, i == stepsArray.Length - 1));

            }
        }

        start_cook_destroy = Instantiate(start_cook, content);
        buttons_down.transform.SetSiblingIndex(content.childCount - 1);
    }
    private IEnumerator LoadDownloadedImage(string imagePath, RawImage image)
    {
        // Загрузка изображения из локального хранилища.
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
    private IEnumerator LoadImage(string imageURL, RawImage image, bool is_last_index)
    {
        // Загрузка изображения из сети.
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

}
[System.Serializable]
public class RecipeDataAboutRecipe
{
    // Идентификатор рецепта.
    public string recipe_id;

    // Имя автора рецепта.
    public string chief_name;

    // Название рецепта.
    public string recipe_name;

    // Описание рецепта.
    public string description;

    // Ингредиенты, необходимые для приготовления.
    public string ingredients;

    // Описание шагов приготовления.
    public string steps_descriptions;

    // Ссылки на изображения, связанные с рецептом.
    public string links_to_pictures;

    // Информация о таймерах для каждого шага приготовления.
    public string timers;

    // Ссылка на главное изображение рецепта.
    public string main_picture;

    // Рейтинг рецепта.
    public string score;

    // Количество просмотров рецепта.
    public string views;
}
