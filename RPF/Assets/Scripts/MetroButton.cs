using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MetroButton : MonoBehaviour
{
    public TMP_Text name_station_for_prefab;
    AnimationsSearchMetro anim_search_metro;

    private void Start()
    {
        // �������� ������ �� ��������� AnimationsSearchMetro
        anim_search_metro = FindObjectOfType<AnimationsSearchMetro>();
    }

    public void Find()
    {
        // �������� ������ �� ��������� AnimationsSearchMetro
        TMP_InputField text = GameObject.FindGameObjectWithTag("metro").transform.GetChild(0).GetComponent<TMP_InputField>();
        
        // ������������� ����� ���� ����� ������ ������ ������ name_station_for_prefab
        text.text = name_station_for_prefab.text;
        
        // ��������� �������� � ��������, ��������� � ������� �����
        anim_search_metro.Close();
    }
}
