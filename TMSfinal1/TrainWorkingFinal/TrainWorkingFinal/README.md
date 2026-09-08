<div align="center">

# 🚆 INDIAN RAILWAYS — TRAIN WORKING MANAGEMENT SYSTEM (TMS)

### 🇮🇳 *Centralized Digital Solution for Operational Authorities, Block Working & Train Movements*

<br/>

<!-- Technology Badges -->
<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%20Forms%20Desktop-0078D4?style=for-the-badge&logo=windows&logoColor=white" alt="Platform" />
  <img src="https://img.shields.io/badge/Framework-.NET%20Framework%204.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET" />
  <img src="https://img.shields.io/badge/Language-C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/Database-MS%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="Database" />
  <img src="https://img.shields.io/badge/Reports-PDF%20%7C%20Excel%20%7C%20CSV-FF6F00?style=for-the-badge&logo=adobeacrobatreader&logoColor=white" alt="Reports" />
</p>

<br/>

---

</div>

# 🎯 Aim & Main Objective of the Project

The primary goal of the **Train Working Management System (TMS)** is to digitize and safeguard all station operational authorities, signal passing permissions, line clear grants, block working exceptions, and emergency relief records into a secure, centralized database.

The system encapsulates:
- 🚦 **Signal Authorities**: Advance Defective Signal Authority (T/369-3b), Passing Signal at ON, Common Starter (T/512), Signal Passing Warnings.
- 🎫 **Block & Line Clear**: Line Clear Inquiries & Grants (TFC-1 / TFC-2), Paper Line Clear Tickets (T/A 1425 & T/B 1425), Single Line Working on Double Line (T/D 602).
- 🔧 **Maintenance & Trolleys**: S&T Disconnection / Reconnection (S&T T/351), Motor Trolley Permits (T/1518), Push Trolley Notices, Yard Shunting Orders (T/806).
- 🚨 **Emergency & Relief**: Relief Train Authorization (T/A 602), ABS Relief Engine Authorities (T/A 912), Total Communication Failure Logs (T/C 602), Train Movement & Running Registers.

---

# 🌟 Key Features

1. **Enterprise IRCTC / Indian Railways Design System**:
   - Official palette: Indian Railways Blue (`#213D77`), Deep Navy (`#0E2D5F`), IRCTC Action Orange (`#FB792B`), Emerald Green (`#16A34A`).
   - Anti-flicker double-buffering (`WS_EX_COMPOSITED`) on all windows.
   - Live digital railway clock and top-right active operator badges.
2. **PBKDF2 Cryptographic Security**:
   - Password hashing with random 16-byte salt and 10,000 PBKDF2 iterations.
   - Dual-role authentication (`Admin` / `Operator`).
   - 3-point identity verification for password recovery with full audit logs (`TMS_PasswordResetLogs`).
3. **Comprehensive Validation Engine**:
   - Implements all 22 module rules derived directly from `TMS_Train_Working_TestCases (2).xlsx`.
   - Real-time error messages with clear reasons and expected valid formats.
4. **Dynamic Reporting & Multi-Format Exports**:
   - Cross-register query console with date range and text filtering.
   - 1-Click export to **PDF / Print Preview**, **Excel**, and **CSV**.
5. **1-Click Database Setup & Launch**:
   - `SETUP-AND-RUN.bat` and PowerShell scripts for multi-instance automatic database restoration.

---

# ⚡ Quick Start (Terminal Commands)

```powershell
# 1. Enter Project Folder
cd "c:\Users\Asus\Zeta_resp\TMSfinal1\TrainWorkingFinal"

# 2. Restore Database
powershell -ExecutionPolicy Bypass -File ".\Database\Restore-Database.ps1"

# 3. Launch Application
.\SETUP-AND-RUN.bat
```

---

# 🔑 Default Login Credentials

* **🛡️ Admin Login**: Username: `admin` | Password: `Admin@123`
* **👤 Operator Login**: Click *Operator / User* tab -> Register a new operator account or sign in.
