using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Panel_Units : MonoBehaviour
{
    // ������ �� �������� ������ ��� ������ ������ ���������.
    Animator panel;

    // �����, ���������� ��� ������ ����������.
    private void Start()
    {
        // ������� � ��������� ������ �� �������� ������ � ����� "panel_units".
        panel = GameObject.FindGameObjectWithTag("panel_units").GetComponent<Animator>();
    }

    // �����, ���������� ��� ��������� ������ ������ ���������.
    public void OnChanged()
    {
        // ��������� ������ �� ������� ������ � BetweenScenes.units.
        BetweenScenes.units = gameObject.gameObject;

        // ���� ������ �������, ��������� �; ����� ���������.
        if (panel.GetBool("is_open"))
            panel.SetBool("is_open", false);
        else
            panel.SetBool("is_open", true);
    }

    // ����� ��� ������ ������ ��������� "������".
    public void Grams()
    {
        // ������������� ��������� �������� � BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "������";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // ��������� ������.
        panel.SetBool("is_open", false);
    }

    // ����� ��� ������ ������ ��������� "����������".
    public void Millilitres()
    {
        // ������������� ��������� �������� � BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "����������";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // ��������� ������.
        panel.SetBool("is_open", false);
    }

    // ����� ��� ������ ������ ��������� "������ �����".
    public void Smal_Spoon()
    {
        // ������������� ��������� �������� � BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "������\n�����";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // ��������� ������.
        panel.SetBool("is_open", false);
    }

    // ����� ��� ������ ������ ��������� "�������� �����".
    public void BigSpoon()
    {
        // ������������� ��������� �������� � BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "��������\n�����";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // ��������� ������.
        panel.SetBool("is_open", false);
    }

    // ����� ��� ������ ������ ��������� "�����".
    public void Pieces()
    {
        // ������������� ��������� �������� � BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "�����";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // ��������� ������.
        panel.SetBool("is_open", false);
    }

}
