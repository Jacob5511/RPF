using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCooking : MonoBehaviour
{
    // Для доступа к компоненту About_Recipe.
    About_Recipe load_recipes;

    // Метод, вызываемый при старте объекта.
    private void Start()
    {
        // Находим объект типа About_Recipe в сцене.
        load_recipes = FindObjectOfType<About_Recipe>();
    }

    // Метод для начала процесса приготовления.
    public void Start_Cooking()
    {
        // Вызываем метод Start_Cooking у объекта load_recipes.
        load_recipes.Start_Cooking();
    }

}
