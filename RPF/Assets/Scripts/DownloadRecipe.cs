using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadRecipe : MonoBehaviour
{
    // URL для загрузки рецепта с сервера
    string download_recipe_url = "http://22.cshse.beget.tech/download_recipe";
    
    // Идентификатор рецепта
    string recipe_id = BetweenScenes.recipes_id;
    
    // Путь для сохранения данных рецепта
    string path;
    
    // Ссылка на компонент AboutRecipe, который отображает информацию о рецепте
    public About_Recipe about_recipe;
    
    // Аниматор для анимации удаления рецепта и успешной загрузки рецепта
    public Animator recipe_deleted, recipe_downloaded;
   
    // Метод для загрузки рецепта
    public void Download_Recipe()
    {
        StartCoroutine(Download());
    }
    
    // Метод для выполнения загрузки рецепта
    private IEnumerator Download()
    {
        // Путь для сохранения данных рецепта в локальном хранилище
        path = Path.Combine(Application.persistentDataPath, "recipes");
        
        // Путь для сохранения изображений рецепта
        string images_save_path = Path.Combine(path, "images");
        
        // Путь для сохранения всех данных, кроме изображений
        string all_except_images_save_path = Path.Combine(path, "text_info.txt");
        
        // Создаем директорию для сохранения изображений, если она не существует
        if (!Directory.Exists(Path.GetDirectoryName(images_save_path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(images_save_path));
        }
        
        // Форма для отправки запроса на сервер
        WWWForm form = new WWWForm();
        form.AddField("recipe_id", recipe_id);

        using (UnityWebRequest www = UnityWebRequest.Post(download_recipe_url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string answer = www.downloadHandler.text;
                bool flag = false;
                DownloadRecipeData download_recipe_data = JsonUtility.FromJson<DownloadRecipeData>(answer);
                
                // Сохранили текстовую информацию, короче все, кроме изображений.
                try
                {
                    string json = File.ReadAllText(all_except_images_save_path);
                    DownloadRecipeObject download_recipe_object = JsonUtility.FromJson<DownloadRecipeObject>(json);
                    
                    // Создаем новый объект DownloadRecipeData
                    DownloadRecipeData newRecipe = JsonUtility.FromJson<DownloadRecipeData>(answer);
                    for (int i = 0; i < download_recipe_object.recipes.Length; i++)
                    {
                        if (newRecipe.recipe_id == download_recipe_object.recipes[i].recipe_id)
                        {
                            flag = true;
                            break;
                        }    
                    }

                    // Копируем существующие данные в новый список рецептов
                    List<DownloadRecipeData> recipeList = new List<DownloadRecipeData>(download_recipe_object.recipes);

                    // Добавляем новый объект в список
                    recipeList.Add(newRecipe);

                    // Обновляем массив рецептов в объекте DownloadRecipeObject
                    download_recipe_object.recipes = recipeList.ToArray();
                    answer = JsonUtility.ToJson(download_recipe_object);
                }
                catch
                {
                    // Файл еще не существует, создаем новый объект DownloadRecipeObject
                    DownloadRecipeObject download_recipe_object = new DownloadRecipeObject();

                    // Создаем новый объект DownloadRecipeData
                    DownloadRecipeData newRecipe = JsonUtility.FromJson<DownloadRecipeData>(answer);

                    // Добавляем новый объект в список рецептов
                    List<DownloadRecipeData> recipeList = new List<DownloadRecipeData>();
                    recipeList.Add(newRecipe);

                    // Обновляем массив рецептов в объекте DownloadRecipeObject
                    download_recipe_object.recipes = recipeList.ToArray();
                    answer = JsonUtility.ToJson(download_recipe_object);
                }
                if (flag)
                    yield break;

                // Сохраняем информацию о рецепте, кроме изображений
                File.WriteAllText(all_except_images_save_path, answer);

                // Сохраняем изображения рецепта
                string path_for_each_image = Path.Combine(images_save_path, Path.GetFileName(download_recipe_data.main_picture));

                Texture2D main_picture_texture = (Texture2D)about_recipe.main_foto.texture;
                byte[] bytes = main_picture_texture.EncodeToJPG();
                saveImage(path_for_each_image, bytes);
                string[] links_to_description_images = download_recipe_data.links_to_pictures.Split(" ");
                for (int i = 4; i < about_recipe.content.transform.childCount - 2; i++)
                {
                    path_for_each_image = Path.Combine(images_save_path, Path.GetFileName(links_to_description_images[i - 4]));
                    Texture2D description_image_texture = (Texture2D)about_recipe.content.GetChild(i).GetChild(2).GetChild(0).GetComponent<RawImage>().texture;
                    byte[] description_image_bytes = description_image_texture.EncodeToJPG();
                    saveImage(path_for_each_image, description_image_bytes);
                }

                // Запускаем анимацию успешной загрузки рецепта
                recipe_downloaded.SetTrigger("must");
            }
            else
            {
                Debug.LogError("Либо ошибка со стороны сервера" + www.error + ", либо такой рецепт вы уже писали");
            }
            www.Dispose();
        }
    }

    // Метод для удаления рецепта из локального хранилища
    public void DeleteDownloadRecipe()
    {
        string recipe_id = about_recipe.recipe_id;
        path = Path.Combine(Application.persistentDataPath, "recipes");
        string json = File.ReadAllText(Path.Combine(path, "text_info.txt"));
        string images_save_path = Path.Combine(path, "images");
        DownloadRecipeObject download_recipe_object = JsonUtility.FromJson<DownloadRecipeObject>(json);
        List<DownloadRecipeData> new_download_sp = new List<DownloadRecipeData>();

        // Проходим по списку сохраненных рецептов и удаляем выбранный
        for (int i = 0; i < download_recipe_object.recipes.Length; i++)
        {
            DownloadRecipeData download_recipe_data = download_recipe_object.recipes[i];
            if (download_recipe_data.recipe_id != recipe_id)
            {
                new_download_sp.Add(download_recipe_data);
            }
            else
            {
                // Удаляем изображения, связанные с рецептом
                string path_to_delete_main_picture = Path.Combine(images_save_path, Path.GetFileName(download_recipe_data.main_picture));
                print(path_to_delete_main_picture);
                File.Delete(path_to_delete_main_picture);
                foreach (string link_to_picture in download_recipe_object.recipes[i].links_to_pictures.Split(" "))
                {
                    if (link_to_picture == "")
                        break;
                    string path_to_delete = Path.Combine(images_save_path, Path.GetFileName(link_to_picture));
                    print(path_to_delete);
                    File.Delete(path_to_delete);
                }
            }
        }

        // Обновляем список сохраненных рецептов
        download_recipe_object.recipes = new_download_sp.ToArray();
        string new_json = JsonUtility.ToJson(download_recipe_object);
        print(new_json);
        string all_except_images_save_path = Path.Combine(path, "text_info.txt");
        File.WriteAllText(all_except_images_save_path, new_json);

        // Запускаем анимацию успешного удаления рецепта
        recipe_deleted.SetTrigger("must");
    }

    // Метод для сохранения изображения
    void saveImage(string path, byte[] imageBytes)
    {
        //Создаем директорию если не существует
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        try
        {
            File.WriteAllBytes(path, imageBytes);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogError("Error: " + e.Message);
        }
    }
}

[Serializable]
public class DownloadRecipeData
{
    public string recipe_id;           // Идентификатор рецепта.
    public string main_picture;        // Главное изображение рецепта.
    public string recipe_name;        // Название рецепта.
    public string score;              // Оценка рецепта.
    public string views;              // Просмотры рецепта.
    public string description;         // Описание рецепта.
    public string ingredients;         // Ингредиенты рецепта.
    public string steps_descriptions;  // Описания шагов приготовления.
    public string links_to_pictures;   // Ссылки на изображения рецепта.
    public string timers;             // Таймеры рецепта.
}

[Serializable]
public class DownloadRecipeObject
{
    public DownloadRecipeData[] recipes;  // Массив данных о скачанных рецептах.
}

[Serializable]
public class RecipeObject
{
    public RecipeDataAboutRecipe[] recipes;  // Массив данных о рецептах
}

