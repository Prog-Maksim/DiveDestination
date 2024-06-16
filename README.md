# Backend для сайта с турами.

### Используемый стек: ASP.NET Core, Entity Framework Core

__Данный проект еще находится в разработке__

## Структура проекта
- `Scripts` - скрипты проверки данных
- `Models` - папка с моделями для БД
- `Controllers` - папка с endpoint 
- `ConfigData` - папка с ключами для выпуска jwt токена
- `ProfileImage` - папка с изображения для профиля

* `Program.cs` - настройка бекенда, сборка endpoint, запуск бекенда

## Демонстрация работы

### Регистрация пользователя
Чтобы зарегистрироваться в DiveDestination и получить доступ к турам, нужно выполнить запрос по адресу:
- `(POST) /api/registration/user` - и передать следующие параметры: \`__firstName__\`, \`__lastName__\`, \`__login__\`, \`__password__\`

__Пример с curl__

- ```shell
  curl -X 'POST' \
  '<ip адрес>/api/registration/user' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'firstName=<имя>' \
  -F 'lastName=<фамилия>' \
  -F 'login=<почта>' \
  -F 'password=<пароль>'
  ```

Если регистрация прошла успешно, в ответ будет выслан JWT access токен.

### Авторизация пользователя
Чтобы авторизоваться в DiveDestination и получить доступ к турам нужно выполнить запрос по адресу:
- `(POST) /api/authorization/user` - передать следующие параметры: \`__login__\`, \`__password__\`

__Пример с curl__ 

- ```shell
    curl -X 'POST' \
    '<ip адрес>/api/authorization/user' \
    -H 'accept: */*' \
    -H 'Content-Type: multipart/form-data' \
    -F 'login=<почта>' \
    -F 'password=<пароль>'
    ```

Если авторизация прошла успешно, в ответ будет выслан JWT access токен.

### Refresh JWT access token
Когда у выпущенного при авторизации/регистрации JWT токена закончится время жизни (а именно 5 минут) потребуется выпустить новый для этого нужно выполнить запрос по адресу:
- `(POST) /api/authorization/refresh-token` - передать следующий параметр \`__access_token__\`

__Пример с curl__

- ```shell
    curl -X 'POST' \
  '<ip адрес>/api/authorization/refresh-token' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'access_token=<выпущенный ранее JWT токен>'
    ```
Если обновление токена прошло успешно, в ответ будет выслан новый JWT access токен

__Для более подробного изучения бекенда и endpoint откройте его по адресу <ip адрес>/swagger__

## Что необходимо еще реализовать
1. [x] выпуск jwt токена
2. [x] обновление jwt токена
3. [x] добавить swagger
3. [ ] карточки с турами
4. [ ] покупку туров
5. [ ] админ панель
6. [ ] редактирование профиля