using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Add_Step : MonoBehaviour
{
    public GameObject add_step, step_prefab, parentObject, send_recipe, mainFoto, delete_step, delete_ingredient, ingedient_inputs_prefab;
    // Ссылки на объекты в сцене.

    public RectTransform content, parent_for_ingredient;
    // Ссылки на компоненты RectTransform для контента и родителя ингредиентов.

    public int count = 0;
    // Счетчик шагов.

    public ScrollRect scrollRect;
    // Ссылка на компонент ScrollRect.

    public List<GameObject> objects = new List<GameObject>();
    // Список объектов (шагов) в рецепте.

    public List<GameObject> ingredients = new List<GameObject>();
    // Список объектов (ингредиентов) в рецепте.

    private void Start()
    {
        // Инициализация списка объектов рецепта, добавление главной фотографии.

        objects.Add(mainFoto);
    }
    public void add_Step()
    {
        // Метод для добавления нового шага в рецепт.

        count += 1;
        GameObject new_step = Instantiate(step_prefab, parentObject.transform);
        TMP_Text step_text = new_step.GetComponentInChildren<TMP_Text>();
        step_text.text = $"Шаг {count}";
        objects.Add(new_step);

        // Обновление макета и вертикальной позиции прокрутки.
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        scrollRect.verticalNormalizedPosition = 0f;

        if (count > 0)
            delete_step.SetActive(true);
    }

    public void Delete_Step()
    {
        // Метод для удаления последнего шага из рецепта.

        count -= 1;
        Destroy(objects[objects.Count - 1]);
        objects.Remove(parentObject.transform.GetChild(parentObject.transform.childCount - 1).gameObject);

        // Задержка перед обновлением макета.
        StartCoroutine(DelayedLayoutRebuild());

        if (count == 0)
            delete_step.SetActive(false);
    }

    public void Add_Step_Ingredient()
    {
        // Метод для добавления нового ингредиента в текущий шаг.

        GameObject instantiate_ingredient = Instantiate(ingedient_inputs_prefab, parent_for_ingredient);
        ingredients.Add(instantiate_ingredient);
        delete_ingredient.SetActive(true);

        // Обновление макета ингредиентов.
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public void Delete_Step_Ingredient()
    {
        // Метод для удаления последнего ингредиента из текущего шага.

        Destroy(ingredients[ingredients.Count - 1]);
        ingredients.Remove(ingredients[ingredients.Count - 1]);

        if (ingredients.Count == 1)
            delete_ingredient.SetActive(false);

        // Задержка перед обновлением макета.
        StartCoroutine(DelayedLayoutRebuild());
    }

    public IEnumerator DelayedLayoutRebuild()
    {
        // Задержка перед обновлением макета.

        yield return null; // Ждем один кадр.
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

}
