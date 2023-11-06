using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_tests : MonoBehaviour
{
    public Authorization auth;  // Компонент для авторизации
    public MyAccount my_account;  // Компонент для управления аккаунтом

    private void test1(string username, string password, string enter_url)
    {
        // Метод для регистрации пользователя
        // username: имя пользователя
        // password: пароль пользователя
        // enter_url: URL, по которому отправляется запрос на регистрацию
        StartCoroutine(auth.SendRequest(username, Authorization.HashPassword(password), enter_url));
    }

    private void test2(string username)
    {
        // Метод для входа пользователя
        // username: имя пользователя
        StartCoroutine(auth.GetHash(username));
    }

    private void test3(string username)
    {
        // Метод для загрузки информации о поваре
        // username: имя пользователя повара
        StartCoroutine(my_account.Load_About_Chief(username));
    }

    private void test4(string username)
    {
        // Метод для получения информации о холодильнике
        // username: имя пользователя
        StartCoroutine(my_account.GetFridge(username));
    }

    private void test5(string username)
    {
        // Метод для получения всех откликов поваров на объявления клиента
        // username: имя пользователя клиента
        StartCoroutine(my_account.Get_Responses(username));
    }

}
