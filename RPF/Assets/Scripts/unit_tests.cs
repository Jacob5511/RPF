using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_tests : MonoBehaviour
{
    public Authorization auth;  // ��������� ��� �����������
    public MyAccount my_account;  // ��������� ��� ���������� ���������

    private void test1(string username, string password, string enter_url)
    {
        // ����� ��� ����������� ������������
        // username: ��� ������������
        // password: ������ ������������
        // enter_url: URL, �� �������� ������������ ������ �� �����������
        StartCoroutine(auth.SendRequest(username, Authorization.HashPassword(password), enter_url));
    }

    private void test2(string username)
    {
        // ����� ��� ����� ������������
        // username: ��� ������������
        StartCoroutine(auth.GetHash(username));
    }

    private void test3(string username)
    {
        // ����� ��� �������� ���������� � ������
        // username: ��� ������������ ������
        StartCoroutine(my_account.Load_About_Chief(username));
    }

    private void test4(string username)
    {
        // ����� ��� ��������� ���������� � ������������
        // username: ��� ������������
        StartCoroutine(my_account.GetFridge(username));
    }

    private void test5(string username)
    {
        // ����� ��� ��������� ���� �������� ������� �� ���������� �������
        // username: ��� ������������ �������
        StartCoroutine(my_account.Get_Responses(username));
    }

}