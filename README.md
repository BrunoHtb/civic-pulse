
# CivicPulse 🌎

CivicPulse é uma API backend desenvolvida em **ASP.NET Core (.NET 10)** para ingestão, armazenamento e consulta de dados ambientais, com foco inicial em **clima** e **hidrologia**.

---

## ✨ Principais Funcionalidades

- 🔐 Autenticação **JWT (Bearer Token)** com policies
- 🌦️ Ingestão automática de dados climáticos (Open-Meteo)
- 🗄️ Persistência em **PostgreSQL (Docker)**
- 🧾 Auditoria de execuções de ingestão (`IngestionRuns`)
- 🧩 Arquitetura em camadas (Api / Core / Infrastructure)

---

## 🧱 Arquitetura

```
CivicPulse
 ├── CivicPulse.Api            # Controllers, Auth, Swagger
 ├── CivicPulse.Core           # Entidades, Enums, Interfaces, Models, Services
 ├── CivicPulse.Infrastructure # EF Core, Ingestion, Persistence
 └── docker-compose.yml        # Infraestrutura local
```

---

## 🚀 Como executar o projeto

### Pré-requisitos
- Docker + Docker Compose
- .NET SDK 10
- Git

### 1️⃣ Clonar o repositório
```bash
git clone https://github.com/seu-usuario/civic-pulse.git
cd civic-pulse
```

### 2️⃣ Subir o banco de dados
```bash
docker-compose up -d
```

### 3️⃣ Configuração da aplicação
As configurações são feitas via:
- `appsettings.json`
- `appsettings.Development.json` (ambiente local)

Exemplo de configuração no `CivicPulse.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5433;Database=civicpulse;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "CivicPulse",
    "Audience": "CivicPulse.Api",
    "Key": "CHAVE_SUPER_SECRETA_AQUI"
  }
}
```

### 4️⃣ Aplicar migrations
```bash
dotnet ef database update   --project CivicPulse.Infrastructure   --startup-project CivicPulse.Api
```

### 5️⃣ Executar a API
```bash
dotnet run --project CivicPulse.Api
```

---

## 🔐 Autenticação (JWT)

### Gerar token
Utilize o endpoint:
```
POST /api/auth/login
```

Exemplo de body:
```json
{
  "username": "admin",
  "password": "admin123"
}
```

Copie o token retornado.

### Usar no Swagger
1. Clique em **Authorize**
2. Cole:
```
Bearer SEU_TOKEN_AQUI
```

---

## ⚙️ Ingestão de dados

### Executar ingestão climática
```
POST /api/admin/ingestion/run
```

Esse endpoint:
- Consulta a API Open-Meteo
- Insere/atualiza medições
- Registra a execução em `IngestionRuns`

---

## 🛠️ Tecnologias utilizadas

- ASP.NET Core (.NET 10)
- Entity Framework Core
- PostgreSQL
- Docker / Docker Compose
- JWT Authentication
