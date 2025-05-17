# ENGLISH VERSION


# Yakov Dergachev - "RPF"
## User Scenarios

**Group:** 11 - I - 3  
**Email:** yakov.dergachev.06@mail.ru  
**VK:** [www.vk.com/ydergachev0](https://www.vk.com/ydergachev0)  

---

### Scenario 1 — User Registration / Login

**If the user does not have an account:**  
1. User enters a username to log into the system  
2. User enters a password  
3. User selects their type: chef or client  
4. User clicks the submit button  
5. If the username already exists, the system notifies the user and asks for a new username  
6. If the second username is also taken, the system repeats step 1 until a valid username is provided  
7. If all data is valid, the system confirms registration and redirects the user to the main page  

**If the user already has an account:**  
1. User opens the app  
2. If the user has not logged in for a long time, they enter their username and password  
3. User clicks the submit button  
4. If username and password match, the system confirms login and redirects to the main page  
5. If the user logged in recently, the app automatically redirects to the main page  

---

### Scenario 2 — Viewing Recipes

1. User opens the app  
2. Selects the "Search" tab  
3. If the user has internet access, the app shows all available recipes online  
4. If no internet, the app shows saved offline recipes  
5. User searches by ingredients, recipe name, or selects "most popular dishes"  
6. User selects a recipe to cook  
7. App displays detailed recipe info including ingredients and step-by-step instructions  

---

### Scenario 3 — Creating Own Recipes

1. User opens the app  
2. Clicks the "Create Recipe" button  
3. Chooses "For other people"  
4. Fills in recipe title, ingredients, step-by-step instructions, adds timers if needed  
5. Optionally adds a photo of the dish  
6. Clicks the submit button  
7. If all required fields are filled, the system confirms the successful addition  

---

### Scenario 4 — Client Creates a Chef Request

1. User opens the app and logs in  
2. Clicks "Create Recipe"  
3. Selects "Find a Chef"  
4. Enters dish name, metro station, and desired price  
5. Confirms the order  
6. If all fields are filled, the system notifies about successful request creation  
7. The request appears in the chefs' feed on the main screen  

---

### Scenario 5 — Chef Creates a Client Offer

1. User opens the app and logs in  
2. Switches to chef mode  
3. Selects "Add Offer" tab  
4. App shows how the offer will appear to clients  
5. If changes are needed, edits info in personal account  
6. Clicks "Publish"  
7. App confirms successful publication  

---

### Scenario 6 — Client Orders a Chef

1. User opens the app and logs in  
2. Goes to the "Find a Chef" tab  
3. Searches for chefs by name  
4. Sorts chefs by rating, views, metro station  
5. Selects a suitable chef  
6. Views detailed chef info  
7. Contacts the chef by phone  

---

### Scenario 7 — Chef Finds Clients

1. Chef opens the app and switches to chef mode  
2. Views all active client requests  
3. Searches requests by dish name  
4. Sorts requests by metro station and price  
5. Selects a suitable request and clicks "Add"  
6. The offer appears in the chef's responses  
7. Client checks "My Responses" tab, sees notification, and contacts the chef by phone  

---

### Scenario 8 — Deleting Recipes

1. User logs into their account  
2. Goes to the recipe list page  
3. Filters recipes by "My Recipes"  
4. Finds the recipe to delete  
5. Clicks the "Delete" button next to the recipe  
6. System asks for deletion confirmation  
7. If confirmed, recipe is removed from the database  
8. System shows a success message and updates the recipe list  
9. If canceled, no changes are made and user stays on the recipe list page  

---

### Scenario 9 — Recipes You Can Cook

1. User opens the app  
2. Filters recipes by "Recipes I can make with my fridge ingredients"  
3. App shows recipes that can be made from current fridge ingredients  
4. To update fridge contents, user goes to "Personal Account"  
5. Clicks "My Fridge"  
6. Adds, deletes, or updates ingredient quantities  
7. Saves changes  

# RUSSIAN VERSION

# Дергачве Яков - "RPF"
# Пользовательские сценарии

### Группа: 11 - И - 3
### Электронная почта: yakov.dergachev.06@mail.ru
### VK: www.vk.com/ydergachev0



### [ Сценарий 1 - Регистрация / Вход пользователя ]

Если пользователь не имеет аккаунт:
1. Пользователь вводит логин, с которым он будет заходить в систему
2. Пользователь вводит пароль, с которым он будет заходить в систему
3. Пользователь вводит свой тип: он повар или клиент
4. Пользователь нажимает кнопку отправки данных
5. Если выбранный логин уже существует в системе, то пользователю сообщается об этом и предлагается придумать новый логин
6. Если второй логин уже занят, то система зацикливает пункт 1 то тех пор, пока пользователь не введет удовлетворительный логин
7. Если все введённые данные соответствуют требованиям регистрации, то система сообщает пользователю об успешной регистрации перенаправляет его на главную страницу

Если пользователь имеет аккаунт:
1. Польхователь открывае приложение
2. Если пользователь долго не входил в приложение
3. Пользователь вводит логин, с которым он будет заходить в систему
4. Пользователь вводит пароль, с которым он будет заходить в систему
5. Пользователь нажимает кнопку отправки данных
6. Если логин существует и пароль соответствует логину (является правильным), то система сообщает пользователю об успешном вхождении и перенаправляет его на главную страницу
7. Если пользователь недавно вводил, то приложение автоматически перенаправляет пользователя на главную страницу

### [ Сценарий 2 - Просмотр рецептов ]

1. Пользователь открывает приложение
2. Выбирает вкладку "Поиск"
3. Если у пользователя есть доступ к интернету, то приложение отображает список всех доступных рецептов онлайн
4. Если у пользователя нет доступа к интернету, то приложение отображает список всех доступных рецептов в офлайн-режиме, которые были сохранены ранее
5. Пользователь выбирает поиск по ингредиентам или Пользователь выбирает поиск по названию блюда или Пользователь выбирает "список самых популярных блюд"
6. Пользователь выбирает рецепт, который хочет приготовить
7. Приложение отображает подробную информацию о рецепте, такую как список ингредиентов и пошаговые инструкции

### [ Сценарий 3 - Создание своих рецептов ]

1. Пользователь открывает приложение
2. Нажимает на кнопку "Создать рецепт"
3. Пользователь выбирает для кого этот рецепт "Для других людей"
4. Пользователь заполняет название рецепта, список ингредиентов и пошаговые инструкции. Добавляет таймеры по необходиости
5. Пользователь имеет возможность добавить фотографию блюда
6. Пользователь нажимает на кнопку отправки рецепта
7. Если все необходимые поля заполнены, то система сообщает пользователю об успешном добавлении рецепта

### [ Сценарий 4 - Клиент создает обьявление для повара ]

1. Пользователь заходит в приложение
2. Авторизируется
3. Нажимает на кнопку "Создать рецепт"
4. Пользователь выбирает для кого этот рецепт "Найти повара"
5. Пользователь вводит название блюда, метро, желаемую цену
6. Пользователь подтверждает заказ
7. Если все поля заполнены, то система оповещает пользователя об успешном добавлении заказа
8. Обьявление (заказ) появляется у поваров в "ленте" (на главном экране)

### [ Сценарий 5 - Повар создает обьявление для клиента ]

1. Пользователь заходит в приложение
2. Авторизируется
3. Переходит в режим для повара
3. Повар выбирает вкладку "Добавить обьявление"
4. Приложение показывает как будет выглядеть заявка повара на странице у клиента
5. Если повар хочет что то подправить, то он идет в личный кабинет и подправляет
6. Повар нажимает кнопку "Опубликовать"
7. Приложение выводит сообщение о том, что все успешно опубликовалось


### [ Сценарий 6 - Клиент заказывает повара ]

1. Пользователь открывает приложение
2. Авторизируется
3. Клиент переходит на вкладку "Поиск повара"
4. Клиент может найти повара по имени
5. Клиент может отсортировать поваров по рейтингу, просмотрам, метро
6. Если клиент нашел подходящего повара, то он выбирает его
7. Приложение отображает информацию о поваре
8. Пользователь связывается с поваром по номеру телефона

### [ Сценарий 7 - Повар заказывает клиента ]

1. Повар открывает приложение
2. Переходит в режим для повара
3. Приложение отображает все действующие запросы клиентов
4. Повар может найти запрос по названию блюда
5. Повар может отсортировать заявки по метро и цене
6. Если повар нашел подходящую заявку, то он ее выбирает и нажимает на кнопку "добавить"
7. Обьявление добавляется во все отклики повара
8. Клиент проверят вкладку "Мои отклики" и, если видит уведомление о том, что его выбрал повар, то они связываются по номеру телефона повара

### [ Сценарий 8 - Удаление рецептов ]

1. Пользователь входит в свой аккаунт
2. Пользователь переходит на страницу со списком всех рецептов
3. Пользователь сортирует рецепты по ключу "мои рецепты"
4. Пользователь находит рецепт, которы хочет удалить
5. Пользователь нажимает на кнопку "Удалить" рядом с названием рецепта
6. Система запрашивает у пользователя подтверждение удаления
7. Если пользователь соглашается удалить рецепт, то система удаляет его из базы данных приложения
8. Система выводит сообщение об успешном удалении рецепта и обновляет страницу со списком рецептов
9. Если пользователь не соглашается удалить рецепт, то система не удаляет его и возвращает пользователя на страницу со списком рецептов

### [ Сценарий 9 - Список рецептов, которые вы можете приготовить ]

1. Пользователь входит в приложение
2. Пользователь сортирует рецепты по ключу "рецепты, которые я могу приготовить из холодильника"
3. Приложение отображает рецепты, которые пользватель может приготовить из продуктов, которые есть у него в холодильнике
4. Если пользователь хочет обновить холодильник, то он переходит в "личный кабинет"
5. Пользователь нажимает кнопку "мой холодильник"
6. Пользователь может добавить, удалить ингредиент или обновить количество ингредиентов
7. Пользователь сохраняет изменения

