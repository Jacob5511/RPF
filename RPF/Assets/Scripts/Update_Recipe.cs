using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Update_Recipe : MonoBehaviour
{
    public RawImage own_image;
    RawImage image;
    private string serverURL = "http://22.cshse.beget.tech/update_recipe";
    public TMP_InputField recipe_name, description;
    Add_Step add_step;
    string ingredients;
    ImagePathScripts imagePathScripts;
    public ScrollRect scrollRect;
    public GameObject stepPrefab, timer_prefab, add_image;
    public RectTransform content, steps;
    public Animator success_anim, all_fields_must;
    string recipe_id;
    void Start()
    {

        // Получение ссылок на компоненты и предыдущие данные рецепта

        // Получение изображения и данных о рецепте из предыдущего экрана
        // и присвоение их соответствующим компонентам

        // Инициализация данных о ингредиентах, шагах и таймерах
        // на основе предыдущей информации

        // Загрузка изображений для рецепта и шагов приготовления
        add_step = GameObject.Find("Add_Step").GetComponent<Add_Step>();
        recipe_id = BetweenScenes.recipes_id;
        imagePathScripts = add_image.GetComponent<ImagePathScripts>();
        BetweenScenes.is_from_about_recipe = false;
        own_image.texture = BetweenScenes.texture;
        own_image.name = BetweenScenes.recipe_file_name;
        imagePathScripts.is_photo_selected = true;
        own_image.GetComponent<AspectRatioFitter>().aspectRatio = BetweenScenes.aspect_ratio;
        recipe_name.text = BetweenScenes.recipe_name;
        description.text = BetweenScenes.description;
        ingredients = BetweenScenes.ingredients;
        string links_to_pictures = BetweenScenes.links_to_pictures;
        string timers = BetweenScenes.timers;
        string stepsDescriptions = BetweenScenes.steps_descriptions;

        string[] sp_ingredients = ingredients.Split("|&");
        string[] name_and_quantity = sp_ingredients[0].Split("&|");
        add_step.ingredients[0].transform.GetChild(0).GetComponent<TMP_InputField>().text = name_and_quantity[0];
        add_step.ingredients[0].transform.GetChild(1).GetComponent<TMP_InputField>().text = name_and_quantity[1];
        add_step.ingredients[0].transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = name_and_quantity[2];
        add_step.ingredients[0].transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";
        print(sp_ingredients.Length);
        for (int i = 1; i < sp_ingredients.Length; i++)
        {
            print("lol   " + sp_ingredients.Length);
            add_step.Add_Step_Ingredient();
            name_and_quantity = sp_ingredients[i].Split("&|");
            int length = add_step.ingredients.Count;
            add_step.ingredients[length - 1].transform.GetChild(0).GetComponent<TMP_InputField>().text = name_and_quantity[0];
            add_step.ingredients[length - 1].transform.GetChild(1).GetComponent<TMP_InputField>().text = name_and_quantity[1];
            add_step.ingredients[length - 1].transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = name_and_quantity[2];
        }

        string[] timers_list = timers.Split('|');
        int length_timers_list;
        int.TryParse(timers_list[timers_list.Length - 1].Split('&')[0], out length_timers_list);
        List<List<(string, string)>> ind = new List<List<(string, string)>>();
        for (int i = 0; i < length_timers_list; i++)
        {
            ind.Add(new List<(string, string)>());
        }
        int index = -100;
        bool is_index = true;
        for (int i = 0; i < timers_list.Length; i++)
        {
            string[] l = timers_list[i].Split('&');
            bool success = int.TryParse(l[0], out index);
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
        if (stepsDescriptions.Length != 0)
        {
            string[] stepsArray = stepsDescriptions.Split("|&");
            string[] picturesArray = links_to_pictures.Split(' ');
            //Vector2 prev_pos = new Vector2(0,0);
            for (int i = 0; i < stepsArray.Length; i++)
            {
                add_step.delete_step.SetActive(true);
                add_step.count += 1;
                GameObject stepObject = Instantiate(stepPrefab, steps);
                Add_Step_But add_step_but = stepObject.GetComponentInChildren<Add_Step_But>();
                stepObject.transform.GetChild(5).GetChild(1).name = Path.GetFileName(picturesArray[i]);
                GameObject timer = stepObject.transform.GetChild(3).gameObject;
                TMP_Text step_text = stepObject.transform.GetChild(0).GetComponent<TMP_Text>();
                TMP_InputField stepDescription = stepObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>();
                stepDescription.text = stepsArray[i];
                RawImage stepImage = stepObject.transform.GetChild(5).GetChild(1).GetComponent<RawImage>();
                ImagePathScripts ips = stepObject.transform.GetChild(5).GetChild(0).GetComponent<ImagePathScripts>();
                ips.is_photo_selected = true;
                step_text.text = $"Шаг {i + 1}";
                if (index != -100 && is_index && i < ind.Count)
                {
                    for (int j = 0; j < ind[i].Count; j++)
                    {
                        timer.SetActive(true);
                        GameObject object_timer = Instantiate(timer_prefab, timer.transform);
                        TMP_InputField for_what = object_timer.transform.GetChild(1).GetComponent<TMP_InputField>();
                        TMP_InputField duration_input = object_timer.transform.GetChild(0).GetComponent<TMP_InputField>();
                        for_what.text = ind[i][j].Item1;
                        duration_input.text = ind[i][j].Item2;
                        ips.inputs_prefab.Add(object_timer);
                        add_step_but.delete_timer.SetActive(true);
                    }
                }

                add_step.objects.Add(stepObject);

                StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(picturesArray[i])}", stepImage));
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
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
    public void UpdateRecipe()
    {
        if (recipe_name.text == "" || description.text == "")
            No();
        print("click");
        StartCoroutine(SendImage());
    }
    void No()
    {
        // Вывод уведомления о необходимости заполнить все обязательные поля
        all_fields_must.SetTrigger("must");
    }
    private IEnumerator SendImage()
    {
        // Подготовка и отправка изображения рецепта на сервер

        // Загрузка данных о ингредиентах, шагах и таймерах

        // Проверка обязательных полей на заполненность

        // Загрузка и отправка изображений для шагов приготовления

        // Формирование запроса к серверу

        // Отправка запроса на сервер

        // Вывод уведомления о успешном обновлении рецепта
        print("start");
        Texture2D texture;
        byte[] imageBytes_prefab;
        string fileName;
        string names_ingredients;

        WWWForm form = new WWWForm();
        form.AddField("id", recipe_id);

        string imagePath_prefab, ingredients, inputs_string = "";
        List<string> sp = new List<string>();
        List<string> inputFields = new List<string>();
        List<string> names_ingredients_list = new List<string>();

        for (int i = 0; i < add_step.ingredients.Count; i++)
        {
            TMP_InputField name = add_step.ingredients[i].transform.GetChild(0).GetComponent<TMP_InputField>();
            TMP_InputField quantity = add_step.ingredients[i].transform.GetChild(1).GetComponent<TMP_InputField>();
            TMP_Text units = add_step.ingredients[i].transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
            if (name.text == "" || quantity.text == "" || units.text == "")
            {
                No();
                yield break;
            }
            string name_and_quantity = name.text + "&|" + quantity.text + "&|" + units.text;
            sp.Add(name_and_quantity);
            names_ingredients_list.Add(name.text);
        }

        ingredients = string.Join("|&", sp);
        names_ingredients = string.Join(",", names_ingredients_list);
        string fieldName = "picture0";
        Guid new_guid = Guid.NewGuid();
        fileName = new_guid.ToString();
        texture = (Texture2D)own_image.texture;
        imageBytes_prefab = texture.EncodeToJPG();
        form.AddBinaryData(fieldName, imageBytes_prefab, fileName, "image/jpg");
        List<string> inpust_string_list = new List<string>();
        for (int i = 1; i < add_step.objects.Count; i++)
        {
            List<GameObject> inputs = add_step.objects[i].transform.GetChild(5).GetChild(0).GetComponent<ImagePathScripts>().inputs_prefab;
            string inputField_prefab = add_step.objects[i].transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text;
            inputFields.Add(inputField_prefab);
            imagePath_prefab = add_step.objects[i].transform.GetChild(5).GetChild(0).GetComponent<ImagePathScripts>().imagePath;
            int count_inputs = inputs.Count;
            bool is_photo_selected = add_step.objects[i].transform.GetChild(5).GetChild(0).GetComponent<ImagePathScripts>().is_photo_selected;
            if(inputField_prefab == "" || (imagePath_prefab == "" && !is_photo_selected))
            {
                No();
                yield break;
            }
            for (int j = 0; j < count_inputs; j++)
            {
                string for_what = inputs[j].transform.GetChild(1).GetComponent<TMP_InputField>().text;
                string duration_input = inputs[j].transform.GetChild(0).GetComponent<TMP_InputField>().text;
                if (for_what == "" || duration_input == "")
                {
                    No();
                    yield break;
                }
                inpust_string_list.Add($"{i}&{j}&{for_what}&{duration_input}");
            }
            texture = (Texture2D)add_step.objects[i].transform.GetChild(5).GetChild(1).GetComponent<RawImage>().texture;
            imageBytes_prefab = texture.EncodeToJPG();
            fieldName = "picture" + i.ToString();
            Guid new_guid_i = Guid.NewGuid();
            fileName = new_guid_i.ToString();
            form.AddBinaryData(fieldName, imageBytes_prefab, fileName, "image/jpg");

        }
        string inputFieldsString = string.Join("|&", inputFields);
        inputs_string = string.Join("|", inpust_string_list);
        form.AddField("recipe_name", recipe_name.text);
        form.AddField("description", description.text);
        form.AddField("ingredients", ingredients);
        form.AddField("steps_descriptions", inputFieldsString);
        form.AddField("timers", inputs_string);
        form.AddField("names_ingredients", names_ingredients);
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        form.AddField("username", user.username);
        print("SENT");

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text == "ok")
            {
                Debug.Log("Recipe upload successful");
                success_anim.SetTrigger("must");
            }
            else
            {
                Debug.LogError(www.error);
            }
            www.Dispose();
        }
    }
}
