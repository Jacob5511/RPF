using System.IO;
using UnityEngine;
using Unity.Notifications.Android;
using TMPro;
using UnityEngine.Android;

public class Timer : MonoBehaviour
{
    int notification_id;  // Идентификатор уведомления
    public int duration;  // Длительность таймера
    string for_what;  // Назначение таймера
    public bool is_play = false;  // Флаг, указывающий, работает ли таймер
    public GameObject cooking, not_cooking, time, play;  // Ссылки на игровые объекты
    TMP_Text time_text;  // Текст, отображающий оставшееся время

    private void Start()
    {
        // Проверка и запрос разрешения на отправку уведомлений (для Android)
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        time_text = time.GetComponent<TMP_Text>();
        duration = int.Parse(gameObject.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text);
        for_what = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
    }

    public void Play()
    {
        // Запуск или остановка таймера
        if (!is_play)
        {
            play.SetActive(false);
            CancelInvoke("Counter");
            InvokeRepeating("Counter", 1, 1);
            SendNotification();  // Отправка уведомления
            time.SetActive(true);
            is_play = true;
        }
        else
        {
            CancelInvoke("Counter");
            AndroidNotificationCenter.CancelNotification(notification_id);
            play.SetActive(true);
            time.SetActive(false);
            is_play = false;
        }
    }

    void Counter()
    {
        // Обратный отсчет времени
        if (is_play)
        {
            if (duration > 0)
                duration -= 1;
            else
                EndDuration();
            time_text.text = duration.ToString();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        // Обработка события приложения в фоновом режиме (приостановка или восстановление)
        if (pause)
        {
            CancelInvoke("Counter");
        }
        else
        {
            CancelInvoke("Counter");
            InvokeRepeating("Counter", 1, 1);
        }
    }

    void EndDuration()
    {
        // Обработка завершения времени таймера (здесь примерно вибрация)
        Handheld.Vibrate();
        //SendNotification();
    }

    public void Change()
    {
        // Смена состояния интерфейса
        cooking.SetActive(true);
        not_cooking.SetActive(false);
    }

    private void Awake()
    {
        // Регистрация канала уведомлений
        AndroidNotificationChannel channel = new AndroidNotificationChannel()
        {
            Name = "RPF",
            Description = "Таймер",
            Id = "timer",
            Importance = Importance.High
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendNotification()
    {
        // Отправка уведомления
        AndroidNotification notification = new AndroidNotification()
        {
            Title = "Время вышло",
            Text = "Время вышло!!! Пора продолжать готовить",
            FireTime = System.DateTime.Now.AddSeconds(duration),
            SmallIcon = "small",
            LargeIcon = "large"
        };

        notification_id = AndroidNotificationCenter.SendNotification(notification, "timer");
    }
}

[System.Serializable]
public class SaveTimer
{
    public string saved_timers; // Строка для хранения сериализованных данных таймеров
}