using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    // ������ �� ������ About_Recipe.
    About_Recipe about_recipe;

    // �����, ���������� ��� ������ �������.
    private void Start()
    {
        // ������� ������ ���� About_Recipe � �����.
        about_recipe = GameObject.FindObjectOfType<About_Recipe>();
    }
    public void Star()
    {
        // �������� ����� Stars � ������� about_recipe, ��������� ������� ������ (gameObject) � �������� ���������.
        about_recipe.Stars(gameObject);
    }

}
