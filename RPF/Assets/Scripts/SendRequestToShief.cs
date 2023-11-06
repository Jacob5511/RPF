using UnityEngine;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SendRequestToShief : MonoBehaviour
{
    // ������ �� ������, ���� ����� ��������� ������.
    private string serverURL = "http://22.cshse.beget.tech/add_request";

    // ������ �� �������� ���������� ��� ����� ������ �������.
    public TMP_InputField request_name, metro, price;

    // ��������� ��� ����������� ��������� ��������� �������.
    public Animator not_allowed_same, success, all_field_must;

    // ���������� ��� ������� �������.
    private void Start()
    {
        // ������������� ������� ������� ������ �� 60 FPS (������ � �������).
        Application.targetFrameRate = 60;
    }

    // ����� ��� �������� �������.
    public void UploadRequest()
    {
        // ���������, ��� ��� ����������� ���� ���������.
        if (request_name.text == "" || metro.text == "" || price.text == "")
        {
            // ���� �����-���� ���� �� ���������, ��������� �������� "all_field_must".
            all_field_must.SetTrigger("must");
            return; // ��������� ���������� ������.
        }

        // ���� ��� ���� ���������, ���������� ������.
        StartCoroutine(SendRequest());
    }

    // ����� ��� �������� ������� �� ������.
    private IEnumerator SendRequest()
    {
        // ������� ����� ��� �������� ������.
        WWWForm form = new WWWForm();

        // ��������� ���������� � ������������ �� �����.
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;

        // ��������� ���� ������ � �����.
        form.AddField("username", username);
        form.AddField("request_name", request_name.text);
        form.AddField("metro", metro.text);
        form.AddField("price", price.text);

        // ������� � ���������� UnityWebRequest.
        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                if (answer == "this_request_already_exist")
                {
                    // ���� ������ ��� ����������, ��������� �������� "not_allowed_same".
                    not_allowed_same.SetTrigger("must");
                }
                else
                {
                    // ���� ������ ������� ���������, ��������� �������� "success" � ��������� � ������ ����� ����� 1.5 �������.
                    success.SetTrigger("must");
                    yield return new WaitForSeconds(1.5f);
                    SceneManager.LoadScene("MainScene");
                }
                Debug.Log("Request upload successful");
            }
            else
            {
                // � ������ ������ ��� �������� ������� ������� ��������� � �������.
                Debug.LogError("Server error: " + www.error + ", or you've already submitted this request");
            }
            www.Dispose();
        }
    }

}
