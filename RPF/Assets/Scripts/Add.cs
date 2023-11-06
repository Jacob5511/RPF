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

public class Add : MonoBehaviour
{
    public RawImage own_image;
    // Ссылка на RawImage для отображения главного изображения.

    RawImage image;
    // Временный RawImage для загрузки и отображения выбранного изображения.

    private string serverURL = "http://22.cshse.beget.tech/upload";
    // URL сервера для загрузки рецепта и изображения.

    public TMP_InputField recipe_name, description;
    // Поля ввода для названия рецепта и описания.

    Add_Step add_step;
    // Ссылка на компонент Add_Step, для работы с шагами приготовления.

    ImagePathScripts imagePathScripts;
    // Ссылка на компонент ImagePathScripts, для управления путями к изображениям.

    private ScrollRect scrollRect;
    // Ссылка на компонент ScrollRect для управления прокруткой.

    Animator gallery_but_anim, camera_but_anim, not_allowed_same, all_fields_must, recipe_upload_successful;
    // Аниматоры для анимаций и визуальных эффектов.

    private void Start()
    {
        Application.targetFrameRate = 60;
        // Установка целевой частоты кадров.

        gallery_but_anim = GameObject.FindGameObjectWithTag("gallery").GetComponent<Animator>();
        camera_but_anim = GameObject.FindGameObjectWithTag("camera").GetComponent<Animator>();
        not_allowed_same = GameObject.FindGameObjectWithTag("not_allowed").GetComponent<Animator>();
        all_fields_must = GameObject.FindGameObjectWithTag("all_fields").GetComponent<Animator>();
        recipe_upload_successful = GameObject.FindGameObjectWithTag("recipe_upload_successful").GetComponent<Animator>();
        // Получение ссылок на аниматоры из объектов в сцене.

        imagePathScripts = gameObject.GetComponent<ImagePathScripts>();
        // Получение компонента ImagePathScripts у текущего объекта.

        scrollRect = GetComponentInParent<ScrollRect>();
        // Получение компонента ScrollRect из родительского объекта.

        add_step = GameObject.Find("Add_Step").GetComponent<Add_Step>();
        // Получение компонента Add_Step для работы с шагами приготовления.
    }

    public void SelectImage()
    {
        // Метод для выбора изображения из галереи или камеры.

        if (!camera_but_anim.GetBool("is_open"))
        {
            // Если анимация камеры не открыта, открываем её и галерею.

            camera_but_anim.Play("Open_camera");
            gallery_but_anim.Play("Open_gallery");
            camera_but_anim.SetBool("is_open", true);
            gallery_but_anim.SetBool("is_open", true);
            BetweenScenes.image = own_image;
            BetweenScenes.image_path_scripts = imagePathScripts;
        }
        else
        {
            // В противном случае запускаем анимацию закрытия.

            Anim();
        }
    }

    void Anim()
    {
        // Метод для анимации закрытия камеры и галереи.

        if (camera_but_anim.GetBool("is_open"))
        {
            camera_but_anim.Play("Close_camera");
            gallery_but_anim.Play("Close_gallery");
            camera_but_anim.SetBool("is_open", false);
            gallery_but_anim.SetBool("is_open", false);
        }
    }

    public void Camera()
    {
        // Метод для использования камеры для фотографии.

        image = BetweenScenes.image;
        Anim();
        if (NativeCamera.IsCameraBusy())
            return;
        TakePicture(2400);
    }

    public void Gallery()
    {
        // Метод для выбора изображения из галереи.

        image = BetweenScenes.image;
        Anim();
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            return;
        }
        StartCoroutine(LoadImageFromGallery());
    }

    private void TakePicture(int maxSize)
    {
        // Метод для съемки фотографии с использованием камеры.

        NativeCamera.Permission permission = NativeCamera.TakePicture((imagePath) =>
        {
            if (imagePath != null)
            {
                // Создание Texture2D из снятой фотографии.

                Texture2D texture = NativeCamera.LoadImageAtPath(imagePath, maxSize, false, true);
                if (texture == null)
                {
                    Debug.LogError("Couldn't load texture from " + imagePath);
                    return;
                }
                float c = (texture.width * 1.0f) / texture.height;
                AspectRatioFitter image_aspect = image.GetComponent<AspectRatioFitter>();
                image_aspect.aspectRatio = c;
                image.texture = texture;
            }
            BetweenScenes.image_path_scripts.imagePath = imagePath;
        }, maxSize, true);

        Debug.Log("Permission result: " + permission);
    }

    private IEnumerator LoadImageFromGallery()
    {
        // Метод для выбора изображения из галереи.

        var pickImageOperation = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                StartCoroutine(LoadImage(path));
            }
        });

        yield return pickImageOperation;
    }

    private IEnumerator LoadImage(string imagePath)
    {
        // Метод для загрузки изображения.

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
        BetweenScenes.image_path_scripts.imagePath = imagePath;
        www.Dispose();
    }

    public void UploadImage()
    {
        // Метод для загрузки изображения и данных о рецепте.

        if (imagePathScripts.imagePath == "" || recipe_name.text == "" || description.text == "")
            No();
        print("START");
        StartCoroutine(SendImage());
    }

    void No()
    {
        // Метод для обработки случая, когда не все поля заполнены.

        all_fields_must.SetTrigger("must");
    }

    private IEnumerator SendImage()
    {
        // Метод для отправки изображения и данных на сервер.

        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);

        WWWForm form = new WWWForm();
        form.AddField("username", user.username);
        string imagePath_prefab, ingredients;
        string inputs_string = "";
        string names_ingredients;
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
        List<string> inpust_string_list = new List<string>();
        Texture2D texture;
        for (int i = 0; i < add_step.objects.Count; i++)
        {
            if (i != 0)
            {
                texture = (Texture2D)add_step.objects[i].transform.GetChild(5).GetChild(1).GetComponent<RawImage>().texture;
                List<GameObject> inputs = add_step.objects[i].transform.GetChild(5).GetChild(0).GetComponent<ImagePathScripts>().inputs_prefab;
                string inputField_prefab = add_step.objects[i].transform.GetChild(1).GetChild(0).GetComponent<TMP_InputField>().text;
                inputFields.Add(inputField_prefab);
                imagePath_prefab = add_step.objects[i].transform.GetChild(5).GetChild(0).GetComponent<ImagePathScripts>().imagePath;
                if (inputField_prefab == "" || imagePath_prefab == "")
                {
                    No();
                    yield break;
                }
                int count_inputs = inputs.Count;
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
            }
            else
            {
                texture = (Texture2D)own_image.texture;
                imagePath_prefab = imagePathScripts.imagePath;
                if (imagePath_prefab == "")
                {
                    No();
                    yield break;
                }
            }

            byte[] imageBytes_prefab = texture.EncodeToJPG();
            string fieldName = "picture" + i.ToString();
            Guid new_guid = Guid.NewGuid();
            string fileName = new_guid.ToString();
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
        print("SENT");

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text == "ok")
            {
                Debug.Log("Recipe upload successful");
                recipe_upload_successful.SetTrigger("must");
            }
            else
            {
                not_allowed_same.SetTrigger("must");
                Debug.Log(www.error);
            }
            www.Dispose();
        }
    }

}