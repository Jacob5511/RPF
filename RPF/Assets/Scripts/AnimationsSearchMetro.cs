using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimationsSearchMetro : MonoBehaviour
{
    public Animator arrow; // �������� �������
    public Animator panel; // �������� ������.
    public RectTransform content; // ������������ ����������� ������.

    private void Anim_Open()
    {
        arrow.SetBool("is_open", true); // ��������� �������� �������� �������.
        panel.SetBool("is_open", true); // ��������� �������� �������� ������.
    }

    private void Anim_Close()
    {
        arrow.SetBool("is_open", false); // ��������� �������� �������� �������.
        panel.SetBool("is_open", false); // ��������� �������� �������� ������.
    }

    public void Open()
    {
        if (!panel.GetBool("is_open")) // ��������, ��� ������ �� �������.
            Anim_Open(); // ��������� �������� ��������.
    }

    public void Close()
    {
        if (panel.GetBool("is_open")) // ��������, ��� ������ �������.
            Anim_Close(); // ��������� �������� ��������.
    }

    public void Call()
    {
        if (panel.GetBool("is_open"))
            Anim_Close(); // ���� ������ �������, ������� �.
        else
            Anim_Open(); // ���� ������ �������, ������� �.
    }

    public void Change()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        // ���������� ������������ ��������� ������ ������ � ������� LayoutRebuilder.
        Canvas.ForceUpdateCanvases();
        // �������������� ���������� ������ ��� ���������� ��������� ������.
    }
}
