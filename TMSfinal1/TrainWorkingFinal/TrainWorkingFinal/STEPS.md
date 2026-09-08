# 🚆 Train Working Management System (TMS) — 100% Terminal Copy-Paste Guide

> **For:** Anyone starting on a completely clean Windows computer.  
> **Goal:** Run everything purely through Terminal (PowerShell) commands from unzipping to viewing pre-loaded data.

---

## 📋 Complete Copy-Paste Terminal Workflow

---

### 🟢 STEP 1: Open PowerShell as Administrator

1. Press **Windows Key** on your keyboard.
2. Type **`powershell`**.
3. Right-click **Windows PowerShell** and click **"Run as Administrator"**.

---

### 🟢 STEP 2: Install Required Software (Copy-Paste in PowerShell)

Copy and paste these commands into PowerShell one by one (press `Enter` after each):

```powershell
# 1. Install .NET Framework 4.7.2
winget install Microsoft.DotNet.Framework.DeveloperPack_4 -e --accept-package-agreements --accept-source-agreements

# 2. Install Microsoft SQL Server 2022 Express Database Engine
winget install Microsoft.SQLServer.2022.Express -e --accept-package-agreements --accept-source-agreements

# 3. Install SQL Server Management Studio (SSMS) & Tools
winget install Microsoft.SQLServerManagementStudio -e --accept-package-agreements --accept-source-agreements
```

> 💡 *Note: If SQL Server Express was just installed, ensure the service is running:*
> ```powershell
> Start-Service MSSQL$SQLEXPRESS
> ```

---

### 🟢 STEP 3: Navigate to the Project Folder

```powershell
cd "c:\Users\Asus\Zeta_resp\TMSfinal1\TrainWorkingFinal"
```

---

### 🟢 STEP 4: Restore the Database (With All Authority Tables & Stored Records)

Run this single command inside the project directory:

```powershell
powershell -ExecutionPolicy Bypass -File ".\Database\Restore-Database.ps1"
```

✅ **Expected Output:**
```
========================================================
    Restoring Database 'TrainManagementDB' with Stored Data
========================================================
[INFO] Copied backup to C:\Users\Public\TrainManagementDB.bak for SQL Server service access.
Attempting automated restore via .NET SQL Client...

[SUCCESS] Database 'TrainManagementDB' has been successfully restored on instance 'localhost\SQLEXPRESS'!
All authority tables, accounts, and historical register data are loaded and ready.
```

---

### 🟢 STEP 5: Launch the Application

Run this command to compile & start the application:

```powershell
.\SETUP-AND-RUN.bat
```

*(Or run `.\TrainWorkingApp\bin\Debug\TrainWorkingApp.exe`)*.

---

### 🟢 STEP 6: Login & View All Saved Records

When the application opens on your screen:

#### 🛡️ 1. Sign in as Administrator:
* Click the **ADMIN** tab.
* **Username:** `admin`
* **Password:** `Admin@123`
* Click **"SIGN IN AS ADMIN"**.

#### 👤 2. Sign in as Station Operator:
* Click the **OPERATOR / USER** tab.
* Sign in with your operator username & password (provided by Administrator).

#### 📊 3. How to See All Saved Railway Authority Records:
1. On the Dashboard, choose any classification:
   * 🚦 **Signal & Defective Authorities** (Advance Authority T/369-3b, Passing Signal ON, Caution Order, Common Starter, etc.)
   * 🎫 **Line Clear & Block Working** (Line Clear Inquiry, Paper Tickets T/A 1425, Single Line Working T/D 602, etc.)
   * 🔧 **Maintenance, S&T & Trolley** (S&T Disconnection T/351, Motor Trolley T/1518, Push Trolleys, Shunting Orders T/806)
   * 🚨 **Emergency, Relief & Movements** (Relief Train T/A 602, ABS Relief Engine, Communication Failure T/C 602, Train Movement Log)
2. Open any authority form (e.g. **Advance Authority Defective Signal**).
3. Click the blue **"VIEW RECORDS"** button in the top right.
4. **All historical and saved database records will appear in the table!**
5. For cross-module dynamic search and PDF/Excel/CSV exports, open **Cross-Register Reports & Exports**.

#### ➕ 4. How to Add New Records:
* Fill out the form fields.
* Click **"SAVE AUTHORITY"**.
* The new record is permanently stored in SQL Server and given an auto-incremented reference number.
