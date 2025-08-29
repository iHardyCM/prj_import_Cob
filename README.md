# ?? Proyecto Importaci¨®n Cobranza

Este es un proyecto **fullstack** para la gesti¨®n de cargas y exportaci¨®n de campa?as de cobranza.  
Incluye un **backend en ASP.NET Core** y un **frontend en React + Vite**.

---

## ?? Descripci¨®n

El sistema permite:

- ?? **Importar** datos de campa?as de cobranza desde distintos canales:
  - SMS
  - IVR
  - Email
  - Bot
  - WAPI
- ?? **Visualizar** las cargas realizadas, con informaci¨®n de cartera, lote, archivo, n¨²mero de filas y rango de fechas.
- ?? **Exportar** los datos de cada carga en formato **CSV o Excel (XLSX)**.
- ?? Arquitectura modular: backend con API REST y frontend desacoplado.

---

## ??? Tecnolog¨ªas

- **Backend**: ASP.NET Core 8, SQL Server, ClosedXML (para exportar Excel).
- **Frontend**: React 18, Vite, Fetch API.
- **Infraestructura**: Vite Proxy para comunicaci¨®n local (`localhost:5173` ¡ú `localhost:7041`).

---

## ?? Estructura

proyecto/
©À©¤©¤ backend/ # ASP.NET Core
©¦ ©À©¤©¤ Controllers/
©¦ ©¸©¤©¤ ...
©À©¤©¤ frontend/ # React + Vite
©¦ ©À©¤©¤ src/
©¦ ©¸©¤©¤ ...
©¸©¤©¤ README.md


--------------------------------------------------------

## ?? C¨®mo correr el proyecto

### ?? Backend (ASP.NET Core)
1. Ir a la carpeta `backend/`.
2. Configurar la cadena de conexi¨®n en `appsettings.json`.
3. Ejecutar:
   ```bash
   dotnet run
4. Backend disponible en: https://localhost:7041/api

?? Frontend (React + Vite)
1. Ir a la carpeta frontend/.
2. Instalar dependencias:
	npm install
3. Ejecutar en modo dev:
	npm run dev
4. Abrir en navegador: http://localhost:5173.

--------------------------------------------------------
??? Futuras mejoras

?? Autenticaci¨®n y control de roles.
?? Paginaci¨®n en la tabla de cargas.
?? Dashboard con m¨¦tricas.
?? Despliegue en la nube (Azure / AWS).

--------------------------------------------------------

?? Autor

Hardy Cruz
Analista de Datos & BI