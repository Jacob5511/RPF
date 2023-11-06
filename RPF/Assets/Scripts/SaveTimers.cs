using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SaveTimers : MonoBehaviour
{
    // ������ �� ���������, � ������� �������� �������.
    public GameObject content;

    // ������ ��������-��������.
    List<GameObject> sp = new List<GameObject>();

    // ������ ����� ��� �������� ������� ��������.
    List<string> sp_timers = new List<string>();

    // ����� ��� ��������� ���� �������� �� ����������.
    private void Get_All_Timers()
    {
        sp.Clear();
        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject child = content.transform.GetChild(i).gameObject;
            if (child.name == "Step_About_Recipe(Clone)")
            {
                sp.Add(child.transform.GetChild(1).gameObject);
            }
        }
    }

    // ����� ��� ���������� ������� �������� � ������� ������.
    private void Save_Timers()
    {
        Get_All_Timers();
        sp_timers.Clear();
        for (int i = 0; i < sp.Count; i++)
        {
            GameObject timer = sp[i];
            for (int j = 1; j < timer.transform.childCount; j++)
            {
                DateTime date_time = DateTime.Now;
                int time = date_time.Hour * 3600 + date_time.Minute * 60 + date_time.Second;
                sp_timers.Add(timer.transform.GetChild(j).GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text + ' ' + time.ToString());
            }
        }
        BetweenScenes.sp_timers = sp_timers;
    }

    // ����� ��� �������� ������� �������� �� ������.
    private void Upload_Timers()
    {
        List<string> sp_timers_for_upload = BetweenScenes.sp_timers;
        Get_All_Timers();
        List<GameObject> sp_timers_object = new List<GameObject>();
        for (int i = 0; i < sp.Count; i++)
        {
            GameObject child = sp[i];
            for (int j = 1; j < child.transform.childCount; j++)
            {
                sp_timers_object.Add(child.transform.GetChild(j).gameObject);
            }
        }
        for (int i = 0; i < sp_timers_object.Count; i++)
        {
            string[] time_start = sp_timers_for_upload[i].Split(' ');
            DateTime date_time = DateTime.Now;
            int time_now = date_time.Hour * 3600 + date_time.Minute * 60 + date_time.Second;
            Timer timer = sp_timers_object[i].GetComponent<Timer>();
            if (timer.is_play && timer.duration > 0)
            {
                if ((int.Parse(time_start[0]) - (time_now - int.Parse(time_start[1]))) > 0)
                {
                    timer.duration = (int.Parse(time_start[0]) - (time_now - int.Parse(time_start[1])));
                    sp_timers_object[i].transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = (int.Parse(time_start[0]) - (time_now - int.Parse(time_start[1]))).ToString();
                }
                else
                {
                    timer.duration = 0;
                }
            }
        }
    }

    // �����, ���������� ��� ������������ ����������.
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // ��������� ��������� ��������.
            Save_Timers();
        }
        else
        {
            // ��������� ��������� ��������.
            Upload_Timers();
        }
    }

}
