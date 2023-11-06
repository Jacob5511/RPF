using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    string check_chief_url = "http://22.cshse.beget.tech/check_chief"; // URL ��� ��������, �������� �� ������������ ���-�������.
    string email_url = "http://22.cshse.beget.tech/email"; // URL ��� �������� ����������� �����.
    string change_password_url = "http://22.cshse.beget.tech/change_password"; // URL ��� ��������� ������.
    string get_code = ""; // ���������� ��� �������� ����������� ����.
    public GameObject login, registration; // ������ �� ������� ��� ����� � �����������.
    public Toggle can_i_make_recipe, sort_by_views, is_my_recipes, is_downloaded; // ������������� ��� ��������� ������.
    public TMP_InputField search_bar, rating_from_one, rating_from_two, rating_to_one, rating_to_two, views_from, views_to, metro, email, code, new_password, new_password_repeat,
        search_data, price_from, price_to, metro_main_for_chief; // ���� ����� � ��������� ��� ������, ����������� �����, ������ � ������ ������.
    public Animator for_other_people, who_is_it_for, find_shief, reg_button, recover_button, enter, enter_but, for_animation, enter_your_email, incorrect_email, code_sent_successfully,
        smth_went_wrong, invalid_code, password_updated_successfully, password_not_the_same; // ��������� ��� ��������.
    Authorization auth; // ������ �� ��������� �����������.

    private void Start()
    {
        auth = FindObjectOfType<Authorization>(); // ������� ��������� ����������� � �����.
    }

    public IEnumerator CheckInternetConnection(Action<bool> action)
    {
        // �������� ������� ��������-����������.
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            action(false);
        }
        else
        {
            Debug.Log("�������� ����");
            action(true);
        }
    }

    public void Registration()
    {
        // ������� ���� ����������� � ������ ���� �����.
        if (enter_your_email.GetBool("is_open"))
        {
            enter_but.SetBool("is_close", false);
            for_animation.SetBool("is_close", false);
            reg_button.SetBool("is_open", false);
            recover_button.SetBool("is_open", false);
            recover_button.SetBool("is_change", false);
            enter_your_email.SetBool("is_open", false);
            enter.SetBool("is_open", false);
            enter.SetBool("is_transit", false);
        }
        else
        {
            enter_but.SetBool("is_close", false);
            for_animation.SetBool("is_close", false);
            enter.SetBool("is_open", false);
            reg_button.SetBool("is_open", false);
            recover_button.SetBool("is_open", false);
            recover_button.SetBool("is_change", false);
            enter.SetBool("is_transit", false);
        }
    }

    public void Enter()
    {
        // ������� ���� ����� � ������ ���� �����������.
        enter_but.SetBool("is_close", true);
        for_animation.SetBool("is_close", true);
        enter.SetBool("is_open", true);
        recover_button.SetBool("is_open", true);
        reg_button.SetBool("is_open", true);
    }

    public void Send_Email()
    {
        // ������� ��������� ����������� ������.
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        if (email.text == "")
        {
            auth.all_fields_must.SetTrigger("must");
            return;
        }

        if (!IsValidEmail(email.text))
        {
            incorrect_email.SetTrigger("must");
            return;
        }

        StartCoroutine(SendEmail());
    }

    private IEnumerator SendEmail()
    {
        // �������� ������������ ������ � ����� �������������.
        WWWForm form = new WWWForm();
        form.AddField("email", email.text);

        using (UnityWebRequest www = UnityWebRequest.Post(email_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                string[] sp = response.Split(" ");
                string answer;
                answer = sp[0];
                get_code = sp[1];

                if (answer == "ok")
                {
                    code_sent_successfully.SetTrigger("must");
                    enter_your_email.SetTrigger("transit");
                }
                else if (response == "����� ����� ���")
                {
                    incorrect_email.SetTrigger("must");
                }
                else
                {
                    smth_went_wrong.SetTrigger("must");
                }
            }
            else
            {
                Debug.LogError("���� ������ �� ������� �������" + www.error);
            }
            www.Dispose();
        }
    }

    public void Check_Code()
    {
        // �������� ���������� ���� �������������.
        if (code.text == "")
        {
            auth.all_fields_must.SetTrigger("must");
            return;
        }

        if (code.text == get_code)
        {
            enter_your_email.SetTrigger("transit");
        }
        else
        {
            invalid_code.SetTrigger("must");
        }
    }

    public void Update_Passwords()
    {
        // ���������� ������.
        if (new_password.text == "" || new_password_repeat.text == "")
        {
            auth.all_fields_must.SetTrigger("must");
            return;
        }

        if (new_password.text == new_password_repeat.text)
        {
            StartCoroutine(UpdatePasswords());
        }
        else
        {
            print("������ �� ���������");
            password_not_the_same.SetTrigger("must");
        }
    }

    private IEnumerator UpdatePasswords()
    {
        // �������� ������� �� ��������� ������.
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);

        WWWForm form = new WWWForm();
        form.AddField("username", user.username);
        form.AddField("hash", Authorization.HashPassword(new_password.text));

        using (UnityWebRequest www = UnityWebRequest.Post(change_password_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;

                if (answer == "ok")
                {
                    password_updated_successfully.SetTrigger("must");
                }
                else
                {
                    smth_went_wrong.SetTrigger("must");
                }
            }
            else
            {
                smth_went_wrong.SetTrigger("must");
            }
            www.Dispose();
        }
    }

    public void Change_Steps_Recover_Password()
    {
        // ���� ����� ��������� �������� �������� � �������� �������������� ������
        enter_your_email.SetTrigger("transit");
    }

    public void Recover_Password()
    {
        // ���� ����� ��������� �������� �������������� ������, ��������� ����������� �������� ����������
        enter_your_email.SetBool("is_open", true);
        enter.SetBool("is_transit", true);
        recover_button.SetBool("is_change", true);
    }

    public void Enter_To_Account()
    {
        // ���� ����� ��������� �������� �������������� ������ � ���������� ������������ �� �������� �����
        enter.SetBool("is_transit", false);
        enter_your_email.SetBool("is_open", false);
        recover_button.SetBool("is_change", false);
    }

    public void Add_Recipe()
    {
        // ���� ����� ��������� ������, ��������� ��� ��������� ����������� ���� ��� ������ ���� ������� (��� ����)
        Save();
        Save_Find_Chief();

        if (who_is_it_for.GetBool("is_open"))
            Close();
        else
            Open();
    }

    public void Add_For_Other_People()
    {
        // ���� ����� �������������� ������������ �� �������� ���������� ������� ��� ������ �������������
        SceneManager.LoadScene("Add");
    }

    public void Add_For_Shief()
    {
        // ���� ����� �������������� ������������ �� �������� ���������� ������� ��� ���-�������
        SceneManager.LoadScene("AddForShief");
    }

    public void MainScene_Back()
    {
        // ���� ����� ��������� ������ � ���������� ������������ �� ������� ��������
        Save();
        Save_Find_Chief();
        SaveMainForChief();
        SceneManager.LoadScene("MainScene");
    }

    public void ForChief()
    {
        // ���� ����� ��������� ������, �������������� ������������ �� �������� �������� ���� ���-������ � ��������� ��������
        Save();
        Save_Find_Chief();
        BetweenScenes.vertical_normalized_position = 1f;
        StartCoroutine(CheckChief());
    }

    private IEnumerator CheckChief()
    {
        // ���� ����� ��������� ��������, �������� �� ������������ ���-�������, � �������������� ��� ��������������� �������
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        WWWForm form = new WWWForm();
        form.AddField("username", user.username);

        using (UnityWebRequest www = UnityWebRequest.Post(check_chief_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;

                if (answer == "ok")
                {
                    SceneManager.LoadScene("MainForChief");
                }
                else
                {
                    SceneManager.LoadScene("ForChief");
                }
            }
            else
            {
                Debug.LogError("���� ������ �� ������� �������" + www.error);
            }
            www.Dispose();
        }
    }

    public void Back_To_FindChief()
    {
        // ���� ����� ��������� ������ � �������������� ������������ �� �������� ������ ���-������
        Save();
        Save_Find_Chief();
        SceneManager.LoadScene("FindChief");
    }

    public void Exit_From_Account()
    {
        // ���� ����� ��������� ����� ������������, ���������� ����������� ������ � ���������� ������������ �� �������� �����
        User user = new User();
        user.count = 50;
        string json = JsonUtility.ToJson(user);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "user.txt"), json);

        BetweenScenes.search_bar = "";
        BetweenScenes.rating_from_one = "";
        BetweenScenes.rating_from_two = "";
        BetweenScenes.rating_to_one = "";
        BetweenScenes.rating_to_two = "";
        BetweenScenes.views_from = "";
        BetweenScenes.views_to = "";
        BetweenScenes.can_i_make_recipe = false;
        BetweenScenes.sort_by_views = false;
        BetweenScenes.is_my_recipes = false;
        BetweenScenes.is_downloaded = false;

        BetweenScenes.search_bar_find_chief = search_bar.text;
        BetweenScenes.rating_from_one_find_chief = rating_from_one.text;
        BetweenScenes.rating_from_two_find_chief = rating_from_two.text;
        BetweenScenes.rating_to_one_find_chief = rating_to_one.text;
        BetweenScenes.rating_to_two_find_chief = rating_to_two.text;
        BetweenScenes.views_from_find_chief = views_from.text;
        BetweenScenes.views_to_find_chief = views_to.text;
        BetweenScenes.metro_find_chief = metro.text;


        SceneManager.LoadScene("SampleScene");
    }

    public void MainForChief()
    {
        // ���� ����� ��������� ������ � �������������� ������������ �� �������� "MainForChief"
        SaveMainForChief();
        SceneManager.LoadScene("MainForChief");
    }

    public void AddAnnouncement()
    {
        // ���� ����� ��������� ������ � �������������� ������������ �� �������� ���������� ���������� ���-������
        SaveMainForChief();
        SceneManager.LoadScene("ShortChief");
    }

    public void Open()
    {
        // ���� ����� ��������� ����������� ���� ��� ������ ���� �������
        for_other_people.SetBool("is_open", true);
        who_is_it_for.SetBool("is_open", true);
        find_shief.SetBool("is_open", true);
    }

    public void Close()
    {
        // ���� ����� ��������� ����������� ���� ��� ������ ���� �������
        for_other_people.SetBool("is_open", false);
        who_is_it_for.SetBool("is_open", false);
        find_shief.SetBool("is_open", false);
    }

    public void ToMyAccount()
    {
        // ���� ����� ��������� ������ � �������������� ������������ �� �������� ������ ��������
        Save();
        Save_Find_Chief();
        SceneManager.LoadScene("MyAccount");
    }

    public void ToMyChiefAccount()
    {
        // ���� ����� ��������� ������ � �������������� ������������ �� �������� �������� ���-������
        SaveMainForChief();
        SceneManager.LoadScene("MyChiefAccount");
    }

    public void Save_Find_Chief()
    {
        // ���� ����� ��������� ������� ������ ���-������, ���� ������������ ��������� �� �������� ������ ���-������
        if (SceneManager.GetActiveScene().name == "FindChief")
        {
            BetweenScenes.search_bar_find_chief = search_bar.text;
            BetweenScenes.rating_from_one_find_chief = rating_from_one.text;
            BetweenScenes.rating_from_two_find_chief = rating_from_two.text;
            BetweenScenes.rating_to_one_find_chief = rating_to_one.text;
            BetweenScenes.rating_to_two_find_chief = rating_to_two.text;
            BetweenScenes.views_from_find_chief = views_from.text;
            BetweenScenes.views_to_find_chief = views_to.text;
            BetweenScenes.metro_find_chief = metro.text;
        }
    }

    public void SaveMainForChief()
    {
        // ���� ����� ��������� ������� �������� �������� ��� ���-������, ���� ������������ ��������� �� ���� ��������
        if (SceneManager.GetActiveScene().name == "MainForChief")
        {
            BetweenScenes.search_data = search_data.text;
            BetweenScenes.price_from = price_from.text;
            BetweenScenes.price_to = price_to.text;
            BetweenScenes.metro = metro_main_for_chief.text;
        }
    }

    public void Save()
    {
        // ���� ����� ��������� ������� �� ������� �������� (MainScene)
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            BetweenScenes.search_bar = search_bar.text;
            BetweenScenes.rating_from_one = rating_from_one.text;
            BetweenScenes.rating_from_two = rating_from_two.text;
            BetweenScenes.rating_to_one = rating_to_one.text;
            BetweenScenes.rating_to_two = rating_to_two.text;
            BetweenScenes.views_from = views_from.text;
            BetweenScenes.views_to = views_to.text;
            BetweenScenes.can_i_make_recipe = can_i_make_recipe.isOn;
            BetweenScenes.sort_by_views = sort_by_views.isOn;
            BetweenScenes.is_my_recipes = is_my_recipes.isOn;
            BetweenScenes.is_downloaded = is_downloaded.isOn;
        }
    }
}

