using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ForPictureChief : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    private ScrollRect scrollRect;  // Ссылка на компонент ScrollRect.
    public static bool is_possible = true;  // Статическая переменная, указывающая, доступно ли взаимодействие с интерфейсом.
    public AddChief add;  // Ссылка на компонент AddChief.

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();  // Находит компонент ScrollRect в родительском объекте.
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (is_possible)
            add.SelectImage();  // Вызывает метод SelectImage у компонента AddChief при клике (если доступно взаимодействие с интерфейсом).
    }

}
