# 📋 fullstack-assessment

## 🧑‍💼 Employee Management System

A fullstack application for employee management, built with:

- ⚙️ **ASP.NET 6** (RESTful API)
- ⚛️ **React + Vite** (Frontend SPA)
- 🗄️ **SQL Server** (Relational Database)
- 🐳 **Docker & Docker Compose** (Containerized environment)

---

## 🐳 How to Run (Docker Only)

Make sure Docker and Docker Compose are installed. Then run:

```bash
docker compose up --build
```

The app will be available at:

- 🌐 Frontend: [http://localhost](http://localhost)
- 🔌 API: [http://localhost:5000/api](http://localhost:5000/api)
- 📑 Swagger: [http://localhost:5000/swagger](http://localhost:5000/swagger)

> The first time you run this, it may take a few minutes to download dependencies and build the frontend.

---

## ✅ Default Login (for testing)

```txt
Email:    admin@admin.com
Password: admin123
```

---

## 🧱 Project Structure

```
fullstack-assessment/
├── backend/         → ASP.NET 6 Web API
├── frontend/        → React + Vite (SPA)
├── nginx/           → NGINX config and static files
│   ├── default.conf
│   └── html/        → Auto-filled with frontend build
├── Dockerfile.frontend
├── docker-compose.yml
└── README.md
```

---

## ⚙️ How it works

| Service              | Description                      | Exposed Ports          |
| -------------------- | -------------------------------- | ---------------------- |
| **nginx**            | Serves frontend and proxies API  | `80 → localhost`       |
| **backend**          | ASP.NET 6 API                    | `5000` (internal only) |
| **sqlserver**        | SQL Server 2022                  | `1433` (for dev only)  |
| **frontend-builder** | Builds React and copies to NGINX | _no exposed ports_     |

> The frontend is built by a dedicated container (`frontend-builder`) and copied to `nginx/html` via shared volume.

---

## 📦 Data Persistence

To persist SQL Server data across restarts, a named volume is recommended:

```yaml
volumes:
  sqlserver_data:
```

Then mount it:

```yaml
sqlserver:
  volumes:
    - sqlserver_data:/var/opt/mssql
```

---

## 🧪 Development Tips

- To inspect the backend logs:
  ```bash
  docker logs backend
  ```
- To inspect the database:
  Use a SQL tool like Azure Data Studio and connect to `localhost:1433`  
  User: `sa`  
  Password: `Admin123!`

---
