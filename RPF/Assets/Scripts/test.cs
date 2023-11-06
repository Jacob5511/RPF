using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace US.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class test : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerUpHandler
    {
        private ScrollRect _scrollRect;
        private TMP_InputField _input;
        private bool _isDragging;
        private bool _preventScrollRectDrag;

        private void Start()
        {
            // Получаем ссылку на компонент ScrollRect в родительском объекте и компонент TMP_InputField в текущем объекте
            _scrollRect = GetComponentInParent<ScrollRect>();
            _input = GetComponent<TMP_InputField>();
            _input.DeactivateInputField();  // Выключаем компонент ввода текста
            _input.onDeselect.AddListener(_ => _preventScrollRectDrag = false); // Подписываемся на событие "onDeselect"
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (_preventScrollRectDrag)
                return;

            // Начало перетаскивания - перенаправляем событие в ScrollRect
            _scrollRect.OnBeginDrag(data);
            _isDragging = true;

            _input.DeactivateInputField(); // Выключаем компонент ввода текста
        }

        public void OnDrag(PointerEventData data)
        {
            // Продолжение перетаскивания - перенаправляем событие в ScrollRect
            _scrollRect.OnDrag(data);
        }

        public void OnEndDrag(PointerEventData data)
        {
            // Окончание перетаскивания - перенаправляем событие в ScrollRect
            _scrollRect.OnEndDrag(data);
            _isDragging = false;
        }

        public void OnScroll(PointerEventData data)
        {
            // Обработка скроллинга - перенаправляем событие в ScrollRect
            _scrollRect.OnScroll(data);
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (!_isDragging && !data.dragging)
            {
                // Если не происходит перетаскивания и событие не связано с перетаскиванием
                // Включаем компонент ввода текста и предотвращаем перетаскивание ScrollRect
                _input.ActivateInputField();
                _preventScrollRectDrag = true;
            }
        }
    }

}
