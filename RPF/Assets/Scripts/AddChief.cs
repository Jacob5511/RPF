using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class AddChief : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/add_chief";
    // URL-адрес для отправки данных на сервер.

    public Animator camera_but_anim, gallery_but_anim, all_fields_must;
    // Аниматоры для анимаций кнопок камеры, галереи и сообщений об ошибках.

    public RawImage image;
    string imagePath = "";
    // Переменные для работы с изображением и его пути.

    string template = "+7 (***) ***-**-**";
    // Шаблон для отображения телефонного номера.

    public TMP_InputField phone_number, metro, short_about_yourself;
    public TMP_Text text, chief_name;
    // Поля для ввода текста и отображения имени шефа.

    public GameObject placeholder;
    public int count = 0;
    // Пустой объект-заполнитель для поля ввода.


    private void Start()
    {
        // Инициализация при запуске сцены.

        Application.targetFrameRate = 60;
        // Установка целевой частоты кадров.

        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        chief_name.text = user.username;
        
        // Загрузка имени пользователя из файла и отображение его на экране.
    }

    public void SelectImage()
    {
        // Выбор изображения из галереи или камеры.

        if (!camera_but_anim.GetBool("is_open"))
        {
            camera_but_anim.Play("Open_camera");
            gallery_but_anim.Play("Open_gallery");
            camera_but_anim.SetBool("is_open", true);
            gallery_but_anim.SetBool("is_open", true);
        }
        else
        {
            Anim();
        }
    }

    void Anim()
    {
        // Закрытие анимации выбора изображения.

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
        // Сделать фото с помощью камеры.

        Anim();
        if (NativeCamera.IsCameraBusy())
            return;
        TakePicture(2400);
    }

    public void Gallery()
    {
        // Выбор изображения из галереи.

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
        // Сделать фото с помощью камеры.

        NativeCamera.Permission permission = NativeCamera.TakePicture((imagePath) =>
        {
            Debug.Log("Image path: " + imagePath);
            if (imagePath != null)
            {
                // Создание текстуры из захваченного изображения
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
                this.imagePath = imagePath;
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    private IEnumerator LoadImageFromGallery()
    {
        // Открыть галерею и дождаться, пока пользователь выберет изображение.

        var pickImageOperation = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                imagePath = path;
                StartCoroutine(LoadImage(path));
            }
        });

        // Дождитесь завершения операции выбора изображения
        yield return pickImageOperation;
    }

    private IEnumerator LoadImage(string imagePath)
    {
        // Загрузить изображение по указанному пути.

        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + imagePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Получить текстуру из загруженного изображения
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

    public void UploadImage()
    {
        // Загрузка изображения на сервер.
        if (imagePath == "" || phone_number.text == "" || metro.text == "" || short_about_yourself.text == "")
        {
            all_fields_must.SetTrigger("must");
            return;
        }
        StartCoroutine(SendImage(false));
    }

    public void Update_Chief()
    {
        // Обновление информации о шефе.

        StartCoroutine(SendImage(true));
    }

    private IEnumerator SendImage(bool is_update)
    {
        // Отправка данных на сервер.

        WWWForm form = new WWWForm();
        form.AddField("username", chief_name.text);
        Texture2D texture = (Texture2D)image.texture;
        byte[] imageBytes_prefab = texture.EncodeToJPG();
        string fileName = Path.GetFileName(imagePath);
        Guid new_guid = Guid.NewGuid();
        fileName = new_guid.ToString();
        form.AddBinaryData("image", imageBytes_prefab, fileName, "image/jpg");

        form.AddField("phone_number", text.text + " ");
        print(text.text);
        form.AddField("metro", metro.text);
        form.AddField("short_about_yourself", short_about_yourself.text);
        form.AddField("is_update", is_update.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text == "ok")
            {
                Debug.Log("Шеф добавлен или Информация обновлена");
                SceneManager.LoadScene("MainForChief");
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose();
        }
    }

    public void OnChangesPhoneNumber()
    {
        // Изменение формата ввода телефонного номера.

        if (phone_number.text.Length == count - 1)
        {
            count -= 1;
            for (int i = template.Length - 1; i >= 0; i--)
            {
                if (!"()*- ".Contains(template[i]))
                {
                    template = template.Substring(0, i) + '*' + template.Substring(i + 1);
                    break;
                }
            }
        }
        if (phone_number.text.Length == count + 1)
        {
            count += 1;
            for (int i = 0; i < template.Length; i++)
            {
                if (phone_number.text.Length == 0)
                    break;
                if (template[i] == '*')
                {
                    template = template.Substring(0, i) + phone_number.text[count - 1] + template.Substring(i + 1);
                    break;
                }
            }
        }
        text.text = template;
        if (count == 0)
        {
            text.text = "";
            placeholder.SetActive(true);
        }
        else
            placeholder.SetActive(false);
    }
}
