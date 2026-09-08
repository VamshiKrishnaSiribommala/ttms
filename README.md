# 🚆 Train Management System (TMS) & Train Working System

> **Production-Quality Cross-Platform C# Application**  
> Supports **Windows 10/11** and **Linux (Ubuntu/Debian)** with **100% Shared Codebase**, **.NET 8 LTS**, and **Dockerized SQL Server 2022**.

---

## 📌 1. Project Overview

The **Train Management System (TMS)** is a statutory digital station operations suite engineered for Indian Railways. It consolidates:
1. **Station Registers (TMS)**: 41 official electronic logbooks including the Station Master's Diary (`REG-001`), Train Signal Register (`REG-002`), Caution Orders (`REG-004`), Gear Failure Log (`REG-005`), S&T Disconnections (`REG-006`), and Inspection records.
2. **Train Working Authorities (TW)**: 22 statutory clearance authority certificates including Advance Defective Signal (`AUTH-001`), Passing Signals at ON (`AUTH-002`), Caution Orders (`AUTH-003`), and Paper Line Clear Tickets (`AUTH-018`).
3. **Electronic Memos & Mobile Dispatch**: Direct one-click transmission of official station memorandums to Traffic Controllers and S&T Inspectors via WhatsApp & Telegram.
4. **Dynamic Auditing & Reporting**: Search, date range filtering, compliance tracking, and CSV/PDF export across all 52 TMS tables and 24 Train Working tables.

---

## 🏛️ 2. Architecture & Cross-Platform Design

### Architectural Evolution
```text
Previous Legacy Architecture:
.NET Framework 4.7.2 ──► Windows Forms ──► Localhost SQLEXPRESS ──► Windows Only (exe)

Modern Cross-Platform Architecture:
                 Git Repository
                       │
         ┌─────────────┴─────────────┐
         ▼                           ▼
      Windows                      Linux
    (.NET 8 LTS)                (.NET 8 LTS)
         │                           │
         ├───────────────────────────┤
         ▼                           ▼
 ┌───────────────────────────────────────────┐
 │            TMS.Web (ASP.NET Core)         │
 │  • Browser-based (http://localhost:5000)  │
 │  • 41 Station Registers (REG-001..041)    │
 │  • 22 Train Authorities (AUTH-001..022)   │
 │  • PBKDF2 Modern Cryptography (Auth)      │
 │  • Dynamic Reports & Mobile Dispatch      │
 ├───────────────────────────────────────────┤
 │            TMS.Core (.NET 8 ClassLib)     │
 │  • Microsoft.Data.SqlClient               │
 │  • Configurable Connection Strings        │
 └─────────────────────┬─────────────────────┘
                       ▼
 ┌───────────────────────────────────────────┐
 │          Docker Compose (SQL 2022)        │
 │  • mcr.microsoft.com/mssql/server:2022    │
 │  • TMS_2024_New.bak (52 Tables)           │
 │  • TrainManagementDB.bak (24 Tables)      │
 └───────────────────────────────────────────┘
```

---

## 💻 3. Prerequisites

| Tool | Windows 10/11 | Linux (Ubuntu / Debian) |
| :--- | :--- | :--- |
| **.NET SDK** | .NET 8 SDK or .NET 10 SDK | `.NET 8 SDK` (`sudo apt install dotnet-sdk-8.0`) |
| **Container Engine**| Docker Desktop for Windows | Docker Engine & Docker Compose plugin |
| **Git** | Git for Windows | `sudo apt install git` |

---

## ⚡ 4. Quick Start: Windows Setup

Run the following commands in **PowerShell**:

```powershell
# 1. Clone the repository
git clone https://github.com/VamshiKrishnaSiribommala/ttms.git
cd ttms

# 2. Copy the environment configuration template
Copy-Item .env.example .env

# 3. Start SQL Server 2022 in Docker (automatically restores all databases)
docker compose up -d

# 4. Restore & Build the solution
dotnet restore
dotnet build

# 5. Launch the application
dotnet run --project TMS.Web/TMS.Web.csproj
```

Open your browser at: **`http://localhost:5000`**

---

## 🐧 5. Quick Start: Linux Setup (Ubuntu / Debian)

Run the following commands in **Terminal / Bash**:

```bash
# 1. Install .NET 8 SDK and Docker (if not installed)
sudo apt update
sudo apt install -y dotnet-sdk-8.0 docker.io docker-compose-v2

# Ensure user can run Docker without sudo
sudo usermod -aG docker $USER
newgrp docker

# 2. Clone the repository
git clone https://github.com/VamshiKrishnaSiribommala/ttms.git
cd ttms

# 3. Copy the environment configuration template
cp .env.example .env

# 4. Start SQL Server 2022 in Docker (automatically restores all databases)
docker compose up -d

# 5. Restore & Build the solution
dotnet restore
dotnet build

# 6. Launch the application
dotnet run --project TMS.Web/TMS.Web.csproj
```

Open your browser at: **`http://localhost:5000`**

---

## 🗄️ 6. Database Setup & Management

The database environment runs identical MS SQL Server 2022 instances in Docker on both Windows and Linux.

### Automated Initialization
When you run:
```bash
docker compose up -d
```
Docker Compose launches:
1. `tms-sqlserver`: The primary Microsoft SQL Server 2022 container.
2. `tms-db-init`: An idempotent init container that automatically executes `Database/restore-databases.sql`. It restores `TMS_2024_New.bak` (52 tables) and `TrainManagementDB.bak` (24 tables) with all historical registers and accounts.

### Manual Database Restoration (Fallback)
If you ever want to reset or re-restore the databases:

* **On Linux**:
  ```bash
  chmod +x Database/init-databases.sh
  ./Database/init-databases.sh
  ```
* **On Windows**:
  ```powershell
  pwsh ./Database/init-databases.ps1
  ```

### Windows Local SQLEXPRESS Fallback
If you are developing on Windows and prefer your local Windows SQL Server Express instance instead of Docker, `TMS.Web` includes **automatic fallback detection**. Simply ensure `MSSQL$SQLEXPRESS` is running; `TMS.Web` will seamlessly connect to `localhost\SQLEXPRESS`.

---

## 🔑 7. Default Credentials & Authentication

| Role | Username | Default Password | Access |
| :--- | :--- | :--- | :--- |
| **System Administrator** | `admin` | `Admin@123` | Full administrative control, user approvals, audit log, all 41 registers & 22 authorities |
| **Station Staff** | Self-registered | Configured at registration | Station register entries, train working authorities, record viewing |

> 🔒 **Security Notice**: Passwords use salted **PBKDF2 SHA-256** cryptography with 10,000 iterations. Backward compatibility with legacy SHA-1 hashes from existing backups is preserved.

---

## ⚙️ 8. Environment Variables & Configuration

Configuration is managed via `.env` and `TMS.Web/appsettings.json`:

```env
# Microsoft SQL Server SA Credentials
MSSQL_SA_PASSWORD=YourStrong@Password123

# Database Connection Strings (Overrides appsettings.json)
ConnectionStrings__TMSConnection=Server=localhost,1433;Database=TMS_2024_New;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;Connect Timeout=10;
ConnectionStrings__TrainWorkingConnection=Server=localhost,1433;Database=TrainManagementDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;Connect Timeout=10;

# Operating Context
AppSettings__StationName=Secunderabad Junction (SC)
AppSettings__Division=Secunderabad (SC)
AppSettings__Zone=South Central Railway (SCR)
```

---

## 📁 9. Project Directory Structure

```text
TMSfinal1/
├── TMS.slnx                    # Root Solution File
├── docker-compose.yml          # Docker SQL Server & Idempotent Init Service
├── Dockerfile                  # Production Container Build
├── .env.example                # Safe environment variables template
├── .gitignore                  # Visual Studio & .NET Git Exclusion Rules
├── Database/
│   ├── TMS_2024_New.bak        # Backed up TMS database (52 tables)
│   ├── TrainManagementDB.bak   # Backed up Train Working database (24 tables)
│   ├── restore-databases.sql   # Idempotent cross-platform restore SQL script
│   ├── init-databases.sh       # Linux / Bash DB restore script
│   └── init-databases.ps1      # Windows / PowerShell DB restore script
├── TMS.Core/                   # 100% Platform-Independent Business Layer (.NET 8 LTS)
│   ├── Auth/
│   │   └── AuthService.cs      # Modern PBKDF2 authentication & user management
│   ├── Database/
│   │   └── DatabaseHelper.cs   # Microsoft.Data.SqlClient cross-platform helper
│   ├── Models/
│   │   └── RegistryMetadata.cs # Schema metadata for all 41 registers & 22 authorities
│   └── Services/
│       └── RecordDispatchService.cs # Path-independent memo & mobile dispatch
├── TMS.Web/                    # Cross-Platform Web Portal (.NET 8 LTS)
│   ├── Program.cs              # DI, cookie auth, fallback connection detection
│   ├── appsettings.json        # Production / Docker configurations
│   ├── appsettings.Development.json # Development fallback configurations
│   ├── Controllers/            # Account, Dashboard, Registers, Authorities, Reports
│   └── Views/                  # Responsive Indian Railways styled Razor views
└── TMSfinal1/                  # Original Windows Desktop Project & Forms
```

---

## 🛠️ 10. Troubleshooting & Common Errors

### 1. `Cannot connect to SQL Server on localhost:1433`
* Check if Docker is running: `docker ps`
* Verify the SQL container health: `docker compose ps`
* If port 1433 is in use by a local Windows SQL Server instance, either stop the local SQL service via `net stop MSSQL$SQLEXPRESS` or map the container to port 1434 (`1434:1433`) in `docker-compose.yml` and `.env`.

### 2. `Database restore failed / Backup device error`
* In Docker, paths must be Unix-style (`/var/opt/mssql/backup/TMS_2024_New.bak`). The provided `restore-databases.sql` script handles this automatically.

### 3. `Permission denied for docker` on Linux
* Run `sudo usermod -aG docker $USER && newgrp docker` to allow running Docker commands without `sudo`.

---

## 📜 License
Developed for Indian Railways operations compliance.
