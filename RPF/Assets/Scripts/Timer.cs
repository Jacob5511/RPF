using System.IO;
using UnityEngine;
using Unity.Notifications.Android;
using TMPro;
using UnityEngine.Android;

public class Timer : MonoBehaviour
{
    int notification_id;  // ������������� �����������
    public int duration;  // ������������ �������
    string for_what;  // ���������� �������
    public bool is_play = false;  // ����, �����������, �������� �� ������
    public GameObject cooking, not_cooking, time, play;  // ������ �� ������� �������
    TMP_Text time_text;  // �����, ������������ ���������� �����

    private void Start()
    {
        // �������� � ������ ���������� �� �������� ����������� (��� Android)
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
        // ������ ��� ��������� �������
        if (!is_play)
        {
            play.SetActive(false);
            CancelInvoke("Counter");
            InvokeRepeating("Counter", 1, 1);
            SendNotification();  // �������� �����������
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
        // �������� ������ �������
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
        // ��������� ������� ���������� � ������� ������ (������������ ��� ��������������)
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
        // ��������� ���������� ������� ������� (����� �������� ��������)
        Handheld.Vibrate();
        //SendNotification();
    }

    public void Change()
    {
        // ����� ��������� ����������
        cooking.SetActive(true);
        not_cooking.SetActive(false);
    }

    private void Awake()
    {
        // ����������� ������ �����������
        AndroidNotificationChannel channel = new AndroidNotificationChannel()
        {
            Name = "RPF",
            Description = "������",
            Id = "timer",
            Importance = Importance.High
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendNotification()
    {
        // �������� �����������
        AndroidNotification notification = new AndroidNotification()
        {
            Title = "����� �����",
            Text = "����� �����!!! ���� ���������� ��������",
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
    public string saved_timers; // ������ ��� �������� ��������������� ������ ��������
}