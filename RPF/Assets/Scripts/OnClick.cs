using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    // ������ �� ��������� ScrollRect ��� ���������� ����������.
    private ScrollRect scrollRect;

    // ���� ��� �������������� ������������� ������� � ���������.
    bool is_possible = true;

    // ������ �� ��������� ���� �������� �������, ������ � ����������.
    [SerializeField] private TMP_Text recipe_name, score, views;

    // ������ �� ��������� RawImage ��� ����������� ����������� �������.
    [SerializeField] private RawImage pick_foto;

    // �������� �� ������ ���������� � ������� � �����.
    GameManager game_manager;
    public Animator circle;
    LoadRecipes load_recipes;

    // �����, ���������� ��� ������ �������.
    private void Start()
    {
        // ������� � ��������� ������ �� ���������� � �������.
        game_manager = FindObjectOfType<GameManager>();
        scrollRect = GetComponentInParent<ScrollRect>();
        load_recipes = FindObjectOfType<LoadRecipes>();
    }

    // �����, ���������� ��� ����� �� �������� �������.
    public void onClick()
    {
        // ��������� ������� ��� �������� � ��������� ���������� � �������.
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
        // ��������� ��������-����������.
        StartCoroutine(game_manager.CheckInternetConnection(isConnected =>
        {
            if (!isConnected)
            {
                // ���� ��� ��������-����������, ���������� ��������� � ���������.
                load_recipes.no_internet.SetTrigger("must");
                if (!load_recipes.downloaded_recipes.isOn)
                    return;
            }
        }));

        // ��������� ����������� ������ ��� �������� � ��������� �����.
        BetweenScenes.texture = pick_foto.texture;
        BetweenScenes.recipe_file_name = gameObject.name;
        BetweenScenes.recipe_name = recipe_name.text;
        BetweenScenes.recipes_id = gameObject.transform.parent.name;

        // ��������� ��������� ����.
        game_manager.Save();

        // ��������� ��������� �����.
        SceneManager.LoadScene("AboutRecipe");
    }

    // ������ ��� ��������� ������, ��������� � ����������� ��� �������������� ��������.
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

        // ���� ��������� ����� ������, ��������� ������ ��������.
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

    // ����� ��� ��������� ����� �� ��������.
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick();
    }

}
