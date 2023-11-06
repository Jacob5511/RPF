using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public GameObject sort_panel;
    public Animator sort_panel_anim; // �������� ��� ���������� ������� ����������.
    public Animator left_arrow, right_arrow;

    void Start()
    {
        sort_panel_anim = sort_panel.GetComponent <Animator>();
        // ��������� ��������� �� ������� ������ ���������� ��� ������� �����.
    }

    public void Open_Panel()
    {
        left_arrow.SetTrigger("must");
        right_arrow.SetTrigger("must");
        // ������ �������� ��� �������.

        if (!sort_panel_anim.GetBool("is_open"))
            sort_panel_anim.SetBool("is_open", true);
        else
            sort_panel_anim.SetBool("is_open", false);
        // �������� ��� �������� ������ ���������� � ����������� �� �������� ���������.
    }

}
