using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    // Ссылка на компонент ScrollRect для управления прокруткой.
    private ScrollRect scrollRect;

    // Флаг для предотвращения множественных нажатий и прокрутки.
    bool is_possible = true;

    // Ссылки на текстовые поля названия рецепта, оценки и просмотров.
    [SerializeField] private TMP_Text recipe_name, score, views;

    // Ссылка на компонент RawImage для отображения изображения рецепта.
    [SerializeField] private RawImage pick_foto;

    // Ссылания на другие компоненты и объекты в сцене.
    GameManager game_manager;
    public Animator circle;
    LoadRecipes load_recipes;

    // Метод, вызываемый при старте объекта.
    private void Start()
    {
        // Находим и сохраняем ссылки на компоненты и объекты.
        game_manager = FindObjectOfType<GameManager>();
        scrollRect = GetComponentInParent<ScrollRect>();
        load_recipes = FindObjectOfType<LoadRecipes>();
    }

    // Метод, вызываемый при клике на элементе рецепта.
    public void onClick()
    {
        // Проверяем условия для перехода к подробной информации о рецепте.
        if (!is_possible || circle.GetBool("is_circling")) return;
        if (load_recipes.downloaded_recipes.isOn)
            BetweenScenes.is_downloaded = true;
        else
            BetweenScenes.is_downloaded = false;
        if (load_recipes.parentObject.childCount <= 25)
        {
            BetweenScenes.vertical_normalized_position = scrollRect.verticalNormalizedPosition;
        }
        else
        {
            BetweenScenes.vertical_normalized_position = 1f;
        }
        // Проверяем интернет-соединение.
        StartCoroutine(game_manager.CheckInternetConnection(isConnected =>
        {
            if (!isConnected)
            {
                // Если нет интернет-соединения, показываем сообщение и завершаем.
                load_recipes.no_internet.SetTrigger("must");
                if (!load_recipes.downloaded_recipes.isOn)
                    return;
            }
        }));

        // Сохраняем необходимые данные для передачи в следующую сцену.
        BetweenScenes.texture = pick_foto.texture;
        BetweenScenes.recipe_file_name = gameObject.name;
        BetweenScenes.recipe_name = recipe_name.text;
        BetweenScenes.recipes_id = gameObject.transform.parent.name;

        // Сохраняем состояние игры.
        game_manager.Save();

        // Загружаем следующую сцену.
        SceneManager.LoadScene("AboutRecipe");
    }

    // Методы для обработки начала, окончания и перемещения при перетаскивании элемента.
    public void OnBeginDrag(PointerEventData data)
    {
        scrollRect.OnBeginDrag(data);
        is_possible = false;
    }

    public void OnDrag(PointerEventData data)
    {
        scrollRect.OnDrag(data);
    }

    public void OnEndDrag(PointerEventData data)
    {
        scrollRect.OnEndDrag(data);
        is_possible = true;

        // Если прокрутка внизу списка, загрузить больше рецептов.
        if (scrollRect.verticalNormalizedPosition < 0f)
        {
            int child_count = load_recipes.parentObject.childCount;
            StartCoroutine(load_recipes.Main_Load_Ricipes(load_recipes.parentObject.transform.GetChild(child_count - 1).gameObject.name));
        }
    }

    public void OnScroll(PointerEventData data)
    {
        scrollRect.OnScroll(data);
    }

    // Метод для обработки клика на элементе.
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick();
    }

}
