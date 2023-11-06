using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Add_Step_But : MonoBehaviour
{
    Animator anim;
    // Аниматор для управления анимациями объекта.

    public GameObject inputs, timers, delete_timer;
    // Ссылки на игровые объекты для инпутов, таймеров и кнопки удаления таймеров.

    Add_Step add_step;
    // Ссылка на компонент Add_Step для управления шагами в рецепте.

    public ImagePathScripts image_path_scripts;
    // Ссылка на компонент ImagePathScripts для работы с изображениями.

    private void Start()
    {
        anim = GetComponent<Animator>();
        // Получение ссылки на аниматор объекта.

        add_step = FindObjectOfType<Add_Step>().GetComponent<Add_Step>();
        // Получение ссылки на компонент Add_Step через поиск в сцене.
    }

    public void Change()
    {
        // Метод для смены состояния анимации объекта.

        if (anim.GetBool("is_open_add_step"))
            anim.SetBool("is_open_add_step", false);
        else
            anim.SetBool("is_open_add_step", true);
    }

    public void Delete_Timer()
    {
        // Метод для удаления последнего таймера.

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
        // Задержка перед обновлением макета шагов.
        StartCoroutine(add_step.DelayedLayoutRebuild());
    }

    public void Add_Inputs()
    {
        // Метод для добавления нового инпута таймера.

        GameObject inst = Instantiate(inputs, timers.transform);

        image_path_scripts.inputs_prefab.Add(inst);
        if (image_path_scripts.inputs_prefab.Count > 0)
        {
            timers.SetActive(true);
            delete_timer.SetActive(true);
        }

        // Обновление макета шагов.
        LayoutRebuilder.ForceRebuildLayoutImmediate(add_step.content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(add_step.content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(add_step.content.GetComponent<RectTransform>());
    }

}
