# Proyecto de Sublimación

Este repositorio contiene una aplicación web desarrollada con: **React**, **React Router** y **Tailwind CSS** en el frontend, y una API en **.NET Core** con arquitectura hexagonal en el backend. La persistencia de datos es una base de datos PostgreSQL.

El backend está desplegado en **Railway**, mientras que el frontend está alojado en **Vercel**.

---

## 🚀 Tecnologías utilizadas

### Frontend:
- React con React Router
- Tailwind CSS

### Backend:
- .NET Core con arquitectura hexagonal
- Entity Framework Core (EF Core) para la base de datos
- PostgreSQL

### Despliegue:
- **Backend**: Railway
- **Base de Datos**: Railway (PostgreSQL)
- **Frontend**: Vercel

- ## ⚙️ Configuración local

### 1️⃣ Clonar el repositorio
```sh
 git clone https://github.com/brayanpdraza/book-reviews-app.git
 cd {tu-repositorio}
```

### 2️⃣ Configurar el Backend (.NET Core)
1. Ir a la carpeta del backend:
   ```sh
   cd backend
   ```
2. Crear un archivo `.env` o usar variables de entorno para la configuración de la base de datos y JWT.

## Variables de Entorno

Antes de ejecutar la aplicación, asegúrate de configurar las siguientes variables de entorno:

### Backend (ASP.NET Core)
| Variable            | Descripción                                        |
|---------------------|----------------------------------------------------|
| `Jwt_SecretKey`   | Clave secreta para firmar los tokens JWT.          | 
| `Jwt_Issuer`   | URL del emisor del token.          | 
| `Jwt_Audience`   | URL del receptor del token.          | 
| `Jwt_AccessTokenExpiration`   | Tiempo de vida del token de acceso.          | 
| `Jwt_RefreshTokenExpiration`   | Tiempo de vida del token de refresco.          | 
| `DATABASE_URL`   | URL de conexión de la base de datos.          | 

###

3. Ejecutar las migraciones para la base de datos:
   ```sh
   dotnet ef database update
   ```
4. Compilar y ejecutar el backend:
   ```sh
   dotnet run
   ```
   El servidor se ejecutará en `http://localhost:5000` o `http://localhost:5001`.

### 3️⃣ Configurar el Frontend (React)
1. Ir a la carpeta del frontend:
   ```sh
   cd frontend
   ```
2. Instalar las dependencias:
   ```sh
   npm install
   ```
3. Ejecutar la aplicación:
   ```sh
   npm run dev
   ```
   El frontend estará disponible en `http://localhost:3000`.

---

## 🚀 Despliegue

### 🔹 Desplegar el Backend en Railway
Se debe subir a un repositorio tanto el backend como el frontend. Tal como en el proyecto o en diferentes ramas, a preferencia del usuario.
1. Accede a [Railway](https://railway.app/).
2. Crea un nuevo proyecto y elige **Deploy from GitHub**.
3. Configura las variables de entorno del .env en Railway:
4. Ejecuta el despliegue y obtén la URL del backend.

### 🔹 Desplegar la Base de Datos en Railway
1. En Railway, añade un nuevo servicio **PostgreSQL** en el mismo proyecto del **backend**.
2. Copia la URL de conexión y configúrala como se explica en la conexión: 

DATABASE_URL : ${{ Postgres.DATABASE_URL }}.

### 🔹 Desplegar el Frontend en Vercel
1. Accede a [Vercel](https://vercel.com/).
2. Crea un nuevo proyecto y conéctalo a tu repositorio de GitHub.
3. Despliega la aplicación.

---

## 🚧 Problemas conocidos
- La API en Railway podría requerir CORS configurado adecuadamente, y se debe tener cuidado con las mayúsculas y mínusculas en las variables de entorno, pues es case sensitive.
- El frontend en Vercel debe apuntar a la URL correcta del backend en Railway.

Si encuentras algún error, revisa los logs de Railway y Vercel o abre un issue en GitHub.

---

## 📜 Licencia
Este proyecto está bajo la licencia **MIT**. ¡Siéntete libre de usarlo y mejorarlo! 🚀
