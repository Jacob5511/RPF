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
    private ScrollRect scrollRect;  // ������ �� ��������� ScrollRect.
    public static bool is_possible = true;  // ����������� ����������, �����������, �������� �� �������������� � �����������.
    public Animator add_announcement;  // ������ �� ��������� �������� ��� ���������� ����������.
    MyAccount my_account;  // ������ �� ��������� MyAccount.

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();  // ������� ��������� ScrollRect � ������������ �������.
        my_account = FindObjectOfType<MyAccount>();  // ������� ������ MyAccount � �����.
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

    public void DelteteResponse()
    {
        StartCoroutine(my_account.Get_My_Requests_To_Chief(true, gameObject.name));  // ��������� �������� Get_My_Requests_To_Chief � ���������� �����������.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (add_announcement != null && is_possible)  // ���������, ���������� �� ������ add_announcement � �������� �� �������������� � �����������.
        {
            if (add_announcement.GetBool("is_open"))  // ���������, ������� �� ���� ����������.
                add_announcement.SetBool("is_open", false);  // ��������� ���� ����������, ������������ �������� �������� � false.
            else
                add_announcement.SetBool("is_open", true);  // ��������� ���� ����������, ������������ �������� �������� � true.
            return;
        }
    }

}
