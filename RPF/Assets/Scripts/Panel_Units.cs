using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Panel_Units : MonoBehaviour
{
    // Ссылка на аниматор панели для выбора единиц измерения.
    Animator panel;

    // Метод, вызываемый при старте компонента.
    private void Start()
    {
        // Находим и сохраняем ссылку на аниматор панели с тегом "panel_units".
        panel = GameObject.FindGameObjectWithTag("panel_units").GetComponent<Animator>();
    }

    // Метод, вызываемый при изменении выбора единиц измерения.
    public void OnChanged()
    {
        // Сохраняем ссылку на текущий объект в BetweenScenes.units.
        BetweenScenes.units = gameObject.gameObject;

        // Если панель открыта, закрываем её; иначе открываем.
        if (panel.GetBool("is_open"))
            panel.SetBool("is_open", false);
        else
            panel.SetBool("is_open", true);
    }

    // Метод для выбора единиц измерения "граммы".
    public void Grams()
    {
        // Устанавливаем текстовое значение в BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "граммы";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // Закрываем панель.
        panel.SetBool("is_open", false);
    }

    // Метод для выбора единиц измерения "миллилитры".
    public void Millilitres()
    {
        // Устанавливаем текстовое значение в BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "миллилитры";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // Закрываем панель.
        panel.SetBool("is_open", false);
    }

    // Метод для выбора единиц измерения "чайная ложка".
    public void Smal_Spoon()
    {
        // Устанавливаем текстовое значение в BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "чайная\nложка";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // Закрываем панель.
        panel.SetBool("is_open", false);
    }

    // Метод для выбора единиц измерения "столовая ложка".
    public void BigSpoon()
    {
        // Устанавливаем текстовое значение в BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "столовая\nложка";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // Закрываем панель.
        panel.SetBool("is_open", false);
    }

    // Метод для выбора единиц измерения "штуки".
    public void Pieces()
    {
        // Устанавливаем текстовое значение в BetweenScenes.units.
        BetweenScenes.units.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = "штуки";
        BetweenScenes.units.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "";

        // Закрываем панель.
        panel.SetBool("is_open", false);
    }

}
