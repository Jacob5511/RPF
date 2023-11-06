using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class MyChiefAccount : MonoBehaviour
{
    public RawImage photo, animated_image;
    public TMP_Text chief_name, short_about, phone_number, metro, score, views;
    string url = "http://22.cshse.beget.tech/chief_account";
    string my_responses_url = "http://22.cshse.beget.tech/my_responses";
    public RectTransform content;
    public GameObject scroll_for_anim, old_scroll, background, announcement_prefab, up, short_about_object, down, back_area;
    public Animator image_for_anim_anim_component, photo_anim, back, name_anim, number, metro_anim, short_about_anim, next_area;
    public VerticalLayoutGroup content_vlg;

    // ���������� ��� ������� �����.
    private void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(LoadAboutChief());
    }

    void Animation(bool value)
    {
        animated_image.texture = photo.texture;
        AspectRatioFitter asp = animated_image.GetComponent<AspectRatioFitter>();
        AspectRatioFitter asp_photo = photo.GetComponent<AspectRatioFitter>();
        asp.aspectRatio = asp_photo.aspectRatio;
        image_for_anim_anim_component.SetBool("is_open", value);
        back.SetBool("is_open", value);
        photo_anim.SetBool("is_open", value);
        name_anim.SetBool("is_open", value);
        number.SetBool("is_open", value);
        metro_anim.SetBool("is_open", value);
        short_about_anim.SetBool("is_open", value);
        next_area.SetBool("is_open", value);
    }

    // ��������� �������� ��� ����������� �������������� ���������� � ���-������.
    public void MakeChanges()
    {
        scroll_for_anim.SetActive(true);
        Animation(true);
        Invoke("TurnOn", 1f);
    }

    // ���������� ������� � �������� ����������� ���������� � ���-������.
    public void Back()
    {
        content_vlg.enabled = false;
        Animation(false);
        Invoke("TurnOff", 1.1f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
    public void Back_To_Account()
    {
        up.SetActive(true);
        short_about_object.SetActive(true);
        down.SetActive(true);
        back_area.SetActive(false);
        int childCount = content.childCount;
        for (int i = childCount - 1; i >= 4; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
    void TurnOn()
    {
        content_vlg.enabled = true;
    }
    void TurnOff()
    {
        scroll_for_anim.SetActive(false);
    }

    // ��������� �������, �� ������� ������������ ������������.
    public void My_Responses()
    {
        up.SetActive(false);
        short_about_object.SetActive(false);
        down.SetActive(false);
        back_area.SetActive(true);
        StartCoroutine(LoadMyResponses());
    }

    // ��������� �������, �� ������� ������������ ������������, �� ��������� ��������������.
    public IEnumerator LoadMyResponses(string id = "-1")
    {
        WWWForm form = new WWWForm();

        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        form.AddField("username", username);
        form.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(my_responses_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                int childCount = content.childCount;
                for (int i = childCount - 1; i >= 4; i--)
                {
                    Destroy(content.GetChild(i).gameObject);
                }
                Load(answer);
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }

    // ��������� ������ �������� � ��������� �� JSON-������.
    private void Load(string json)
    {
        // ����������� JSON-������ � ������� ��������.
        RootObjectRequests requestData = JsonUtility.FromJson<RootObjectRequests>("{\"requests\":" + json + "}");
        for (int i = 0; i < requestData.requests.Length; i++)
        {
            RecipeDataRequests request = requestData.requests[i];

            // ������� ��������� ������� ������ ������� � ��������� ��� �������.
            GameObject instantiatedPrefab = Instantiate(announcement_prefab, content);
            TextMeshProUGUI recipe_name = instantiatedPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI metro_tmp = instantiatedPrefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI price = instantiatedPrefab.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            instantiatedPrefab.name = request.id;
            recipe_name.text = request.request_name;
            metro_tmp.text = request.metro;
            price.text = request.price + " ���";

            // ��������� ������� ��������� ����������.
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }

    // ��������� ������� ��������� ����������.
    private IEnumerator LoadImage(string imageURL, RawImage image)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                float c = (texture.width * 1.0f) / texture.height;
                AspectRatioFitter image_aspect = image.GetComponent<AspectRatioFitter>();
                image_aspect.aspectRatio = c;
                image.texture = texture;
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
            www.Dispose();
        }
    }

    // ��������� ������ � ���-������ � ������� � ���������� �� � ����������.
    private IEnumerator LoadAboutChief()
    {
        // ��������� ������ ��� �������� ������ � ���-������.
        WWWForm form = new WWWForm();

        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));
        User user = JsonUtility.FromJson<User>(json);
        string username = user.username;
        form.AddField("username", username);

        // ���������� ������ �� ������.
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // ���������� ������ �� ������.
                string answer = www.downloadHandler.text;
                Chief chief = JsonUtility.FromJson<Chief>(answer);

                // ��������� ����������� ���-������ � ���������� ��� � ���������� RawImage.
                StartCoroutine(LoadImage($"http://22.cshse.beget.tech/foto/{Path.GetFileName(chief.foto_path)}", photo));

                // ��������� ��������� ���� ������� � ���-������.
                chief_name.text = chief.name;
                short_about.text = chief.short_about_chief;
                phone_number.text = chief.phone_number;
                metro.text = chief.metro;
                score.text = chief.score.ToString().Replace(",", ".");
                views.text = chief.views;

                // ��������� ������� ��������� ����������.
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            }
            else
            {
                Debug.LogError("Image upload failed: " + www.error);
            }
            www.Dispose();
        }
    }
}
