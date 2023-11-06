using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public static class BetweenScenes
{
    public static string recipes_id { get; set; } // ID рецепта
    public static Texture texture { get; set; } // Текстура
    public static RawImage image { get; set; } // Объект для отображения изображения
    public static ImagePathScripts image_path_scripts { get; set; } // Сценарий для управления путями изображений
    public static string recipe_name { get; set; } // Название рецепта
    public static string search_bar { get; set; } // Поисковый запрос
    public static string rating_from_one { get; set; } // Рейтинг от (1)
    public static string rating_from_two { get; set; } // Рейтинг от (2)
    public static string rating_to_one { get; set; } // Рейтинг до (1)
    public static string rating_to_two { get; set; } // Рейтинг до (2)
    public static string views_from { get; set; } // Просмотры от
    public static string views_to { get; set; } // Просмотры до
    public static string name { get; set; } // Имя пользователя
    public static bool can_i_make_recipe { get; set; } // Можно ли готовить рецепт
    public static bool sort_by_views { get; set; } // Сортировка по просмотрам
    public static bool is_my_recipes { get; set; } // Это мои рецепты
    public static bool is_downloaded { get; set; } // Файл рецепта загружен
    public static float aspect_ratio { get; set; } // Соотношение сторон
    public static string description { get; set; } // Описание
    public static string ingredients { get; set; } // Ингредиенты
    public static string timers { get; set; } // Таймеры
    public static string links_to_pictures { get; set; } // Ссылки на изображения
    public static string steps_descriptions { get; set; } // Описания шагов
    public static bool is_from_about_recipe { get; set; } // Открыто из "О рецепте"
    public static string recipe_file_name { get; set; } // Имя файла рецепта
    public static GameObject units { get; set; } // Объекты (юниты)
    public static string search_bar_find_chief { get; set; } // Поисковый запрос для поиска шеф-повара
    public static string rating_from_one_find_chief { get; set; } // Рейтинг от (1) для поиска шеф-повара
    public static string rating_from_two_find_chief { get; set; } // Рейтинг от (2) для поиска шеф-повара
    public static string rating_to_one_find_chief { get; set; } // Рейтинг до (1) для поиска шеф-повара
    public static string rating_to_two_find_chief { get; set; } // Рейтинг до (2) для поиска шеф-повара
    public static string views_from_find_chief { get; set; } // Просмотры от для поиска шеф-повара
    public static string views_to_find_chief { get; set; } // Просмотры до для поиска шеф-повара
    public static string metro_find_chief { get; set; } // Метро для поиска шеф-повара
    public static float vertical_normalized_position { get; set; } = 1; // Вертикальная нормализованная позиция
    public static List<string> sp_timers { get; set; } // Таймеры для шагов
    public static string old_request { get; set; } // Старый запрос
    public static string search_data { get; set; } // Данные для поиска
    public static string metro { get; set; } // Метро
    public static string price_from { get; set; } // Цена от
    public static string price_to { get; set; } // Цена до

}

[System.Serializable]
public class User
{
    public string username; // Имя пользователя
    public string hash; // Хеш пароля
    public int count; // Счетчик
    public string email; // Электронная почта
}

[System.Serializable]
public class Chief
{
    public string foto_path; // Путь к фото
    public string name; // Имя
    public string views; // Просмотры
    public string score; // Рейтинг
    public string phone_number; // Номер телефона
    public string metro; // Метро
    public string short_about_chief; // Краткая информация о шеф-поваре
    public string add_or_remove; // Добавить или удалить
}

