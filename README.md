# Postable - RESTful API para Gestión de Posts

Bienvenido a Postable, una API RESTful diseñada para gestionar publicaciones (posts) en una red social. Esta API debe ser capaz de manejar diferentes operaciones dependiendo de si el usuario está registrado o no.

## Tecnologías y Herramientas

- **Lenguaje:** C#.
- **Framework:** .Net Entity Framework Core
- **Autenticación/Autorización:** JWT.
- **Base de Datos:** MS SQL Server.

## Guía de Configuración y Levantamiento de la Aplicación C# .NET

### Requisitos Previos

Antes de comenzar, asegúrate de tener instalados los siguientes elementos:

- **IDE:** Preferiblemente Visual Studio o Visual Studio Code.
- **SDK de .NET:** Instalado y configurado en tu sistema.
- **Base de Datos:** MS SQL Server u otro compatible con Entity Framework Core si la aplicación utiliza una base de datos.

### Pasos para Levantar la Aplicación

1. **Clonar el Repositorio:**

   Clona el repositorio desde el repositorio remoto.

   ```json
   git clone https://github.com/codeableorg/postable-evaluation-EmersonHoruss
   cd postable-evaluation-EmersonHoruss
   ```

2. **Configuración de la Base de Datos:**

   - Crea el archivo appsettings.Development.json y asegúrate de que la cadena de conexión (DefaultConnection) esté configurada correctamente para tu instancia de SQL Server. Además debes configurar el Jwst. Ejemplo:

     ```json
     {
       "Jwt": {
         "Key": "clave-secreta-de-al-menos-32-caracteres",
         "Issuer": "your_issuer_here",
         "Audience": "your_audience_here"
       },
       "ConnectionStrings": {
         "DefaultConnection": "Server=HOLA_MUNDO;Database=Postable;Integrated Security=True;TrustServerCertificate=True;"
       },
       "Logging": {
         "LogLevel": {
           "Default": "Information",
           "Microsoft.AspNetCore": "Warning"
         }
       },
       "AllowedHosts": "*"
     }
     ```

   - Ejecuta las migraciones para crear la estructura inicial de la base de datos:
     ```json
     dotnet ef database update
     ```
     Esto aplicará todas las migraciones pendientes y creará las tablas necesarias en tu base de datos.

3. **Compilación de Aplicación**

   Compila la aplicación para asegurarte de que no hay errores y para generar el ejecutable.

   ```json
   dotnet build
   ```

4. **Ejecutar la Aplicación**

   Una vez configurado y compilado, puedes levantar tu aplicación API Postable:

   ```json
   dotnet run
   ```

   Esto iniciará la aplicación en modo de desarrollo y estará disponible en http://localhost:5202 por defecto.

5. **Acceder a Swagger UI**

   Una vez que la aplicación esté ejecutándose, puedes acceder a Swagger UI para explorar y probar los endpoints de tu API. Abre tu navegador web y visita:

   ```json
     http://localhost:5202/swagger
   ```

   Esto abrirá la interfaz de Swagger donde puedes ver todos los endpoints disponibles y probar sus operaciones.

6. **Usar Postman**

   Existe una forma alternativa de probar los endpoints, es usando Postman. En la raíz de la aplicación existe un archivo llamado `Postable.postman_collection.json`. Abres postman e importas este archivo, y estás listo para poder probarlo.

## Esquema de Base de Datos

1. **Users**

- `Id`: Identificador único del usuario. _Restricción_: Clave primaria.
- `Username`: Apodo de usuario. _Restricción_: Único, no nulo.
- `Password`: Contraseña del usuario, almacenada de manera segura. _Restricción_: No nulo.
- `Email`: Email del usuario. _Restricción_: Único, puede ser nulo.
- `FirstName`: Nombre del usuario. _Restricción_: Puede ser nulo.
- `LastName`: Apellido del usuario. _Restricción_: Puede ser nulo.
- `Role`: Rol del usuario, con valores 'user' o 'admin'. _Restricción_: No nulo, "user" por defecto.
- `CreatedAt`: Fecha y hora de creación del usuario. _Restricción_: No nulo.

2. **Posts**

- `Id`: Identificador único del post. _Restricción_: Clave primaria.
- `UserId`: Identificador del usuario que creó el post. _Restricción_: Clave foránea, no nulo.
- `Content`: Contenido del post. _Restricción_: No nulo.
- `CreatedAt`: Fecha y hora de creación del post. _Restricción_: No nulo.

3. **Likes**

- `Id`: Identificador único del like. _Restricción_: Clave primaria.
- `PostId`: Identificador del post al que se le dio like. _Restricción_: Clave foránea, no nulo.
- `UserId`: Identificador del usuario que dio like. _Restricción_: Clave foránea, no nulo.
- `CreatedAt`: Fecha y hora en que se dio el like. _Restricción_: No nulo.

### Restricciones y Relaciones Adicionales

- **Unicidad en Likes:** La combinación de `PostId` y `UserId` en la tabla `Likes` debe ser única para evitar likes duplicados.
- **Restricciones de Datos:** Deberás aplicar restricciones adecuadas en cuanto a longitud y formato de los datos según tu criterio (por ejemplo, definir una longitud máxima de `Username` o validar el formato de `Email`).

## Especificación de API

### Visualización de Posts

- **GET `/posts` (Ver Todos los Posts con Paginación y Filtros)**
  - **Descripción:** Retorna una lista de posts disponibles en la plataforma, con opciones de filtrado por usuario y ordenación.
  - **Parámetros Query:**
    - `username`: Filtrar posts por nombre de usuario (opcional).
    - `orderBy`: Criterio de ordenación, opciones: `createdAt`, `likesCount` (opcional, por defecto `createdAt`).
    - `order`: Dirección de la ordenación, opciones: `asc`, `desc` (opcional, por defecto `asc`).
  - **Respuesta:**
    - **200 OK:** Lista paginada de posts en formato JSON.
  - **Ejemplo de Respuesta:**
    ```json
    [
      {
        "id": 1,
        "content": "Este es un post",
        "createdAt": "2024-01-19 07:37:16-08",
        "username": "usuario1",
        "likesCount": 5
      },
      ...
    ]
    ```
  - **Ejemplo de Uso:**
    - Para obtener la lista de posts filtrando por el usuario 'usuarioEjemplo', ordenados por número de likes en orden descendente:
      - `GET /posts?username=usuarioEjemplo&orderBy=likesCount&order=desc`

### Interacción de Usuarios Registrados

- **POST `/posts` (Crear Nuevo Post)**

  - **Descripción:** Permite a un usuario registrado crear un nuevo post.
  - **Body:**
    - `content`: Texto del post.
  - **Respuesta:**
    - **201 Created:** Post creado exitosamente.
    - **400 Bad Request:** Si falta información o el formato es incorrecto.
    - **401 Unauthorized:** Si el usuario no está autenticado.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "id": 10,
      "content": "Mi post actualizado",
      "createdAt": "2024-01-19 10:37:16-08",
      "username": "mi-usuario",
      "likesCount": 0
    }
    ```

- **PATCH `/posts/:id` (Editar Post Existente)**

  - **Descripción:** Permite a un usuario registrado editar un post existente.
  - **Parámetros URL:**
    - `id`: Id del post a editar.
  - **Body:**
    - `content`: Texto actualizado del post. (El campo es opcional, pero se debe enviar al menos un campo para actualizar)
  - **Respuesta:**
    - **200 OK:** Post actualizado exitosamente. Devuelve el post actualizado.
    - **400 Bad Request:** Si falta información, el formato es incorrecto o no se envía ningún campo para actualizar.
    - **401 Unauthorized:** Si el usuario no está autenticado o no es el propietario del post.
    - **404 Not Found:** Si el post no existe.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "id": 10,
      "content": "Mi post actualizado",
      "createdAt": "2024-01-19 10:37:16-08",
      "username": "mi-usuario",
      "likesCount": 0
    }
    ```

- **POST `/posts/:postId/like` (Dar Like a un Post)**

  - **Descripción:** Permite a un usuario registrado dar "Like" a un post.
  - **Parámetros deURL:**
    - `postId`: Id del post a dar like.
  - **Respuesta:**
    - **200 OK:** Like registrado.
    - **404 Not Found:** Si el post no existe.
    - **401 Unauthorized:** Si el usuario no está autenticado.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "id": 15,
      "content": "Mi nuevo post",
      "createdAt": "2024-01-19 10:37:16-08",
      "username": "usuario",
      "likesCount": 1
    }
    ```

- **DELETE `/posts/:postId/like` (Eliminar Like de un Post)**
  - **Descripción:** Permite a un usuario eliminar su "like" de un post.
  - **Parámetros de URL:**
    - `postId`: ID del post a remover like.
  - **Respuesta:**
    - **200 OK:** Like eliminado.
    - **404 Not Found:** Si el post no existe o no tenía like previamente.
    - **401 Unauthorized:** Si el usuario no está autenticado.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "id": 15,
      "content": "Mi nuevo post",
      "createdAt": "2024-01-19 10:37:16-08",
      "username": "usuario",
      "likesCount": 0
    }
    ```

#### Gestión de Perfil de Usuario

- **GET `/me` (Ver Perfil de Usuario)**

  - **Descripción:** Muestra el perfil del usuario autenticado.
  - **Respuesta:**
    - **200 OK:** Información del perfil en formato JSON.
    - **401 Unauthorized:** Si el usuario no está autenticado.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "id": 2,
      "username": "miUsuario",
      "email": "miemail@example.com",
      "firstName": "Nombre",
      "lastName": "Apellido",
      "createdAt": "2024-01-19 10:37:16-08"
    }
    ```

- **PATCH `/me` (Editar Cuenta de Usuario)**

  - **Descripción:** Permite al usuario editar su información de perfil.
  - **Body:**
    - `email`, `firstName`, `lastName`: Campos opcionales para actualizar.
  - **Respuesta:**
    - **200 OK:** Perfil actualizado.
    - **400 Bad Request:** Si el formato es incorrecto.
    - **401 Unauthorized:** Si el usuario no está autenticado.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "id": 2,
      "username": "miUsuario",
      "email": "nuevo@mail.com",
      "firstName": "Nombre",
      "lastName": "Apellido",
      "createdAt": "2024-01-19 10:37:16-08"
    }
    ```

- **DELETE `/me` (Eliminar Cuenta de Usuario)**
  - **Descripción:** Permite al usuario eliminar su cuenta.
  - **Respuesta:**
    - **204 No Content:** Cuenta eliminada exitosamente.
    - **401 Unauthorized:** Si el usuario no está autenticado.

#### Registro y Autenticación de Usuarios

- **POST `/signup` (Crear Cuenta)**
- **Descripción:** Permite a un nuevo usuario registrarse en la plataforma.
- **Body:**
  - `username`, `password`: Campos requeridos para el registro.
- **Respuesta:**
  - **201 Created:** Cuenta creada.
  - **400 Bad Request:** Si falta información o el formato es incorrecto.
- **Ejemplo de Respuesta:**

  ```json
  {
    "id": 20,
    "username": "nuevoUsuario",
    "email": "un-mail@example.com",
    "firstName": "Nombre",
    "lastName": "Apellido",
    "createdAt": "2024-01-19 10:37:16-08"
  }
  ```

````

- **POST `/login` (Iniciar Sesión)**
  - **Descripción:** Permite a un usuario existente iniciar sesión.
  - **Body:**
    - `username`, `password`: Credenciales requeridas para el inicio de sesión.
  - **Respuesta:**
    - **200 OK:** Sesión iniciada, retorna token JWT.
    - **401 Unauthorized:** Credenciales incorrectas.
  - **Ejemplo de Respuesta:**
    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5..."
    }
    ```
````
