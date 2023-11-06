using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ForPictureChief : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    private ScrollRect scrollRect;  // ������ �� ��������� ScrollRect.
    public static bool is_possible = true;  // ����������� ����������, �����������, �������� �� �������������� � �����������.
    public AddChief add;  // ������ �� ��������� AddChief.

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();  // ������� ��������� ScrollRect � ������������ �������.
    }

    public void OnBeginDrag(PointerEventData data)
    {
        scrollRect.OnBeginDrag(data);  // ��������� ����� OnBeginDrag � ���������� ScrollRect ��� ��������� ������ ��������������.
        is_possible = false;  // ������������� ����������� ���������� is_possible � false ��� �������������� ������ �������� � ����������.
    }

    public void OnDrag(PointerEventData data)
    {
        scrollRect.OnDrag(data);  // ��������� ����� OnDrag � ���������� ScrollRect ��� ��������� ��������������.
    }

    public void OnEndDrag(PointerEventData data)
    {
        scrollRect.OnEndDrag(data);  // ��������� ����� OnEndDrag � ���������� ScrollRect ��� ��������� ���������� ��������������.
        is_possible = true;  // ��������������� ����������� �������������� � �����������, ������������ is_possible � true.
    }

    public void OnScroll(PointerEventData data)
    {
        scrollRect.OnScroll(data);  // ��������� ����� OnScroll � ���������� ScrollRect ��� ��������� ����������.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (is_possible)
            add.SelectImage();  // �������� ����� SelectImage � ���������� AddChief ��� ����� (���� �������� �������������� � �����������).
    }

}
