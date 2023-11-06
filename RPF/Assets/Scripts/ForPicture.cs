using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ForPicture : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    private ScrollRect scrollRect;  // Объявление переменной ScrollRect.

    public static bool is_possible = true;  // Объявление статической переменной is_possible и установка её значения в true.
    private string add_response_url = "http://22.cshse.beget.tech/add_response";  // Строка с URL для отправки ответа.

    public Add add;  // Объявление переменной типа Add.
    public bool is_announcement;  // Булева переменная, указывающая, является ли объявлением.
    public Animator add_announcement;  // Объявление переменной типа Animator для анимации объявления.
    Animator success;  // Объявление переменной типа Animator для анимации успешного действия.
    MyChiefAccount mca;  // Объявление переменной типа MyChiefAccount.
    GameManager game_manager;  // Объявление переменной типа GameManager.
    MyAccount my_account;  // Объявление переменной типа MyAccount.
    public GameObject foto, chief_name, star, eye, views, score, about_announcecment;  // Объявление переменных GameObject для различных элементов.
    public Animator circle;

    private void Start()
    {
        game_manager = FindObjectOfType<GameManager>();  // Поиск объекта GameManager в сцене и присваивание его переменной game_manager.
        mca = FindObjectOfType<MyChiefAccount>();  // Поиск объекта MyChiefAccount в сцене и присваивание его переменной mca.
        scrollRect = GetComponentInParent<ScrollRect>();  // Получение компонента ScrollRect из родительского объекта и присваивание его переменной scrollRect.
        my_account = FindObjectOfType<MyAccount>();  // Поиск объекта MyAccount в сцене и присваивание его переменной my_account.

        try
        {
            success = GameObject.FindGameObjectWithTag("success").GetComponent<Animator>();  // Попытка найти объект с тегом "success" и получить компонент Animator.
        }
        catch
        {
            return;  // В случае ошибки, выход из функции Start.
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        scrollRect.OnBeginDrag(data);  // Вызов метода OnBeginDrag компонента ScrollRect.
        is_possible = false;  // Установка значения is_possible в false.
    }

    public void OnDrag(PointerEventData data)
    {
        scrollRect.OnDrag(data);  // Вызов метода OnDrag компонента ScrollRect.
    }

    public void OnEndDrag(PointerEventData data)
    {
        scrollRect.OnEndDrag(data);  // Вызов метода OnEndDrag компонента ScrollRect.
        is_possible = true;  // Установка значения is_possible в true.
    }

    public void OnScroll(PointerEventData data)
    {
        scrollRect.OnScroll(data);  // Вызов метода OnScroll компонента ScrollRect.
    }

    public void Delete_Response_For_Announcement()
    {
        StartCoroutine(mca.LoadMyResponses(gameObject.name));  // Запуск корутины LoadMyResponses из объекта mca с параметром имени текущего объекта.
    }

    public void Add_Response_For_Announcement()
    {
        add_announcement.SetBool("is_open", false);  // Установка параметра is_open объекта add_announcement в false.
        StartCoroutine(Add_Response());  // Запуск корутины Add_Response.
    }

    private IEnumerator Add_Response()
    {
        WWWForm form = new WWWForm();  // Создание новой формы для отправки данных.
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));  // Чтение JSON-файла из пути Application.persistentDataPath.
        User user = JsonUtility.FromJson<User>(json);  // Десериализация JSON-строки в объект User.
        string username = user.username;  // Получение имени пользователя из объекта User.
        form.AddField("username", username);  // Добавление данных в форму.
        form.AddField("id", gameObject.name);  // Добавление данных в форму.

        using (UnityWebRequest www = UnityWebRequest.Post(add_response_url, form))  // Отправка POST-запроса на указанный URL с использованием формы.
        {
            yield return www.SendWebRequest();  // Ожидание завершения запроса.

            if (www.result == UnityWebRequest.Result.Success)  // Если запрос выполнен успешно.
            {
                success.SetTrigger("must");  // Запуск анимации "must" для объекта success.
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");  // Вывод сообщения об ошибке.
            }
            www.Dispose();  // Освобождение ресурсов запроса.
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (add_announcement != null && is_possible)
        {
            if (add_announcement.GetBool("is_open"))
                add_announcement.SetBool("is_open", false);
            else
                add_announcement.SetBool("is_open", true);
            return;
        }
        if (is_possible && !is_announcement)
            add.SelectImage();
        else if (is_possible && is_announcement && circle != null && !circle.GetBool("is_circling"))
            Cause();
    }

    public void Cause()
    {
        if (is_possible)
        {
            string name_scene = SceneManager.GetActiveScene().name;  // Получение имени активной сцены.
            string chief_name = gameObject.transform.GetChild(2).GetComponent<TMP_Text>().text;  // Получение имени "шефа" из дочернего объекта.
            RawImage chief_image = gameObject.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();  // Получение изображения "шефа" из дочернего объекта.
            float aspect_ratio = gameObject.transform.GetChild(1).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio;  // Получение соотношения сторон изображения.
            BetweenScenes.name = chief_name;  // Присвоение имени "шефа" переменной BetweenScenes.name.
            BetweenScenes.texture = chief_image.texture;  // Присвоение текстуры изображения переменной BetweenScenes.texture.
            BetweenScenes.aspect_ratio = aspect_ratio;  // Присвоение соотношения сторон переменной BetweenScenes.aspect_ratio.
            if (name_scene != "MyAccount")
            {
                game_manager.Save_Find_Chief();  // Вызов метода Save_Find_Chief из game_manager.
                SceneManager.LoadScene("AboutChief");  // Загрузка сцены "AboutChief".
            }
            else
            {
                my_account.Load();  // Вызов метода Load из my_account.
            }
        }
    }

}
