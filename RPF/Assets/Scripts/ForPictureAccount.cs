using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class ForPictureAccount : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    private ScrollRect scrollRect;  // Ссылка на компонент ScrollRect.
    public static bool is_possible = true;  // Статическая переменная, указывающая, доступно ли взаимодействие с интерфейсом.
    public Animator add_announcement;  // Ссылка на компонент анимации для добавления объявления.
    MyAccount my_account;  // Ссылка на компонент MyAccount.

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();  // Находит компонент ScrollRect в родительском объекте.
        my_account = FindObjectOfType<MyAccount>();  // Находит объект MyAccount в сцене.
    }

    public void OnBeginDrag(PointerEventData data)
    {
        scrollRect.OnBeginDrag(data);  // Запускает метод OnBeginDrag у компонента ScrollRect для обработки начала перетаскивания.
        is_possible = false;  // Устанавливает статическую переменную is_possible в false для предотвращения других действий в интерфейсе.
    }

    public void OnDrag(PointerEventData data)
    {
        scrollRect.OnDrag(data);  // Запускает метод OnDrag у компонента ScrollRect для обработки перетаскивания.
    }

    public void OnEndDrag(PointerEventData data)
    {
        scrollRect.OnEndDrag(data);  // Запускает метод OnEndDrag у компонента ScrollRect для обработки завершения перетаскивания.
        is_possible = true;  // Восстанавливает доступность взаимодействия с интерфейсом, устанавливая is_possible в true.
    }

    public void OnScroll(PointerEventData data)
    {
        scrollRect.OnScroll(data);  // Запускает метод OnScroll у компонента ScrollRect для обработки скроллинга.
    }

    public void DelteteResponse()
    {
        StartCoroutine(my_account.Get_My_Requests_To_Chief(true, gameObject.name));  // Запускает корутину Get_My_Requests_To_Chief с указанными параметрами.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (add_announcement != null && is_possible)  // Проверяет, существует ли объект add_announcement и доступно ли взаимодействие с интерфейсом.
        {
            if (add_announcement.GetBool("is_open"))  // Проверяет, открыто ли окно объявления.
                add_announcement.SetBool("is_open", false);  // Закрывает окно объявления, устанавливая значение анимации в false.
            else
                add_announcement.SetBool("is_open", true);  // Открывает окно объявления, устанавливая значение анимации в true.
            return;
        }
    }

}
