using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class LoadAboutChief : MonoBehaviour
{
    string url = "http://22.cshse.beget.tech/add_announcement";
    public RawImage chief_image; // ���� ��� ����������� ����������� ���-������
    public TMP_Text chief_name, views, score, short_about, phone_number, metro; // ���� ��� ��������� ���������� � ���-������
    public RectTransform content; // RectTransform, ������������ ��� ���������� ������� ��������
    public AspectRatioFitter image_aspect_ratio; // ��������� ��� ���������� ������������ ������ �����������

    private void Start()
    {
        Application.targetFrameRate = 60;
        chief_name.text = BetweenScenes.name; // ������������� ��� ���-������
        chief_image.texture = BetweenScenes.texture; // ������������� ����������� ���-������
        image_aspect_ratio.aspectRatio = BetweenScenes.aspect_ratio; // ������������� ����������� ������ �����������
        Load(); // �������� ����� Load() ��� �������� ���������� � ���-������
    }

    public void Load()
    {
        StartCoroutine(Load_About_Chief()); // ��������� �������� ���������� � ���-������
    }

    private IEnumerator Load_About_Chief()
    {
        // ������� ����� ��� �������� ������ �� ������
        WWWForm form = new WWWForm();
        form.AddField("username", chief_name.text); // ���������� ��� ���-������
        form.AddField("is_add", "False");
        form.AddField("action", "False");

        // ���������� POST-������ �� ��������� URL
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest(); // ���� ���������� �������

            if (www.result == UnityWebRequest.Result.Success)
            {
                string loaded = www.downloadHandler.text; // �������� ����� �� ������� � ��������� ��� � ������ Chief

                Chief answer_data = JsonUtility.FromJson<Chief>(loaded);

                // ��������� ��������� ���������� � ���-������
                chief_name.text = answer_data.name;
                views.text = answer_data.views;
                score.text = answer_data.score.ToString().Replace(",", ".");
                short_about.text = answer_data.short_about_chief;
                phone_number.text = answer_data.phone_number;
                metro.text = answer_data.metro;

                LayoutRebuilder.ForceRebuildLayoutImmediate(content); // ������������� ����� ��������
            }
            else
            {
                Debug.LogError("���� ������ �� ������� �������" + www.error + ", ���� ����� ������ �� ��� ������");
            }
            www.Dispose(); // ����������� �������, ��������� � ��������
        }
    }
}
