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
    // URL ��� �������� ���������� � ������� �� ����.

    string send_estimate_url = "http://22.cshse.beget.tech/send_estimate";
    // URL ��� �������� ������ �������.

    string delete_recipe_url = "http://22.cshse.beget.tech/delete_recipe";
    // URL ��� �������� �������.

    int index_steps = 0;
    // ������ �������� ���� �������������.

    int star_count = 0;
    // ���������� ��������� ���� ��� ������ �������.

    string recipe_name, description, ingredients, stepsDescriptions, linksToPictures, timers, recipe_data_ingredients;
    // ��������� ������ � �������, ����� ��� ��������, ��������, �����������, �������� �����, ������ �� �����������, ���������� � ��������.

    public string recipe_id;
    // ������������� ������� (public ��� ������� �����).

    Texture texture;
    // �������� ����������� �������.

    public RawImage main_foto;
    // ������� ����������� �������.

    public TMP_Text recipe_name_text, desc, ingr;
    // ��������� ���� ��� ����������� ���������� � �������.

    public GameObject stepPrefab, ing_desc_con, timer_prefab, start_cook, cooking_1, home, previous_step, next_step, name_rawimage, finish, apply_object, buttons, name_object, buttons_down, loading_screen, scroll, download_recipe, delete_download_recipe;
    // ��������� ������� ������� � ������������ ��������.

    public Animator estimate, apply, are_you_sure_to_delete, loading_screen_anim;
    // ��������� ��� ��������� �������� � ��������� � ����������.

    GameObject timer, start_cook_destroy;
    // ������� �������, ������������ ��� �������� � �������� � ������ �������������.

    public RectTransform content;
    // RectTransform ��� ���������� ��������� ������ ScrollRect.

    public ScrollRect scrollRect;
    // ScrollRect ��� ��������� ��������.

    string[] timers_list;
    // ������ ����� ��� �������� ���������� � ��������.

    public List<GameObject> step_prefab_list = new List<GameObject>();
    // ������ ������� ��������, �������������� ���� �������������.

    public List<GameObject> stars = new List<GameObject>();
    // ������ ������� ��������, �������������� ����� ��� ������ �������.

    public Sprite full_star, not_selected_star;
    // ������� ��� ������ � �� ��������� ����.

    public GameManager game_manager;
    // ������ �� ��������� GameManager, ��������, ��� ���������� ������� �������.



    public void Start()
    {
        // ������������� ������� ������� ������ �� 60 ������ � �������.
        Application.targetFrameRate = 60;

        // ��������� ����������� � ��������� ����� ������������ ���������� ����.
        StartCoroutine(game_manager.CheckInternetConnection(isConnected =>
        {
            // ���� ��� ����������� � ��������� � ������ ��� �� ��������, ��������� �� ����� "MainScene".
            if (!isConnected && !BetweenScenes.is_downloaded)
            {
                SceneManager.LoadScene("MainScene");
            }
        }));

        // �������� ������ �� ����������� � ����������� ����������� ������.
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

        // ������������� ��� ������� � ��� ��������� ��������.
        recipe_name = BetweenScenes.recipe_name;
        recipe_name_text.text = recipe_name;

        // ��������� ������� ��� � ������ �����.
        step_prefab_list.Add(cooking_1);

        // ������ ���� ��� �������� ��������.
        string download_path = Path.Combine(Application.persistentDataPath, "recipes");

        try
        {
            // �������� ��������� ���������� � ����������� ��������.
            string download_json = File.ReadAllText(Path.Combine(download_path, "text_info.txt"));
            DownloadRecipeObject download_recipe_object = JsonUtility.FromJson<DownloadRecipeObject>(download_json);

            // ���������, ���������� �� ������ � ������ ID � ������ �����������, � ���������� ��������������� ������.
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
            // ���� ��������� ������ ��� ������ �����, �������� ������ �������� �������.
            delete_download_recipe.SetActive(false);
        }

        // ���� ������ ��� ��������, �������� ������ �������� � ��������� ����������� ��������.
        if (BetweenScenes.is_downloaded)
        {
            download_recipe.SetActive(false);
            loading_screen_anim.SetTrigger("must_close");
            loading_screen.SetActive(false);
            AboutDownloadedRecipe();
        }
        else
        {
            // � ��������� ������ ��������� ���������� � ������� � ���������� �������.
            StartCoroutine(Load_All_About_Recipe(url));
        }
    }
    public void Stars(GameObject go)
    {
        // ��������� ������, ������������ ������.
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
        // ��������� ��� ��������� ���� �������������.
        if (apply.GetBool("is_open"))
            apply.SetBool("is_open", false);
        else
            apply.SetBool("is_open", true);
    }

    public void Open_Estimate()
    {
        // ��������� ���� ������.
        estimate.SetBool("is_open", true);
    }

    public void Send_Estimate()
    {
        // ���������� ������ �� ������.
        StartCoroutine(Send());
    }

    private IEnumerator Send()
    {
        // �������� ������ �� ������.
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
                    // ���� ������ ���������� �������, ��������� �� ������� �����.
                    SceneManager.LoadScene("MainScene");
                }
            }
            else
            {
                Debug.LogError("���� ������ �� ������� �������" + www.error);
            }
            www.Dispose();
        }
    }

    public void Start_Cooking()
    {
        // �������� ������������� �������.
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
        // ������� � ���������� ���� �������������.
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
        // ������� � ����������� ���� �������������.
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
        // �������� ������� � �������� �� �������������.
        if (are_you_sure_to_delete.GetBool("is_open"))
        {
            StartCoroutine(Delete_Recipe_Request());
            return;
        }
        are_you_sure_to_delete.SetBool("is_open", true);
    }

    public void I_Made_Mistake()
    {
        // ������ �������� �������.
        are_you_sure_to_delete.SetBool("is_open", false);
    }

    private IEnumerator Delete_Recipe_Request()
    {
        // �������� ������� �� �������� ������� �� ������.
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
                    // ���� �������� ������� ������ �������, ������� ��������� � ��������� �� ������� �����.
                    print("������ ������� ������");
                    SceneManager.LoadScene("MainScene");
                }
            }
            else
            {
                Debug.LogError("���� ������ �� ������� �������" + www.error);
            }
            www.Dispose();
        }
    }

    public void Edit_Recipe()
    {
        // �������������� �������. �������� ������ � ������� � ����� "UpdateRecipe".
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
        // �������� ���������� � ������� � ���������� �������.
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
        // ����������� ���������� � ��������� �������.
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
        // �������� ������ � �������.
        RecipeDataAboutRecipe recipeData = JsonUtility.FromJson<RecipeDataAboutRecipe>(json);
        recipe_name = recipeData.recipe_name;
        description = recipeData.description;
        recipe_data_ingredients = recipeData.ingredients;
        // ��������� ������ ������������.
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
        // ��������� ��������.
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
        // ������ ���������� � ��������.
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
        // ��������� ����� �������������.
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
                step_text.text = $"��� {i + 1}";
                // ��������� �������� ��� �������� ����.
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
                // �������� ����������� ��� �����.
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
        // �������� ����������� �� ���������� ���������.
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
        // �������� ����������� �� ����.
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
    // ������������� �������.
    public string recipe_id;

    // ��� ������ �������.
    public string chief_name;

    // �������� �������.
    public string recipe_name;

    // �������� �������.
    public string description;

    // �����������, ����������� ��� �������������.
    public string ingredients;

    // �������� ����� �������������.
    public string steps_descriptions;

    // ������ �� �����������, ��������� � ��������.
    public string links_to_pictures;

    // ���������� � �������� ��� ������� ���� �������������.
    public string timers;

    // ������ �� ������� ����������� �������.
    public string main_picture;

    // ������� �������.
    public string score;

    // ���������� ���������� �������.
    public string views;
}