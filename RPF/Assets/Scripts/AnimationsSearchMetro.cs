using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimationsSearchMetro : MonoBehaviour
{
    public Animator arrow; // Аниматор стрелки
    public Animator panel; // Аниматор панели.
    public RectTransform content; // Ректрансформ содержимого панели.

    private void Anim_Open()
    {
        arrow.SetBool("is_open", true); // Установка анимации открытия стрелки.
        panel.SetBool("is_open", true); // Установка анимации открытия панели.
    }

    private void Anim_Close()
    {
        arrow.SetBool("is_open", false); // Установка анимации закрытия стрелки.
        panel.SetBool("is_open", false); // Установка анимации закрытия панели.
    }

    public void Open()
    {
        if (!panel.GetBool("is_open")) // Проверка, что панель не открыта.
            Anim_Open(); // Запустить анимацию открытия.
    }

    public void Close()
    {
        if (panel.GetBool("is_open")) // Проверка, что панель открыта.
            Anim_Close(); // Запустить анимацию закрытия.
    }

    public void Call()
    {
        if (panel.GetBool("is_open"))
            Anim_Close(); // Если панель открыта, закрыть её.
        else
            Anim_Open(); // Если панель закрыта, открыть её.
    }

    public void Change()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        // Обновление расположения элементов внутри панели с помощью LayoutRebuilder.
        Canvas.ForceUpdateCanvases();
        // Принудительное обновление холста для применения изменений макета.
    }
}
