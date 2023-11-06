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
        // Получаем ссылку на компонент AnimationsSearchMetro
        anim_search_metro = FindObjectOfType<AnimationsSearchMetro>();
    }

    public void Find()
    {
        // Получаем ссылку на компонент AnimationsSearchMetro
        TMP_InputField text = GameObject.FindGameObjectWithTag("metro").transform.GetChild(0).GetComponent<TMP_InputField>();
        
        // Устанавливаем текст поля ввода текста равным тексту name_station_for_prefab
        text.text = name_station_for_prefab.text;
        
        // Закрываем анимации и действия, связанные с поиском метро
        anim_search_metro.Close();
    }
}
