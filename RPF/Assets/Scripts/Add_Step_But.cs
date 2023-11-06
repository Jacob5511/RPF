using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Add_Step_But : MonoBehaviour
{
    Animator anim;
    // �������� ��� ���������� ���������� �������.

    public GameObject inputs, timers, delete_timer;
    // ������ �� ������� ������� ��� �������, �������� � ������ �������� ��������.

    Add_Step add_step;
    // ������ �� ��������� Add_Step ��� ���������� ������ � �������.

    public ImagePathScripts image_path_scripts;
    // ������ �� ��������� ImagePathScripts ��� ������ � �������������.

    private void Start()
    {
        anim = GetComponent<Animator>();
        // ��������� ������ �� �������� �������.

        add_step = FindObjectOfType<Add_Step>().GetComponent<Add_Step>();
        // ��������� ������ �� ��������� Add_Step ����� ����� � �����.
    }

    public void Change()
    {
        // ����� ��� ����� ��������� �������� �������.

        if (anim.GetBool("is_open_add_step"))
            anim.SetBool("is_open_add_step", false);
        else
            anim.SetBool("is_open_add_step", true);
    }

    public void Delete_Timer()
    {
        // ����� ��� �������� ���������� �������.

        if (image_path_scripts.inputs_prefab.Count > 0)
        {
            Destroy(image_path_scripts.inputs_prefab[image_path_scripts.inputs_prefab.Count - 1]);
            image_path_scripts.inputs_prefab.Remove(image_path_scripts.inputs_prefab[image_path_scripts.inputs_prefab.Count - 1]);

            if (image_path_scripts.inputs_prefab.Count == 0)
            {
                timers.SetActive(false);
                delete_timer.SetActive(false);
            }
        }
        // �������� ����� ����������� ������ �����.
        StartCoroutine(add_step.DelayedLayoutRebuild());
    }

    public void Add_Inputs()
    {
        // ����� ��� ���������� ������ ������ �������.

        GameObject inst = Instantiate(inputs, timers.transform);

        image_path_scripts.inputs_prefab.Add(inst);
        if (image_path_scripts.inputs_prefab.Count > 0)
        {
            timers.SetActive(true);
            delete_timer.SetActive(true);
        }

        // ���������� ������ �����.
        LayoutRebuilder.ForceRebuildLayoutImmediate(add_step.content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(add_step.content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(add_step.content.GetComponent<RectTransform>());
    }

}
