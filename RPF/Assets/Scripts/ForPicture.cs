using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ForPicture : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    private ScrollRect scrollRect;  // ���������� ���������� ScrollRect.

    public static bool is_possible = true;  // ���������� ����������� ���������� is_possible � ��������� � �������� � true.
    private string add_response_url = "http://22.cshse.beget.tech/add_response";  // ������ � URL ��� �������� ������.

    public Add add;  // ���������� ���������� ���� Add.
    public bool is_announcement;  // ������ ����������, �����������, �������� �� �����������.
    public Animator add_announcement;  // ���������� ���������� ���� Animator ��� �������� ����������.
    Animator success;  // ���������� ���������� ���� Animator ��� �������� ��������� ��������.
    MyChiefAccount mca;  // ���������� ���������� ���� MyChiefAccount.
    GameManager game_manager;  // ���������� ���������� ���� GameManager.
    MyAccount my_account;  // ���������� ���������� ���� MyAccount.
    public GameObject foto, chief_name, star, eye, views, score, about_announcecment;  // ���������� ���������� GameObject ��� ��������� ���������.
    public Animator circle;

    private void Start()
    {
        game_manager = FindObjectOfType<GameManager>();  // ����� ������� GameManager � ����� � ������������ ��� ���������� game_manager.
        mca = FindObjectOfType<MyChiefAccount>();  // ����� ������� MyChiefAccount � ����� � ������������ ��� ���������� mca.
        scrollRect = GetComponentInParent<ScrollRect>();  // ��������� ���������� ScrollRect �� ������������� ������� � ������������ ��� ���������� scrollRect.
        my_account = FindObjectOfType<MyAccount>();  // ����� ������� MyAccount � ����� � ������������ ��� ���������� my_account.

        try
        {
            success = GameObject.FindGameObjectWithTag("success").GetComponent<Animator>();  // ������� ����� ������ � ����� "success" � �������� ��������� Animator.
        }
        catch
        {
            return;  // � ������ ������, ����� �� ������� Start.
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        scrollRect.OnBeginDrag(data);  // ����� ������ OnBeginDrag ���������� ScrollRect.
        is_possible = false;  // ��������� �������� is_possible � false.
    }

    public void OnDrag(PointerEventData data)
    {
        scrollRect.OnDrag(data);  // ����� ������ OnDrag ���������� ScrollRect.
    }

    public void OnEndDrag(PointerEventData data)
    {
        scrollRect.OnEndDrag(data);  // ����� ������ OnEndDrag ���������� ScrollRect.
        is_possible = true;  // ��������� �������� is_possible � true.
    }

    public void OnScroll(PointerEventData data)
    {
        scrollRect.OnScroll(data);  // ����� ������ OnScroll ���������� ScrollRect.
    }

    public void Delete_Response_For_Announcement()
    {
        StartCoroutine(mca.LoadMyResponses(gameObject.name));  // ������ �������� LoadMyResponses �� ������� mca � ���������� ����� �������� �������.
    }

    public void Add_Response_For_Announcement()
    {
        add_announcement.SetBool("is_open", false);  // ��������� ��������� is_open ������� add_announcement � false.
        StartCoroutine(Add_Response());  // ������ �������� Add_Response.
    }

    private IEnumerator Add_Response()
    {
        WWWForm form = new WWWForm();  // �������� ����� ����� ��� �������� ������.
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "user.txt"));  // ������ JSON-����� �� ���� Application.persistentDataPath.
        User user = JsonUtility.FromJson<User>(json);  // �������������� JSON-������ � ������ User.
        string username = user.username;  // ��������� ����� ������������ �� ������� User.
        form.AddField("username", username);  // ���������� ������ � �����.
        form.AddField("id", gameObject.name);  // ���������� ������ � �����.

        using (UnityWebRequest www = UnityWebRequest.Post(add_response_url, form))  // �������� POST-������� �� ��������� URL � �������������� �����.
        {
            yield return www.SendWebRequest();  // �������� ���������� �������.

            if (www.result == UnityWebRequest.Result.Success)  // ���� ������ �������� �������.
            {
                success.SetTrigger("must");  // ������ �������� "must" ��� ������� success.
            }
            else
            {
                Debug.LogError("���� ������ �� ������� �������" + www.error + ", ���� ����� ������ �� ��� ������");  // ����� ��������� �� ������.
            }
            www.Dispose();  // ������������ �������� �������.
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (add_announcement != null && is_possible)
        {
            if (add_announcement.GetBool("is_open"))
                add_announcement.SetBool("is_open", false);
            else
                add_announcement.SetBool("is_open", true);
            return;
        }
        if (is_possible && !is_announcement)
            add.SelectImage();
        else if (is_possible && is_announcement && circle != null && !circle.GetBool("is_circling"))
            Cause();
    }

    public void Cause()
    {
        if (is_possible)
        {
            string name_scene = SceneManager.GetActiveScene().name;  // ��������� ����� �������� �����.
            string chief_name = gameObject.transform.GetChild(2).GetComponent<TMP_Text>().text;  // ��������� ����� "����" �� ��������� �������.
            RawImage chief_image = gameObject.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();  // ��������� ����������� "����" �� ��������� �������.
            float aspect_ratio = gameObject.transform.GetChild(1).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio;  // ��������� ����������� ������ �����������.
            BetweenScenes.name = chief_name;  // ���������� ����� "����" ���������� BetweenScenes.name.
            BetweenScenes.texture = chief_image.texture;  // ���������� �������� ����������� ���������� BetweenScenes.texture.
            BetweenScenes.aspect_ratio = aspect_ratio;  // ���������� ����������� ������ ���������� BetweenScenes.aspect_ratio.
            if (name_scene != "MyAccount")
            {
                game_manager.Save_Find_Chief();  // ����� ������ Save_Find_Chief �� game_manager.
                SceneManager.LoadScene("AboutChief");  // �������� ����� "AboutChief".
            }
            else
            {
                my_account.Load();  // ����� ������ Load �� my_account.
            }
        }
    }

}
