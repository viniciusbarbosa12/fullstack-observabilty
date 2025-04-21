# 📋 fullstack-assessment

## 🧑‍💼 Employee Management System

A fullstack application for employee management, built with:

- ⚙️ **ASP.NET** (RESTful API)
- ⚛️ **React + Vite** (Frontend SPA)
- 🗄️ **SQL Server** (Relational Database)
- 📊 **Grafana, Prometheus, Loki, Tempo** (Observability Stack)
- 🐳 **Docker & Docker Compose** (Containerized Environment)

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
- 📈 Grafana: [http://localhost:3000](http://localhost:3000)  
  Login: `admin / admin`

> The first time you run this, it may take a few minutes to download dependencies and build the frontend.

---

## ✅ Default Login (for testing)

```txt
Email:    admin@admin.com
Password: admin123
```

---

## 🔍 Observability Dashboards (Grafana)

Grafana is pre-configured with dashboards and data sources:

### 📊 Dashboards Included:

- **Employee Management Observability**

  - API Request Count and Latency
  - DB Query Count and Duration
  - Errors 5xx by Endpoint
  - Logs (realtime + traceId filter)
  - Traces (Tempo)

- **CorrelationId Lookup**
  - 🔍 Search logs and traces by CorrelationId

### ⚙️ Data Sources

- **Prometheus**: API metrics (HTTP, EF Core, custom)
- **Loki**: Structured logs via Serilog
- **Tempo**: Distributed tracing via OpenTelemetry

> 📌 All dashboards and data sources are auto-provisioned via Docker volumes.

---

## 📦 Project Structure

```
fullstack-assessment/
├── backend/                    → ASP.NET  Web API
├── frontend/                   → React + Vite (SPA)
├── nginx/                      → NGINX config and static files
├── grafana/                    → Grafana provisioning (dashboards, datasources)
│   └── dashboards/             → JSON dashboards auto-loaded at startup
├── prometheus/                 → prometheus.yml + alert.rules.yml
├── tempo/                      → tempo.yaml config
├── loki/                       → loki-config.yaml + storage
├── Dockerfile.frontend
├── docker-compose.yml
└── README.md
```

---

## ⚙️ How it works

| Service              | Description                          | Port   |
| -------------------- | ------------------------------------ | ------ |
| **nginx**            | Serves frontend and proxies API      | `80`   |
| **backend**          | ASP.NET API                          | `5000` |
| **sqlserver**        | SQL Server 2022                      | `1433` |
| **prometheus**       | Collects metrics                     | `9090` |
| **grafana**          | Observability dashboards             | `3000` |
| **loki**             | Log aggregation                      | `3100` |
| **tempo**            | Distributed traces (OTLP + Tempo UI) | `3200` |
| **alertmanager**     | Alert routing (email-ready)          | `9093` |
| **frontend-builder** | Builds frontend & copies to NGINX    | —      |

---

## 🧪 Development Tips

- Inspect backend logs:

  ```bash
  docker logs backend
  ```

- Access Prometheus directly:
  [http://localhost:9090](http://localhost:9090)

- View logs in Grafana (Loki):

  - 📌 Go to _Explore > Logs_
  - Select `Loki` and use `{app="EmployeeManagementApi"}`

- View traces (Tempo):
  - Use `Explore > Tempo`
  - Query: `{resource.service.name="EmployeeManagementApi"}`

---

## 📬 Alerting (Optional)

Email alerts via AlertManager are pre-configured (uses Gmail SMTP). To enable:

1. Generate a Gmail App Password [here](https://myaccount.google.com/apppasswords)
2. Add your app password in: `alertmanager/config.yml`
3. Restart containers:
   ```bash
   docker compose down
   docker compose up --build
   ```

---

## ✅ Status

✅ Fully containerized  
✅ Observability (metrics, logs, traces)  
✅ Ready for production usage & interview assessments
