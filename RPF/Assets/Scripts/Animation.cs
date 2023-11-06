using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public GameObject sort_panel;
    public Animator sort_panel_anim; // Аниматор для управления панелью фильтрации.
    public Animator left_arrow, right_arrow;

    void Start()
    {
        sort_panel_anim = sort_panel.GetComponent <Animator>();
        // Получение аниматора из объекта панели фильтрации при запуске сцены.
    }

    public void Open_Panel()
    {
        left_arrow.SetTrigger("must");
        right_arrow.SetTrigger("must");
        // Запуск анимаций для стрелок.

        if (!sort_panel_anim.GetBool("is_open"))
            sort_panel_anim.SetBool("is_open", true);
        else
            sort_panel_anim.SetBool("is_open", false);
        // Открытие или закрытие панели сортировки в зависимости от текущего состояния.
    }

}
