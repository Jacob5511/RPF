using UnityEngine;
using UnityEngine.SceneManagement;

public class LogHandler : MonoBehaviour
{
    public Animator anim; // ������ �� ��������� Animator ��� ���������� ���������
    private GameObject loading_screen; // ������ �� ������ ������ ��������

    // ���������� ��� ��������� �������
    private void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>(); // �������� ��������� Animator
        Application.logMessageReceived += HandleException; // ������������ ������� HandleException ��� ��������� ��������� �� �������
    }

    // ���������� ��� ���������� �������
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleException; // �������� ����������� ������� HandleException
    }

    // ������� ��� ��������� ��������� �� ������� � �����������
    void HandleException(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            anim.SetTrigger("must"); // ��������� �������� ������
            if (logString.ToLower().Contains("gateway 502"))
            {
                string current_scene = SceneManager.GetActiveScene().name; // �������� ��� ������� �����
                SceneManager.LoadScene(current_scene); // ������������� ������� �����
            }
            try
            {
                loading_screen = GameObject.FindGameObjectWithTag("loading_screen"); // �������� ����� ������ ������ ��������
                if (loading_screen.activeSelf)
                {
                    string current_scene = SceneManager.GetActiveScene().name; // �������� ��� ������� �����
                    SceneManager.LoadScene(current_scene); // ������������� ������� �����
                }
            }
            catch
            {
                // ������ ���� catch ��� ��������� ����������, ���� ���-�� ����� �� ���
            }
        }
    }
}