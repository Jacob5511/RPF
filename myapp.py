# -*- coding: utf-8 -*-

import random
from flask import make_response, send_file, Flask, render_template, jsonify, request, redirect
from sqlalchemy import create_engine, text
import sys
import os
import io
import site
import base64
import subprocess
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from werkzeug.utils import secure_filename

username = "cshse_22"
passwd = "wxrdqQ1!"
db_name = "cshse_22"
engine = create_engine("mysql://" + username + ":" + passwd + "@localhost/" + db_name + "?charset=utf8mb4",
                       pool_size=10,
                       max_overflow=20, echo=True)

app = Flask(__name__)


@app.route('/')
def index():
    # smtp_obj = smtplib.SMTP('smtp.gmail.com', 587)
    # smtp_obj.starttls()
    # smtp_obj.login('yakov.dergachev.00@gmail.com', 'rrxmdyccusziyrlu')
    # smtp_obj.sendmail("test", "yakov.dergachev.yad@gmail.com", "123")
    return 'Иди отсюда, розбийник'


if __name__ == '__main__':
    app.run(debug=True)


@app.route('/download_recipe', methods=['POST'])
# Входные данные: id рецепта
# Возвращает данные о рецепте
def download_recipe():
    connection = engine.connect()
    recipe_id = request.form['recipe_id']

    # Берем сначала обложку рецепта
    recipe_cover = connection.execute('select * from recipes where id=%s', recipe_id).fetchone()
    id = recipe_cover[0]
    username = recipe_cover[1]
    picture = recipe_cover[2]
    recipe_name = recipe_cover[3]
    views = recipe_cover[5]
    score = str(recipe_cover[7])
    res = {'recipe_id': id, 'main_picture': picture, 'recipe_name': recipe_name,
           'score': score, 'views': views}
    # Теперь берем внутренности рецепта
    recipe_into = connection.execute('select * from about_recipe where recipes_id=%s', recipe_id).fetchone()
    res.update({
        'description': recipe_into[4],
        'ingredients': recipe_into[5],
        'steps_descriptions': recipe_into[6],
        'links_to_pictures': recipe_into[7],
        'timers': recipe_into[8]
    })

    connection.close()
    return jsonify(res)


@app.route('/email', methods=['POST'])
# Входные данные: почта пользователя
# Возвращает результат сброса пароля и код восстановления
def email():
    connection = engine.connect()
    email = request.form['email']

    is_exist = connection.execute("select id from users where email = %s", email).fetchone()
    if is_exist is None:
        return 'Такой почты нет'
    code = random.randint(1000, 9999)

    # Создайте объект MIMEMultipart для сообщения
    msg = MIMEMultipart()

    # Установите тему сообщения
    msg['Subject'] = str(code)
    msg['From'] = 'RPF - рецепты'

    # Добавьте текст сообщения
    message = str(code)
    msg.attach(MIMEText(message, 'plain'))

    # Создайте SMTP-соединение
    smtp_obj = smtplib.SMTP('smtp.gmail.com', 587)
    smtp_obj.starttls()

    # Войдите в аккаунт Gmail
    smtp_obj.login('yakov.dergachev.00@gmail.com', 'rrxmdyccusziyrlu')

    # Отправьте сообщение
    smtp_obj.sendmail("lol", email, msg.as_string())

    # Закройте соединение
    smtp_obj.quit()
    connection.close()
    return 'ok ' + str(code)


@app.route('/change_password', methods=['POST'])
# Входные данные: имя пользователя, хеш пароля
# Возвращает результат смены пароля
def change_password():
    connection = engine.connect()
    trans = connection.begin()
    username = request.form['username']
    hash = request.form['hash']
    connection.execute("update users set password = %s where username = %s", (hash, username))
    trans.commit()
    connection.close()
    return 'ok'


@app.route('/add_request', methods=['POST'])
# Входные данные: имя пользователя, имя запрашиваемого рецепта, метро, цена
# Возвращает результат добавления запроса
def add_request():
    connection = engine.connect()
    trans = connection.begin()
    username = request.form['username']
    request_name = request.form['request_name']
    metro = request.form['metro']
    price = request.form['price']
    sp = connection.execute("select id from requests_to_shief where request_name = %s", request_name).fetchone()
    if sp is not None:
        return 'this_request_already_exist'
    connection.execute("insert into requests_to_shief(username, request_name, metro, price) values(%s, %s,%s,%s)",
                       (username, request_name, metro, price))

    trans.commit()
    connection.close()
    return 'ok'


@app.route('/add_response', methods=['POST'])
# Входные данные: имя повара, id повара, название рецепта, описание, ингредиенты
# Возвращает результат добавления рецепта
def add_response():
    connection = engine.connect()
    trans = connection.begin()
    username = request.form['username']  # это имя повара
    id = request.form['id']
    sp_chief_responsed = set(
        connection.execute("select responsed from chiefs where chief_name = %s", username).fetchone()[
            0].split(','))
    sp_chief_responsed.add(id)
    sp_chief_responsed = ','.join(sp_chief_responsed)
    connection.execute("update chiefs set responsed = %s where chief_name = %s", (sp_chief_responsed, username))
    sp_chiefs_names = connection.execute('select chiefs from requests_to_shief where id = %s', id).fetchone()[0]
    if username not in sp_chiefs_names:
        sp_chiefs_names += ',' + username
    connection.execute('update requests_to_shief set chiefs = %s where id = %s', (sp_chiefs_names, id))

    trans.commit()
    connection.close()
    return 'ok'


@app.route('/update_recipe', methods=['POST'])
# Входные данные: id рецепта, имя рецепта, описание, ингредиенты, описания шагов, таймеры, названия ингредиентов
# Функция возвращает результат обновления рецепта
def update_recipe():
    connection = engine.connect()
    trans = connection.begin()
    id = request.form['id']
    old_main_picture = connection.execute("select picture from recipes where id = %s", id).fetchone()[0]
    old_main_picture = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + old_main_picture.split('/')[-1]

    def check_exist(path):
        check_exist_picture = connection.execute("select count_links_on_picture from images where image_path = %s",
                                                 path).fetchone()
        if check_exist_picture is None:
            connection.execute("insert into images(image_path, count_links_on_picture) values (%s, %s)",
                               (path, 1))
        else:
            connection.execute("update images set count_links_on_picture = %s where image_path = %s",
                               (check_exist_picture[0] + 1, path))

    def check(path):
        count_links = connection.execute("select count_links_on_picture from images where image_path = %s",
                                         path).fetchone()
        if count_links[0] == 1:
            os.remove(path)
            connection.execute("delete from images where image_path = %s", path)
        else:
            connection.execute("update images set count_links_on_picture = %s where image_path = %s",
                               (count_links[0] - 1, path))

    check(old_main_picture)

    old_pictures = connection.execute("select links_to_pictures from about_recipe where recipes_id = %s", id).fetchone()
    old_pictures = old_pictures[0].split()
    for link in old_pictures:
        path = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + link.split('/')[-1]
        check(path)
    sp_image_path = ''
    main_picture = request.files['picture0']
    main_picture_path = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + secure_filename(main_picture.filename)
    main_picture.save(main_picture_path)
    check_exist(main_picture_path)
    for i in range(1, len(request.files)):
        picture = request.files['picture' + str(i)]
        print(picture)
        image_path = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + secure_filename(picture.filename)
        sp_image_path += image_path + ' '
        picture.save(image_path)
        check_exist(image_path)
    recipe_name = request.form['recipe_name']
    description = request.form['description']
    ingredients = request.form['ingredients']
    steps_descriptions = request.form['steps_descriptions']
    timers = request.form['timers']
    names_ingredients = request.form['names_ingredients']
    connection.execute("update about_recipe set recipe_name = %s, description = %s, ingredients = %s, "
                       "steps_descriptions = %s, links_to_pictures = %s, timers = %s where recipes_id = %s",
                       (recipe_name, description,
                        ingredients, steps_descriptions,
                        sp_image_path, timers, id))
    connection.execute("update recipes set picture = %s, recipe_name = %s where id = %s",
                       (main_picture_path, recipe_name, id))

    sp = connection.execute("select id, recipes_id from ingredients where recipes_id like %s",
                            '%' + id + '%').fetchall()

    for answer in sp:
        string_id = answer[0]
        string_recipes_id = answer[1]
        string_recipes_id = string_recipes_id.replace(',' + id, '')
        if ',' not in string_recipes_id:
            connection.execute("delete from ingredients where id = %s", string_id)
        else:
            connection.execute("update ingredients set recipes_id = %s where id = %s", (string_recipes_id, string_id))

    request_main = []
    word = ''
    for i in names_ingredients:
        if i.isalpha():
            word += i
        else:
            if word != '':
                request_main.append(word)
            word = ''
    if word != '' and word[0].isalpha():
        request_main.append(word)
    for ingredient in request_main:
        is_exist = connection.execute("select recipes_id from ingredients where ingredient = %s", ingredient).fetchone()
        if is_exist is None:
            connection.execute("insert into ingredients(ingredient, recipes_id) values (%s, %s)", (ingredient, id))
        else:
            connection.execute("update ingredients set recipes_id = %s where ingredient = %s",
                               (is_exist[0] + ',' + str(id), ingredient))
    trans.commit()
    connection.close()

    return 'ok'


@app.route('/upload', methods=['POST'])
# Входные данные: id рецепта, имя рецепта, описание, ингредиенты, описания шагов, таймеры, названия ингредиентов
# Функция фозвращает результат добавления рецепта в базу данных
def upload():
    connection = engine.connect()
    trans = connection.begin()

    def check_exist(path):
        check_exist_picture = connection.execute("select count_links_on_picture from images where image_path = %s",
                                                 path).fetchone()
        if check_exist_picture is None:
            connection.execute("insert into images(image_path, count_links_on_picture) values (%s, %s)",
                               (path, 1))
        else:
            connection.execute("update images set count_links_on_picture = %s where image_path = %s",
                               (check_exist_picture[0] + 1, path))

    username = request.form['username']
    sp_image_path = ''
    main_picture = request.files['picture0']
    main_picture_path = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + secure_filename(main_picture.filename)
    main_picture.save(main_picture_path)
    check_exist(main_picture_path)
    for i in range(1, len(request.files)):
        picture = request.files['picture' + str(i)]
        print(picture)
        image_path = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + secure_filename(picture.filename)
        sp_image_path += image_path + ' '
        picture.save(image_path)
        check_exist(image_path)
    recipe_name = request.form['recipe_name']
    all_recipes_of_user = connection.execute("select recipe_name from recipes where username = %s", username).fetchall()
    all_recipes_of_user_eq = [str(i[0].encode('utf-8')).strip().decode('utf-8') for i in all_recipes_of_user]
    if recipe_name in all_recipes_of_user_eq:
        return 'no'
    description = request.form['description']
    ingredients = request.form['ingredients']
    steps_descriptions = request.form['steps_descriptions']
    timers = request.form['timers']
    names_ingredients = request.form['names_ingredients']
    connection.execute("insert into recipes(username, picture, recipe_name) values(%s,%s,%s)",
                       username, main_picture_path, recipe_name)
    result = connection.execute("SELECT LAST_INSERT_ID()")
    id = result.fetchone()[0]

    connection.execute(
        "insert into about_recipe(username, recipes_id, recipe_name, description, ingredients, steps_descriptions, "
        "links_to_pictures, timers) values(%s,%s,%s,%s,%s,%s,%s,%s)",
        username, id, recipe_name, description, ingredients, steps_descriptions, sp_image_path, timers)

    request_main = []
    word = ''
    for i in names_ingredients:
        print(i.isalpha())
        if i.isalpha():
            word += i
        else:
            if word != '':
                request_main.append(word)
            word = ''
    if word != '' and word[0].isalpha():
        request_main.append(word)
    for ingredient in request_main:
        is_exist = connection.execute("select recipes_id from ingredients where ingredient = %s", ingredient).fetchone()
        if is_exist is None:
            connection.execute("insert into ingredients(ingredient, recipes_id) values (%s, %s)", (ingredient, id))
        else:
            connection.execute("update ingredients set recipes_id = %s where ingredient = %s",
                               (is_exist[0] + ',' + str(id), ingredient))

    trans.commit()
    connection.close()

    return 'ok'


@app.route('/add_chief', methods=['POST'])
# Входные данные: имя повара, фотография повара, номер телефона, метро, кратко о поваре,
# обновляется информация о поваре или это новый повар
# Функция возвращает результат добавления или обновления информации о поваре
def add_chief():
    connection = engine.connect()
    trans = connection.begin()

    # username = ''
    # for i in request.form.get('username'):
    #     username += i
    # username = username.encode('utf-8').decode('utf-8')
    username = request.form['username']
    item = request.files['image']
    item_path = '/home/c/cshse/cshse.beget.tech/public_html/22/foto/' + secure_filename(item.filename)
    item.save(item_path)
    phone_number = request.form['phone_number']
    metro = request.form['metro']
    short_about_yourself = request.form['short_about_yourself']
    is_update = request.form['is_update']
    print("---------", is_update)
    if is_update == 'True':
        connection.execute(
            "update chiefs set image_path=%s, phone_number=%s, metro=%s, short_about_yourself=%s where chief_name=%s",
            item_path, phone_number, metro, short_about_yourself, username)
    else:
        connection.execute(
            "insert into chiefs(image_path, chief_name, phone_number, metro, short_about_yourself) "
            "values(%s,%s,%s,%s,%s)",
            item_path, username, phone_number, metro, short_about_yourself)

    trans.commit()
    connection.close()

    return 'ok'


@app.route('/about_recipe', methods=['POST'])
# Входные данные: id рецепта
# Функция овзвращает информацию о рецепте
def about_recipe():
    connection = engine.connect()
    recipes_id = request.form['recipes_id']
    # username = request.form['username']

    # Получаем текущее значение поля 'views'
    current_views_and_score = connection.execute("SELECT views,score FROM recipes WHERE id = %s", recipes_id).fetchone()
    current_views = current_views_and_score[0]
    current_score = current_views_and_score[1]
    # Обновляем значение поля 'views', добавляя 1 к текущему значению
    new_views = current_views + 1
    connection.execute("UPDATE recipes SET views = %s WHERE id = %s", (new_views, recipes_id))
    try:
        chief_name = connection.execute("select username from recipes where id = %s", recipes_id).fetchone()[0]
        main_views = connection.execute("select views from chiefs where chief_name = %s", chief_name).fetchone()[0]
        connection.execute("update chiefs set views = %s where chief_name = %s", (main_views + 1, chief_name))
    except:
        pass

    sp_about_recipe = connection.execute(
        "SELECT * FROM about_recipe where recipes_id=%s", recipes_id)
    recipe_data = sp_about_recipe.fetchone()
    recipe = {
        'recipe_id': recipes_id,
        'chief_name': recipe_data[2],
        'recipe_name': recipe_data[3],
        'description': recipe_data[4],
        'ingredients': recipe_data[5],
        'steps_descriptions': recipe_data[6],
        'links_to_pictures': recipe_data[7],
        'timers': recipe_data[8],
        'score': current_score,
        'views': new_views
    }
    connection.close()
    return jsonify(recipe)


@app.route('/delete_recipe', methods=['POST'])
# Входные данные: id рецепта
# Функция возвращает результат удаления рецепта из базы данных
def delete_recipe():
    connection = engine.connect()
    recipe_id = request.form['recipe_id']

    main_picture = connection.execute("select picture from recipes where id = %s", recipe_id).fetchone()[0]
    links_to_pictures = connection.execute("select links_to_pictures from about_recipe where recipes_id = %s",
                                           recipe_id).fetchone()[0].split()

    def check(path):
        count_links = connection.execute("select count_links_on_picture from images where image_path = %s",
                                         path).fetchone()
        if count_links[0] == 1:
            os.remove(path)
            connection.execute("delete from images where image_path = %s", path)
        else:
            connection.execute("update images set count_links_on_picture = %s where image_path = %s",
                               (count_links[0] - 1, path))

    check(main_picture)
    for link in links_to_pictures:
        check(link)
    sp = connection.execute("select id, recipes_id from ingredients where recipes_id like %s",
                            '%' + recipe_id + '%').fetchall()

    for answer in sp:
        string_id = answer[0]
        string_recipes_id = answer[1]
        string_recipes_id = string_recipes_id.replace(',' + recipe_id, '')
        if ',' not in string_recipes_id:
            connection.execute("delete from ingredients where id = %s", string_id)
        else:
            connection.execute("update ingredients set recipes_id = %s where id = %s", (string_recipes_id, string_id))
    connection.execute("delete from recipes where id = %s", recipe_id)
    connection.execute("delete from about_recipe where recipes_id = %s", recipe_id)
    connection.close()
    return 'ok'


@app.route('/my_responses', methods=['POST'])
# Входные данные: имя повара, id запроса
# Фунция возвращает список запросов повара
def my_responses():
    connection = engine.connect()
    username = request.form['username']
    id = request.form['id']

    sp_my_responses = connection.execute("select responsed from chiefs where chief_name = %s", username).fetchone()[
        0].split(',')
    print(sp_my_responses, id)
    if id != '-1':
        sp_my_responses.remove(id)
        sp_chiefs = connection.execute('select chiefs from requests_to_shief where id=%s', id).fetchone()[
            0]
        sp_chiefs = sp_chiefs.split(',')
        sp_chiefs.remove(username)
        sp_chiefs = ','.join(sp_chiefs)
        connection.execute('update requests_to_shief set chiefs = %s where id = %s', (sp_chiefs, id))

    results = connection.execute(text("select * from requests_to_shief where id in :ids"), ids=sp_my_responses)
    sp_my_responses = ','.join(sp_my_responses)
    connection.execute("update chiefs set responsed = %s where chief_name = %s", (sp_my_responses, username))

    res = []
    sp_ids = set()
    for recipe in results:
        id = recipe[0]
        request_name = recipe[2]
        metro = recipe[3]
        price = recipe[4]
        if id not in sp_ids:
            sp_ids.add(id)
            a = {'id': id, 'request_name': request_name, 'metro': metro, 'price': price}
            res.append(a)
    connection.close()
    return jsonify(res)


@app.route('/send_estimate', methods=['POST'])
# Входные данные: id рецепта, количество звезд, имя повара
# Функция возвращает результат добавления оценки рецепта
def send_estimate():
    connection = engine.connect()
    recipe_id = request.form['recipe_id']
    star_count = request.form['star_count']
    username = request.form['username']

    # Получаем текущее значение поля 'views'
    sp = connection.execute("SELECT sum_score,score_count FROM recipes WHERE id = %s", recipe_id).fetchone()
    old_sum_score = sp[0]
    old_count_score = sp[1]

    # Обновляем значение поля 'views', добавляя 1 к текущему значению
    new_score = (old_sum_score + float(star_count)) / (old_count_score + 1)
    connection.execute("UPDATE recipes SET sum_score = %s, score_count = %s, score = %s WHERE id = %s",
                       (old_sum_score + float(star_count), old_count_score + 1, new_score, recipe_id))
    connection.execute("update chiefs set score = %s where chief_name = %s", (new_score, username))
    connection.close()
    return 'ok'


@app.route('/chief_account', methods=['POST'])
# Входные данные: имя повара
# Функция озвращает всю информацию о поваре
def chief_account():
    connection = engine.connect()
    username = request.form['username']
    chief_data = connection.execute(
        "select * from chiefs where chief_name = %s", username).fetchone()
    connection.close()
    chief = {
        'foto_path': chief_data[1],
        'name': chief_data[2],
        'views': chief_data[3],
        'score': str(chief_data[4]),
        'phone_number': chief_data[5],
        'metro': chief_data[6],
        'short_about_chief': chief_data[7]
    }
    return jsonify(chief)


@app.route('/add_announcement', methods=['POST'])
# Входные данные: имя повара, добавление повара или загрузка информации о нем
# Функция возвращает либо всю информацию о поваре, либо возвращает результат добавления обьявления повара
def add_announcement():
    connection = engine.connect()
    username = request.form['username']
    is_add = request.form['is_add']
    print('---------------', is_add)
    if is_add == 'False':
        chief_data = connection.execute(
            "SELECT * FROM chiefs where chief_name = %s", username).fetchone()
        chief = {
            'foto_path': chief_data[1],
            'name': chief_data[2],
            'views': chief_data[3],
            'score': str(chief_data[4]),
            'phone_number': chief_data[5],
            'metro': chief_data[6],
            'short_about_chief': chief_data[7]
        }
        connection.close()
        return jsonify(chief)
    else:
        connection.execute(
            "UPDATE chiefs SET is_add = %s WHERE chief_name = %s",
            (1, username))
        connection.close()
        return 'add'


@app.route('/registration', methods=['POST'])
# Входные данные: имя, хеш пароля, почта
# Функция возвращает результат регистрации
def registration():
    username = request.form['username']
    hash = request.form['hash']
    email = request.form['email']
    if add_person(username, hash, email):
        return 'ok'
    else:
        return 'no'


@app.route('/check_chief', methods=['POST'])
# Входные данные: имя повара
# ФУнкция возвращает наличие повара в базе данных
def check_chief():
    username = request.form['username']
    connection = engine.connect()

    check = connection.execute("select id from chiefs where chief_name = %s", username).fetchone()

    connection.close()
    if check is None:
        return 'no'
    return 'ok'


@app.route('/get_hash', methods=['POST'])
# Входные данные: имя пользователя
# Функция возвращает хеш пароля
def get_hash():
    username = request.form['username']
    connection = engine.connect()
    result = connection.execute("select password from users where username=%s", username).fetchone()
    connection.close()
    print('-------------', result)
    if result is None:
        return 'no'
    else:
        return result[0]


@app.route('/enter', methods=['POST'])
# Входные данные: имя, хеш пароля
# Функция возвращает результат авторицации пользователя
def login():
    username = request.form['username']
    hash = request.form['hash']
    email = request.form['email']
    if get_password(username) == hash:
        return 'ok'
    else:
        return 'no'


# Входные данные: имя, пароля, почта
# Функция возвращает результат добавления пользователя
def add_person(username, password, email):
    connection = engine.connect()
    trans = connection.begin()
    all_users = connection.execute("select username from users where username=%s", username).fetchall()
    if len(all_users) != 0:
        return False
    connection.execute("insert into users(username,password,email) values (%s,%s,%s)", username, password, email)
    trans.commit()
    connection.close()
    return True


# Входные данные: имя
# Функуия возвращает результат попытки взятия пароля
def get_password(username):
    connection = engine.connect()
    hash = connection.execute("select password from users where username=%s", username).fetchone()
    connection.close()
    print('--------------------', hash)
    if hash is None:
        return 'no'
    else:
        return hash[0]


@app.route('/get_fridge', methods=['POST'])
# Входные данные: имя пользователя
# Функция возвращает ингредиенты в холодильнике, либо то, тчо в холодильнике нет ингредиентов
def get_fridge():
    username = request.form['username']
    connection = engine.connect()
    trans = connection.begin()

    is_exist = connection.execute("select ingredients from fridges where username = %s", username).fetchone()
    trans.commit()
    connection.close()
    if is_exist is None:
        return 'no_results'
    else:
        return is_exist[0]


@app.route('/save_fridge', methods=['POST'])
# Входные данные: имя пользователя, список ингредиентов
# Функция возвращает результат обновления информации об игредиентах в холодильнике
def save_fridge():
    username = request.form['username']
    ingredients = request.form['ingredients']
    connection = engine.connect()
    trans = connection.begin()

    is_exist = connection.execute("select ingredients from fridges where username = %s", username).fetchone()
    if is_exist is None:
        connection.execute("insert into fridges(username, ingredients) values (%s,%s)", (username, ingredients))
    else:
        connection.execute("update fridges set ingredients = %s where username = %s", (ingredients, username))
    trans.commit()
    connection.close()
    return 'ok'


@app.route('/main_url', methods=['POST'])
# Входные данные: целая часть рейтинга от, дробная часть рейтинга от, целая часть рейтинга до, дробная часть рейтинга до,
# нижняя граница просмотров, верхняя граница просмотров, могу ли я приготовить рецепт, сортируем ли рецепты по просмотрам,
# мои ли это рецепты, текстовый запрос, имя пользователя, индекс последнего рецепта, счетчик
# Функция возвращает рецепты, которые подходят под запрос
def main_url():
    rating_from_one = request.form['rating_from_one']
    rating_from_two = request.form['rating_from_two']
    rating_to_one = request.form['rating_to_one']
    rating_to_two = request.form['rating_to_two']
    views_from = request.form['views_from']
    views_to = request.form['views_to']
    can_i_make_recipe = request.form['can_i_make_recipe']
    sort_by_views = request.form['sort_by_views']
    my_recipes = request.form['my_recipes']
    search_data = request.form['search_data']
    username = request.form['username']
    index_last_recipe = request.form['index_last_recipe']
    count = request.form['count']
    print('----------------------------', count)
    count = str(int(count) * 25)
    print('----------------------------', count)
    request_main = []
    word = ''
    for i in search_data:
        if i.isalpha():
            word += i
        else:
            if word != '':
                request_main.append(word)
            word = ''
    if word != '' and word[0].isalpha():
        request_main.append(word)
    sp_names = tuple('%' + i.strip() + '%' for i in request_main)
    where_clause = " OR ".join(['recipe_name LIKE %s' for _ in range(len(sp_names))])
    print("--------+++--------", request_main)
    if rating_from_one == '': rating_from_one = '0'
    if rating_to_one == '': rating_to_one = '10'
    if views_from == '': views_from = 0
    if views_to == '': views_to = 99999999999

    rating_from = float(rating_from_one + '.' + rating_from_two)
    rating_to = float(rating_to_one + '.' + rating_to_two)

    connection = engine.connect()
    if rating_from == views_from and rating_to_one == '10' and views_to == 99999999999 and search_data == '' and \
            sort_by_views == '0' and can_i_make_recipe == '0' and my_recipes == '0':
        print('111111111111111')
        recipes_results = connection.execute("SELECT * FROM recipes WHERE id > %s LIMIT 25",
                                             int(index_last_recipe)).fetchall()
        if len(recipes_results) == 0:
            return 'no_results'

    elif search_data != '':
        print('222222222222222')

        query_ingredients = '''
                SELECT recipes_id
                FROM ingredients
                WHERE ingredient IN :ingredients
            '''
        result = connection.execute(text(query_ingredients), ingredients=request_main).fetchall()
        recipes_ids = []
        print('result', result)
        if len(result) != 0:
            # Получаем список recipe_id в нужном порядке
            d = {}
            for row in result:
                for i in row['recipes_id'].split(','):
                    num = int(i)
                    d[num] = d.get(num, 0) + 1
                    recipes_ids.append(num)
            # Сортируем по количеству вхождений
            d = sorted(d.items(), key=lambda x: x[1], reverse=True)
            recipes_ids = [i[0] for i in d]
        recipes_ids.append(-1)
        print('recipes_ids', recipes_ids)
        if can_i_make_recipe == '1':
            sp_ingredients = connection.execute("select ingredients from fridges where username = %s",
                                                username).fetchone()
            if sp_ingredients is None:
                return 'you_have_not_ingredients'
            ingredients = sp_ingredients[0].split('|&')

            query = '''select recipes_id from ingredients where ingredient in :ingredient'''
            indexes = connection.execute(text(query), ingredient=ingredients).fetchall()
            print(indexes)
            set_indexes = set(indexes[0][0].split(','))
            for string in indexes:
                print('set_index', set_indexes & set(string[0].split(',')))
                string = string[0].split(',')
                set_indexes &= set(string)
            print('set_index', set_indexes)
            if my_recipes == '1':
                query_names = "select * from recipes where (" + where_clause + ") and id in %s and username = %s and " \
                                                                               "score between %s and %s"
                parameters = sp_names + (list(set_indexes),) + (username,) + (rating_from, rating_to)
            else:
                query_names = "select * from recipes where (" + where_clause + ") and id in %s and score " \
                                                                               "between %s and %s"
                parameters = sp_names + (list(set_indexes),) + (rating_from, rating_to)
            # query_names += ' and id > %s'
            # parameters += (int(index_last_recipe),)
            if sort_by_views == '1':
                query_names += ' order by views desc'
            else:
                query_names += ' and views between %s and %s'
                parameters += (views_from, views_to)
            query_names += ' LIMIT 25 OFFSET ' + count
            recipes_results = connection.execute(query_names, parameters).fetchall()

        else:
            if my_recipes == '1':
                query_ingredients = "select * from recipes where id IN :ids and username = :username and " \
                                    "score between :rf and :rt"
                query_names = "select * from recipes where (" + where_clause + ") and username = %s and " \
                                                                               "score between %s and %s"
                parameters_ingredients = {'ids': recipes_ids, 'username': username, 'rf': rating_from, 'rt': rating_to}
                parameters_names = sp_names + (username,) + (rating_from, rating_to)
            else:
                query_ingredients = "select * from recipes where id in :ids and score between :rf and :rt"
                query_names = "select * from recipes where (" + where_clause + ") and score between %s and %s"
                parameters_ingredients = {'ids': recipes_ids, 'rf': rating_from, 'rt': rating_to}
                parameters_names = sp_names + (rating_from, rating_to)

            # query_ingredients += ' and id > :id'
            # parameters_ingredients.update({'id': int(index_last_recipe)})
            # query_names += ' and id > %s'
            # parameters_names += (int(index_last_recipe),)
            if sort_by_views == '1':
                query_ingredients += ' order by views desc'
                query_names += ' order by views desc'
            else:
                query_ingredients += ' and views between :vf and :vt'
                query_names += ' and views between %s and %s'
                parameters_ingredients.update({'vf': views_from, 'vt': views_to})
                parameters_names += (views_from, views_to)
            query_ingredients += ' LIMIT 25 OFFSET ' + count
            query_names += ' LIMIT 25 OFFSET ' + count
            print('----------', query_names, parameters_names)
            recipes_results = connection.execute(text(query_ingredients), **parameters_ingredients).fetchall()
            recipes_results += connection.execute(query_names, parameters_names).fetchall()

        if len(recipes_results) == 0:
            return "no_results"

        # if sort_by_views == "1":
        #     query_ingredients_sort = '''
        #             SELECT * FROM recipes
        #             WHERE id IN :recipes_ids AND score BETWEEN :rf AND :rt ORDER BY views DESC
        #         '''
        #     recipes_results = connection.execute(
        #         text(query_ingredients_sort),
        #         recipes_ids=recipes_ids, rf=rating_from, rt=rating_to).fetchall()
        #     query_names_sort = "SELECT * FROM recipes WHERE (" + where_clause + ") AND score BETWEEN %s AND %s ORDER BY views DESC"
        #     recipes_results += connection.execute(query_names_sort, sp_names + (rating_from, rating_to)).fetchall()
        # else:
        #     query_ingredients = '''
        #             SELECT * FROM recipes
        #             WHERE id IN :recipes_ids AND score BETWEEN :rf AND :rt AND views BETWEEN :vf AND :vt
        #         '''
        #     recipes_results = connection.execute(text(query_ingredients), recipes_ids=recipes_ids, rf=rating_from,
        #                                          rt=rating_to,
        #                                          vf=views_from,
        #                                          vt=views_to).fetchall()
        #     query_names = "SELECT * FROM recipes WHERE (" + where_clause + ") AND score BETWEEN %s AND %s AND views BETWEEN %s AND %s"
        #     print('+++++++++++====================', rating_from, rating_to, views_from, views_to)
        #     print(query_names, '----------', sp_names + (rating_from, rating_to, views_from, views_to))
        #     recipes_results += connection.execute(query_names,
        #                                           sp_names + (rating_from, rating_to, views_from, views_to)).fetchall()
        # print('recipes_results', recipes_results, recipes_ids, where_clause, sp_names)
        # if len(recipes_results) == 0:
        #     return 'no_results'

    else:
        print('333333333333333')

        if can_i_make_recipe == '1':
            sp_ingredients = connection.execute("select ingredients from fridges where username = %s",
                                                username).fetchone()
            if sp_ingredients is None:
                return 'you_have_not_ingredients'
            sp_ingredients = sp_ingredients[0].split('|&')
            ingredients = []
            for string in sp_ingredients:
                name, quantity, units = string.split('&|')
                ingredients.append(name)

            query = '''select recipes_id from ingredients where ingredient in :ingredient'''
            indexes = connection.execute(text(query), ingredient=ingredients).fetchall()
            print(indexes)
            set_indexes = set(indexes[0][0].split(','))
            for string in indexes:
                print('set_index', set_indexes & set(string[0].split(',')))
                string = string[0].split(',')
                set_indexes &= set(string)
            print('set_index', set_indexes)
            if my_recipes == '1':
                query = "select * from recipes where id in :ids and username = :username and score between :rf and :rt"
                parameters = {'ids': list(set_indexes), 'username': username, 'rf': rating_from, 'rt': rating_to}
            else:
                query = "select * from recipes where id in :ids and score between :rf and :rt"
                parameters = {'ids': list(set_indexes), 'rf': rating_from, 'rt': rating_to}

            # query += ' and id > :id'
            # parameters.update({'id': int(index_last_recipe)})
            if sort_by_views == '1':
                query += ' order by views desc'
            else:
                query += ' and views between :vf and :vt'
                parameters.update({'vf': views_from, 'vt': views_to})
            query += ' LIMIT 25 OFFSET ' + count
            print(query, text(query), parameters)
            recipes_results = connection.execute(text(query), **parameters).fetchall()

        else:
            if my_recipes == '1':
                query = "select * from recipes where username = :username and score between :rf and :rt"
                parameters = {'username': username, 'rf': rating_from, 'rt': rating_to}
            else:
                query = "select * from recipes where score between :rf and :rt"
                parameters = {'rf': rating_from, 'rt': rating_to}

            # query += ' and id > :id'
            # parameters.update({'id': int(index_last_recipe)})
            if sort_by_views == '1':
                query += ' order by views desc'
            else:
                query += ' and views between :vf and :vt'
                parameters.update({'vf': views_from, 'vt': views_to})
            query += ' LIMIT 25 OFFSET ' + count
            recipes_results = connection.execute(text(query), **parameters).fetchall()
        if len(recipes_results) == 0:
            return 'no_results'

        # if sort_by_views == "1":
        #     recipes_results = connection.execute(
        #         "SELECT * FROM recipes WHERE score BETWEEN %s AND %s ORDER BY views DESC", (rating_from,
        #                                                                                     rating_to)).fetchall()
        # else:
        #     recipes_results = connection.execute(
        #         "SELECT * FROM recipes WHERE score BETWEEN %s AND %s AND views BETWEEN %s AND %s",
        #         (rating_from, rating_to, views_from, views_to)).fetchall()
        # if len(recipes_results) == 0:
        #     return 'no_results'
    res = []
    set_sp_for_id = set()
    for recipe in recipes_results:
        id = recipe[0]
        username = recipe[1]
        picture = recipe[2]
        recipe_name = recipe[3]
        views = recipe[5]
        score = str(recipe[7])
        if id not in set_sp_for_id:
            set_sp_for_id.add(id)
            a = {'id': id, 'username': username, 'picture': picture, 'recipe_name': recipe_name,
                 'score': score, 'views': views}
            res.append(a)
    connection.close()
    print('-----------', res)
    return jsonify(res)


@app.route('/find_chief', methods=['POST'])
# Входные данные: целая часть рейтинга от, дробная часть рейтинга от, целая часть рейтинга до, дробная часть рейтинга до,
# нижняя граница просмотров, верхняя граница просмотров, сам текстовый запрос, метро
# Функция возвращает поваров, которые подходят под запрос
def find_chief():
    rating_from_one = request.form['rating_from_one']
    rating_from_two = request.form['rating_from_two']
    rating_to_one = request.form['rating_to_one']
    rating_to_two = request.form['rating_to_two']
    views_from = request.form['views_from']
    views_to = request.form['views_to']
    search_data = request.form['search_data']
    metro = request.form['metro']
    if rating_from_one == '': rating_from_one = '0'
    if rating_to_one == '': rating_to_one = '10'
    if views_from == '': views_from = 0
    if views_to == '': views_to = 99999999999

    rating_from = float(rating_from_one + '.' + rating_from_two)
    rating_to = float(rating_to_one + '.' + rating_to_two)

    connection = engine.connect()
    if rating_from == views_from and rating_to_one == '10' and views_to == 99999999999 and search_data == '' \
            and metro == '':
        print('111111111111111')
        recipes_results = connection.execute("SELECT * FROM chiefs where is_add = %s LIMIT 100", 1).fetchall()

    elif search_data != '':
        print('222222222222222')
        if metro == '':
            query = '''select * from chiefs 
            where chief_name = :cn and score between :sf and :st and views between :vf and :vt and is_add = :is_add'''
            recipes_results = connection.execute(text(query),
                                                 cn=search_data, sf=rating_from, st=rating_to, vf=views_from,
                                                 vt=views_to, is_add=1).fetchall()
        else:
            query = '''select * from chiefs 
            where chief_name = :cn and metro = :metro and score between :sf and :st and views between :vf and :vt and 
            is_add = :is_add'''
            recipes_results = connection.execute(text(query), cn=search_data, metro=metro, sf=rating_from, st=rating_to,
                                                 vf=views_from,
                                                 vt=views_to, is_add=1).fetchall()
        if len(recipes_results) == 0:
            return 'no_results'

    else:
        print('333333333333333')
        if metro == '':
            recipes_results = connection.execute(
                "SELECT * FROM chiefs WHERE score BETWEEN %s AND %s and views between %s and %s and is_add = %s",
                (rating_from,
                 rating_to,
                 views_from,
                 views_to, 1)).fetchall()
        else:
            recipes_results = connection.execute(
                "SELECT * FROM chiefs WHERE metro = %s and score BETWEEN %s AND %s AND views BETWEEN %s AND %s and "
                "is_add = %s",
                (metro, rating_from, rating_to, views_from, views_to, 1)).fetchall()
        if len(recipes_results) == 0:
            return 'no_results'
    res = []
    set_sp_for_id = set()
    for recipe in recipes_results:
        id = recipe[0]
        image_path = recipe[1]
        chief_name = recipe[2]
        views = recipe[3]
        score = str(recipe[4])

        if id not in set_sp_for_id:
            set_sp_for_id.add(id)
            a = {'image_path': image_path, 'chief_name': chief_name, 'views': views, 'score': score}
            res.append(a)
    connection.close()
    return jsonify(res)


@app.route('/get_responses', methods=['POST'])
def get_responses():
    connection = engine.connect()
    username = request.form['username']
    sp_chiefs = connection.execute('select chiefs from requests_to_shief where username = %s', username).fetchone()
    if sp_chiefs is None:
        return 'no'
    sp_chiefs = sp_chiefs[0]
    sp_chiefs = sp_chiefs.split(',')
    result = connection.execute(text('select * from chiefs where chief_name in :sp'), sp=sp_chiefs).fetchall()
    res = []
    set_sp_for_id = set()
    for recipe in result:
        id = recipe[0]
        image_path = recipe[1]
        chief_name = recipe[2]
        views = recipe[3]
        score = str(recipe[4])

        if id not in set_sp_for_id:
            set_sp_for_id.add(id)
            a = {'image_path': image_path, 'chief_name': chief_name, 'views': views, 'score': score}
            res.append(a)
    connection.close()
    return jsonify(res)


@app.route('/load_all_requests', methods=['POST'])
# Входные данные: сам текстовый запрос, цена от, цена до, метро
# Функция возвращает запросы к повару кототрые подходят под запрос
def load_all_requests():
    connection = engine.connect()
    search_data = request.form['search_data']
    price_from = request.form['price_from']
    price_to = request.form['price_to']
    metro = request.form['metro']
    request_main = []
    word = ''
    for i in search_data:
        if i.isalpha():
            word += i
        else:
            if word != '':
                request_main.append(word)
            word = ''
    if word != '' and word[0].isalpha():
        request_main.append(word)
    if price_from == '': price_from = 0
    if price_to == '': price_to = 9999999999
    sp = tuple('%' + i.strip() + '%' for i in request_main)
    where_clause = " OR ".join(['request_name LIKE %s' for _ in range(len(sp))])
    print('++++++++++++++++++++++++++++', sp, where_clause, request_main)
    if search_data == '' and metro == '' and price_from == 0 and price_to == 9999999999:
        recipes_results = connection.execute("select * from requests_to_shief limit 100").fetchall()
    elif search_data != '':
        if metro == '':
            query = "SELECT * FROM requests_to_shief WHERE (" + where_clause + ") AND price BETWEEN %s AND %s"
            recipes_results = connection.execute(query, sp + (price_from, price_to)).fetchall()
        else:
            query = "SELECT * FROM requests_to_shief WHERE (" + where_clause + ") AND metro = %s AND " \
                                                                               "price BETWEEN %s AND %s"
            recipes_results = connection.execute(query, sp + (metro, price_from, price_to)).fetchall()
    else:
        if metro == '':
            query = "SELECT * FROM requests_to_shief WHERE price BETWEEN :price_from AND :price_to"
            recipes_results = connection.execute(text(query), price_from=price_from, price_to=price_to).fetchall()
        else:
            query = "SELECT * FROM requests_to_shief WHERE metro = :metro AND price BETWEEN :price_from AND :price_to"
            recipes_results = connection.execute(text(query), metro=metro, price_from=price_from,
                                                 price_to=price_to).fetchall()

    if len(recipes_results) == 0:
        return 'no_results'
    res = []
    sp_ids = set()
    for recipe in recipes_results:
        id = recipe[0]
        request_name = recipe[2]
        metro = recipe[3]
        price = recipe[4]
        if id not in sp_ids:
            sp_ids.add(id)
            a = {'id': id, 'request_name': request_name, 'metro': metro, 'price': price}
            res.append(a)
    connection.close()
    return jsonify(res)


if __name__ == "__main__":
    app.run()
