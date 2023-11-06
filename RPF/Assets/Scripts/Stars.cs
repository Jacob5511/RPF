using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    // Ссылка на объект About_Recipe.
    About_Recipe about_recipe;

    // Метод, вызываемый при старте объекта.
    private void Start()
    {
        // Находим объект типа About_Recipe в сцене.
        about_recipe = GameObject.FindObjectOfType<About_Recipe>();
    }
    public void Star()
    {
        // Вызываем метод Stars у объекта about_recipe, передавая текущий объект (gameObject) в качестве аргумента.
        about_recipe.Stars(gameObject);
    }

}
