# 🚆 Train Management System (TMS) — 100% Terminal Copy-Paste Guide

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

> 💡 *Note: If SQL Server Express was just installed, make sure the service is started with:*
> ```powershell
> Start-Service MSSQL$SQLEXPRESS
> ```

---

### 🟢 STEP 3: Unzip & Navigate to the Project Folder

Assuming the file `TMSfinal1.zip` is in your **Downloads** folder:

```powershell
# 1. Go to Downloads folder
cd "$HOME\Downloads"

# 2. Unzip the file
Expand-Archive -Path "TMSfinal1.zip" -DestinationPath "$HOME\Downloads\TMSfinal1_Extracted" -Force

# 3. Enter the unzipped project folder
cd "$HOME\Downloads\TMSfinal1_Extracted\TMSfinal1"
```

*(If the zip is on your Desktop instead, replace `$HOME\Downloads` with `$HOME\Desktop`)*.

---

### 🟢 STEP 4: Restore the Database (With All 50 Tables & Stored Records)

Run this single command inside the project directory:

```powershell
powershell -ExecutionPolicy Bypass -File ".\Database\Restore-Database.ps1"
```

✅ **Expected Output:**
```
========================================================
    Restoring Database 'TMS_2024_New' with Stored Data
========================================================
[INFO] Copied backup to C:\Users\Public\TMS_2024_New.bak for SQL Server service access.
Attempting automated restore via .NET SQL Client...

[SUCCESS] Database 'TMS_2024_New' has been successfully restored on instance 'localhost\SQLEXPRESS'!
All 50 tables, accounts, and historical register data are loaded and ready.
```

---

### 🟢 STEP 5: Launch the Application

Run this command to start the application:

```powershell
.\TMSfinal1\bin\Debug\TMSfinal1.exe
```

*(Alternatively, you can just run `.\SETUP-AND-RUN.bat`)*.

---

### 🟢 STEP 6: Login & View All Saved Records

When the application opens on your screen:

#### 🛡️ 1. Sign in as Administrator:
* Click the **ADMIN** tab.
* **Username:** `admin`
* **Password:** `Admin@123`
* Click **"SIGN IN AS ADMIN"**.

#### 👤 2. Sign in as Station Operator:
* Click the **USER** tab.
* **Username:** `sas`
* *(Or click "Create New Account" to register a new operator)*.

#### 📊 3. How to See All Saved Railway Records:
1. On the Dashboard, click any category:
   * 🚆 **Operational List** (Station Diary, Train Signal, Caution Order, SWR, etc.)
   * 🔧 **Maintenance Sub** (Failure Register, Disconnection/Reconnection, Siding Key, etc.)
   * 🏢 **Infrastructure Sub** (Power Supply, Traffic Block, Petty Repair, etc.)
   * 🛡️ **Safety List** (Emergency Key, Safety Meeting, Complete Arrival, etc.)
2. Open any register (e.g. **REG-001 Station Master's Diary**).
3. Click the blue **"VIEW RECORDS"** button in the top right.
4. **All historical and saved database records will appear in the table!**
5. For cross-register analytics and audit exports (PDF/CSV/Excel), open **REG-041 Dynamic Reports**.

#### ➕ 4. How to Add New Records:
* Fill out the form fields.
* Click **"SAVE RECORD"**.
* The new record is permanently stored in SQL Server and given an auto-incremented Log ID.

---

## ⚡ Summary of All Commands in One Block

```powershell
# 1. Install prerequisites (Run PowerShell as Admin)
winget install Microsoft.DotNet.Framework.DeveloperPack_4 -e --accept-package-agreements --accept-source-agreements
winget install Microsoft.SQLServer.2022.Express -e --accept-package-agreements --accept-source-agreements
winget install Microsoft.SQLServerManagementStudio -e --accept-package-agreements --accept-source-agreements

# 2. Extract and enter project folder
cd "$HOME\Downloads"
Expand-Archive -Path "TMSfinal1.zip" -DestinationPath "$HOME\Downloads\TMSfinal1_Extracted" -Force
cd "$HOME\Downloads\TMSfinal1_Extracted\TMSfinal1"

# 3. Restore database with stored data
powershell -ExecutionPolicy Bypass -File ".\Database\Restore-Database.ps1"

# 4. Launch Application
.\TMSfinal1\bin\Debug\TMSfinal1.exe
```
