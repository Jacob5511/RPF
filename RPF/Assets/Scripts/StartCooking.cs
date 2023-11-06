using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCooking : MonoBehaviour
{
    // ��� ������� � ���������� About_Recipe.
    About_Recipe load_recipes;

    // �����, ���������� ��� ������ �������.
    private void Start()
    {
        // ������� ������ ���� About_Recipe � �����.
        load_recipes = FindObjectOfType<About_Recipe>();
    }

    // ����� ��� ������ �������� �������������.
    public void Start_Cooking()
    {
        // �������� ����� Start_Cooking � ������� load_recipes.
        load_recipes.Start_Cooking();
    }

}
