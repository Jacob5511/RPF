using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public static class BetweenScenes
{
    public static string recipes_id { get; set; } // ID �������
    public static Texture texture { get; set; } // ��������
    public static RawImage image { get; set; } // ������ ��� ����������� �����������
    public static ImagePathScripts image_path_scripts { get; set; } // �������� ��� ���������� ������ �����������
    public static string recipe_name { get; set; } // �������� �������
    public static string search_bar { get; set; } // ��������� ������
    public static string rating_from_one { get; set; } // ������� �� (1)
    public static string rating_from_two { get; set; } // ������� �� (2)
    public static string rating_to_one { get; set; } // ������� �� (1)
    public static string rating_to_two { get; set; } // ������� �� (2)
    public static string views_from { get; set; } // ��������� ��
    public static string views_to { get; set; } // ��������� ��
    public static string name { get; set; } // ��� ������������
    public static bool can_i_make_recipe { get; set; } // ����� �� �������� ������
    public static bool sort_by_views { get; set; } // ���������� �� ����������
    public static bool is_my_recipes { get; set; } // ��� ��� �������
    public static bool is_downloaded { get; set; } // ���� ������� ��������
    public static float aspect_ratio { get; set; } // ����������� ������
    public static string description { get; set; } // ��������
    public static string ingredients { get; set; } // �����������
    public static string timers { get; set; } // �������
    public static string links_to_pictures { get; set; } // ������ �� �����������
    public static string steps_descriptions { get; set; } // �������� �����
    public static bool is_from_about_recipe { get; set; } // ������� �� "� �������"
    public static string recipe_file_name { get; set; } // ��� ����� �������
    public static GameObject units { get; set; } // ������� (�����)
    public static string search_bar_find_chief { get; set; } // ��������� ������ ��� ������ ���-������
    public static string rating_from_one_find_chief { get; set; } // ������� �� (1) ��� ������ ���-������
    public static string rating_from_two_find_chief { get; set; } // ������� �� (2) ��� ������ ���-������
    public static string rating_to_one_find_chief { get; set; } // ������� �� (1) ��� ������ ���-������
    public static string rating_to_two_find_chief { get; set; } // ������� �� (2) ��� ������ ���-������
    public static string views_from_find_chief { get; set; } // ��������� �� ��� ������ ���-������
    public static string views_to_find_chief { get; set; } // ��������� �� ��� ������ ���-������
    public static string metro_find_chief { get; set; } // ����� ��� ������ ���-������
    public static float vertical_normalized_position { get; set; } = 1; // ������������ ��������������� �������
    public static List<string> sp_timers { get; set; } // ������� ��� �����
    public static string old_request { get; set; } // ������ ������
    public static string search_data { get; set; } // ������ ��� ������
    public static string metro { get; set; } // �����
    public static string price_from { get; set; } // ���� ��
    public static string price_to { get; set; } // ���� ��

}

[System.Serializable]
public class User
{
    public string username; // ��� ������������
    public string hash; // ��� ������
    public int count; // �������
    public string email; // ����������� �����
}

[System.Serializable]
public class Chief
{
    public string foto_path; // ���� � ����
    public string name; // ���
    public string views; // ���������
    public string score; // �������
    public string phone_number; // ����� ��������
    public string metro; // �����
    public string short_about_chief; // ������� ���������� � ���-������
    public string add_or_remove; // �������� ��� �������
}

