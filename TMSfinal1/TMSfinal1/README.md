<div align="center">

# 🚆 INDIAN RAILWAYS — TRAIN MANAGEMENT SYSTEM (TMS)

### 🇮🇳 *Smart Digital Solution for Railway Station Registers & Centralized Operational Control*

<br/>

<!-- Animated Typing Banner -->
<a href="#">
  <img src="https://readme-typing-svg.demolab.com?font=Outfit&weight=700&size=26&duration=2500&pause=1000&color=00D2FF&center=true&vCenter=true&width=850&height=55&lines=Centralized+Digital+Train+Management;Safely+Monitor+%26+Control+Train+Movements;41+Operational+Registers+Digitized;Instant+Cross-Register+Dynamic+Audit+Reports" alt="TMS Animated Header" />
</a>

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

The main objective of the **Train Management System (TMS)** is to safely monitor, control, and manage train movements by replacing manual railway registers with a centralized digital system.

The system stores train movement information, captures operational records, manages railway station registers, and generates reports for:

- 🚆 Train Operations
- 🛡️ Safety Monitoring
- 📋 Operational Control
- 🔍 Audits and Inspections
- 📊 Management Reporting

> **The goal is simple: Replace manual paperwork with a centralized, searchable, secure, and efficient digital railway management system.**

---

# 🌟 Why Train Management System (TMS)?

Traditional railway station operations depend on multiple physical paper registers maintained manually by Station Masters, Traffic Inspectors, and Signal & Telecom Maintainers.

The **Train Management System (TMS)** transforms this manual process into a centralized digital platform.

<table>
<tr>

<td width="48%" valign="top">

## 🛑 Traditional Paper System

❌ **41 separate physical registers**

❌ Risk of damaged, torn, or misplaced records

❌ Manual shift handovers

❌ Time-consuming inspection preparation

❌ Difficult to search old records

❌ Hundreds of pages must be checked manually

❌ Manual compilation of audit reports

</td>

<td width="4%" align="center">

# ➜

</td>

<td width="48%" valign="top">

## ⚡ Centralized Digital TMS

✅ **One centralized digital system**

✅ Permanent records stored in SQL Server

✅ Digital shift handovers with timestamps

✅ Faster inspection and audit preparation

✅ Real-time search across registers

✅ Easy access to historical records

✅ Instant PDF, Excel, and CSV reports

</td>

</tr>
</table>

---

> ## 🚀 Result
>
> **Instead of managing 41 separate paper registers, railway staff can manage operational information through one centralized Train Management System.**

---

# 🏛️ System Architecture Workflow

```mermaid
flowchart TD

    A[👥 Station Master / Inspector / Maintainer]
    A --> B[🔐 Login & Session Management]

    B --> C[🎛️ Centralized TMS Dashboard]

    C --> D1[🔵 Station Operations & Traffic]
    C --> D2[🟢 Signal & Telecom Maintenance]
    C --> D3[🟠 Infrastructure & Electrical Power]
    C --> D4[🔴 Safety & Audit]

    D1 --> E[(🗄️ Centralized SQL Server Database)]
    D2 --> E
    D3 --> E
    D4 --> E

    E --> F[📊 REG-041 Dynamic Reports Engine]

    F --> G1[📄 PDF Reports]
    F --> G2[📊 Excel Export]
    F --> G3[📋 CSV Export]
    F --> G4[🖨️ Print Reports]
```

---

# 📋 The 41 Operational Registers

The Train Management System digitizes **41 important railway operational registers**.

They are organized into four major operational departments.

| Department | Registers | Main Purpose |
|---|:---:|---|
| 🔵 Station Operations & Traffic | 17 | Train operations, handovers, attendance, complaints |
| 🟢 Signal & Telecom Maintenance | 10 | Signal equipment, testing, failures, maintenance |
| 🟠 Infrastructure & Electrical Power | 4 | Power blocks, sidings, repairs, DG logs |
| 🔴 Safety & Audit Engine | 10 | Safety inspections, meetings, audits, reporting |
| **TOTAL** | **41** | **Centralized Railway Operational Management** |

---

# 🔵 Department 1: Station Operations & Traffic

### Total Registers: 17

> Safely manage daily train traffic, station operations, staff handovers, attendance, and passenger or employee grievances.

| Register Code | Register Name | Operational Scope & Tracked Information |
|---|---|---|
| `REG-001` | **Station Master's Diary** | Daily station operational log, category classification, event narrative, and reporting details. |
| `REG-002` | **Train Signal Register** | Train number, UP/DOWN direction, platform or line, arrival, departure, and passed times. |
| `REG-003` | **SWR Acknowledgment** | Station Working Rules acknowledgment, staff ID, rule version, reading date, and verifier details. |
| `REG-004` | **Caution Order Register** | Speed restrictions, section details, engineering reason, and validity period. |
| `REG-007` | **Bio-Metric Attendance** | Staff attendance, assigned shift, punch timestamps, and duty presence. |
| `REG-008` | **Stabled Load Register** | Train or rake ID, stabling line, stabled time, and handbrake safety checks. |
| `REG-011` | **Public Complaints** | Passenger details, ticket or PNR information, complaint category, and grievance details. |
| `REG-012` | **Staff Grievances** | Employee ID, issue type, grievance subject, and resolution remarks. |
| `REG-021` | **Complete Arrival Register** | Train number, arrival time, wagon count, guard ID, and Station Master verification. |
| `REG-022` | **Control Instructions** | Directives from the Section Controller, message type, validity period, and acknowledgment. |
| `REG-023` | **SM Relief Diary** | Relieving SM, relieved SM, handover time, pending operational issues, and weather information. |
| `REG-028` | **Staff Biodata & Training** | Employee information, medical examination, safety training, and competency expiry. |
| `REG-029` | **Assurance Register** | Safety circular acknowledgment, document information, rule version, and digital sign-off. |
| `REG-030` | **Private Number (PN) Sheet** | Private number exchanges, communication purpose, train number, and recipient Station Master. |
| `REG-032` | **Attendance Late Marks** | Shift timing compliance, late arrival remarks, and supervisor authorization. |
| `REG-033` | **Passenger Complaint Log** | Detailed passenger grievance tracking, PNR, department, and complaint status. |
| `REG-034` | **Employee Complaint Log** | Internal employee complaints, urgency level, and resolution status. |

---

# 🟢 Department 2: Signal & Telecom (S&T) Maintenance

### Total Registers: 10

> Tracks signaling equipment, point machines, interlocking tests, failures, and maintenance activities.

| Register Code | Register Name | Operational Scope & Tracked Information |
|---|---|---|
| `REG-005` | **Signal / Point / Block Failure** | Asset type, asset ID, failure time, reported maintainer, and rectification details. |
| `REG-006` | **S&T Discon / Recon** | Equipment disconnection permit, gear ID, maintainer details, approval, and reconnection time. |
| `REG-014` | **Miscellaneous Counter** | Emergency Route Release, Calling-on counters, old value, new value, and authorization reference. |
| `REG-016` | **Crank Handle Register** | Point number, crank handle ID, authorization number, operator details, and usage record. |
| `REG-017` | **Crank Handle Testing** | Handle ID, test date, test type, tester ID, and test outcome. |
| `REG-018` | **Cross-Over Testing** | Crossover identification, locking verification, detection verification, and maintainer details. |
| `REG-019` | **Signal Failure Register** | Signal ID, failure time, failure classification, root cause, and repair actions. |
| `REG-020` | **Emergency Key Register** | Emergency key ID, asset ID, checkout time, return time, and authorization details. |
| `REG-038` | **Joint Inspection** | Joint inspection involving departments, asset ID, measured values, and observations. |
| `REG-040` | **Failure Rectification** | Failure reference, root cause, repair actions, and post-repair testing result. |

---

# 🟠 Department 3: Infrastructure & Electrical Power

### Total Registers: 4

> Manages sidings, traffic and power blocks, maintenance work, and electrical power records.

| Register Code | Register Name | Operational Scope & Tracked Information |
|---|---|---|
| `REG-015` | **Siding Key Register** | Siding key ID, issued staff, purpose, checkout and return time, and safety checklist. |
| `REG-024` | **Traffic / Power Block** | Block type, section, granted time, and cancellation time. |
| `REG-031` | **Petty Repairs** | Asset category, asset ID, defect description, assigned department, and completion status. |
| `REG-035` | **Power Supply & DG Log** | Primary power source, failure time, DG running hours, and diesel fuel information. |

---

# 🔴 Department 4: Safety & Audit Engine

### Total Registers: 10

> Handles regulatory compliance, officer inspections, safety meetings, audits, and consolidated reporting.

| Register Code | Register Name | Operational Scope & Tracked Information |
|---|---|---|
| `REG-009` | **Fog Signalman Deployment** | Staff ID, deployment location, detonator count, and shift start and end times. |
| `REG-010` | **Night Inspection Register** | Inspecting officer, visit time, staff alertness, and night-working observations. |
| `REG-013` | **Inspection & Observations** | Inspection scope, deficiencies observed, and compliance due date. |
| `REG-025` | **Safety Meeting Register** | Meeting type, chairperson, attendees, agenda, minutes, and action items. |
| `REG-026` | **HQ Safety Circulars** | Circular reference, subject, effective date, and Station Master acknowledgment. |
| `REG-027` | **Safety Meeting Part 2** | Deliberations, safety directives, assigned staff, and deadlines. |
| `REG-036` | **Officers Inspection** | Officer inspection information, station details, irregularities, and priority. |
| `REG-037` | **Traffic Inspector (TI) Audit** | Audit findings, operational observations, rule violations, and instructions. |
| `REG-039` | **Night Inspection Part 2** | Signal visibility checks, safety equipment status, and corrective actions. |
| `REG-041` | **Consolidated Dynamic Reports** | Cross-register querying, date filtering, and PDF, Excel, CSV, and print reporting. |

---

# ⭐ REG-041 — Consolidated Dynamic Reports

The **REG-041 Dynamic Reports Module** is the central reporting and analytical engine of the Train Management System.

It collects operational information from multiple railway registers and presents it in a unified reporting workspace.

## 🔄 Simple Data Flow

```mermaid
flowchart LR

    A[📚 Operational Registers<br/>REG-001 to REG-040]

    A --> B[(🗄️ SQL Server Database)]

    B --> C[📊 REG-041 Dynamic Reports]

    C --> D[🔍 Search & Filter]
    C --> E[📄 PDF]
    C --> F[📊 Excel]
    C --> G[📋 CSV]
    C --> H[🖨️ Print]
```

---

## 📅 Available Report Filters

| Filter | Description |
|---|---|
| **Today** | View records created today |
| **Yesterday & Today** | View records from the latest 2 days |
| **Last 3 Days** | View records within a maximum 3-day range |
| **Register Filter** | View records from a selected register |
| **Search** | Search using train numbers, staff IDs, keywords, and other data |

---

## 💎 Key Capabilities of Dynamic Reports

### ⚡ Fast Cross-Register Querying

Queries operational information across multiple SQL Server register tables using optimized date filters.

### 📅 Strict 1–3 Day Operational Windows

Provides quick report presets:

- `Today`
- `Yesterday & Today`
- `Last 3 Days`

The reporting window is limited to a maximum of three days to maintain focused operational reporting and better performance.

### 🔄 Dynamic Data Retrieval

Records can be retrieved based on:

- Date range
- Selected register
- Search keywords
- Train information
- Staff information
- Operational records

### 📄 Multi-Page PDF Reports

Generate structured PDF reports containing:

- Report headers
- Register information
- Operational records
- Automatic page breaks
- Page numbering

### 📊 Excel Export

Export structured operational information into Excel-compatible files for further analysis and management review.

### 📋 CSV Export

Generate standard CSV files for data sharing and external processing.

### 🖨️ Direct Printing

Reports can be printed directly from the application.

### 🖱️ Horizontal Scrolling

Wide report tables support horizontal navigation for easier viewing of multiple columns.

---

> ## 🏆 REG-041 is the Heart of the System
>
> **It brings information from multiple railway operational registers into a centralized reporting environment, helping staff search, analyze, export, print, and review operational data efficiently.**

---

# ⚡ Key System Features

| Feature | Description |
|---|---|
| 🔐 User Login | Secure user login and session management |
| 🎛️ Centralized Dashboard | Single navigation point for all registers |
| 📚 41 Digital Registers | Digitization of railway operational registers |
| 🗄️ SQL Server Database | Centralized storage of operational records |
| 🔍 Search | Search information across records |
| 📊 Dynamic Reports | Consolidated reporting through REG-041 |
| 📄 PDF Export | Multi-page operational reports |
| 📊 Excel Export | Export structured data for analysis |
| 📋 CSV Export | Standard CSV data export |
| 🖨️ Print Support | Direct printing of reports |
| 🔄 Digital Handovers | Electronic record management and timestamps |

---

# ⚡ Quick Start & Installation

## Step 1: Open the Solution

Open either:

```text
TMSfinal1.slnx
```

or:

```text
TMSfinal1.sln
```

using Visual Studio.

Recommended versions:

- Visual Studio 2019
- Visual Studio 2022
- Compatible newer Visual Studio versions

---

## Step 2: Install Required Components

Make sure the following software and components are available:

- Windows
- Visual Studio
- .NET Framework 4.7.2
- Microsoft SQL Server
- SQL Server Express or a compatible SQL Server instance

---

## Step 3: Verify Database Connection

Open:

```text
TMSfinal1/App.config
```

Verify the database connection string:

```xml
<connectionStrings>
  <add
      name="TMSConnection"
      connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=TMS_2024_New;Integrated Security=True;TrustServerCertificate=True"
      providerName="System.Data.SqlClient" />
</connectionStrings>
```

Make sure:

1. SQL Server is running.
2. Your SQL Server instance name is correct.
3. The database `TMS_2024_New` exists.
4. The required register tables have been created.

If your SQL Server instance is different, change:

```text
localhost\SQLEXPRESS
```

to your actual SQL Server instance name.

---

## Step 4: Restore or Create the Database

Navigate to:

```text
Database/
```

Run the SQL scripts provided in the project to create:

- Database tables
- Register tables
- Required schema
- Supporting database objects

---

## Step 5: Build the Application

Inside Visual Studio:

```text
Build
   ↓
Build Solution
```

Or use:

```text
Ctrl + Shift + B
```

Fix any missing package, dependency, or SQL Server configuration issue if Visual Studio displays an error.

---

## Step 6: Run the Application

Press:

```text
F5
```

or click:

```text
▶ Start
```

The Train Management System will compile and launch.

---

# 📁 Project Structure

```text
📦 Train_Station_Registers
│
├── 📂 TMSfinal1
│   │
│   ├── 📄 Form_Reg001_StationDiary.cs
│   ├── 📄 Form_Reg002_TrainSignal.cs
│   ├── 📄 Form_Reg003_SWRAcknowledgment.cs
│   ├── 📄 ...
│   ├── 📄 Form_Reg040_FailureInspection.cs
│   │
│   ├── 📄 Form_Reg041_DynamicReports.cs
│   │
│   ├── 📄 DatabaseHelper.cs
│   ├── 📄 SessionManager.cs
│   ├── 📄 ThemeManager.cs
│   ├── 📄 App.config
│   └── 📄 Program.cs
│
├── 📂 Database
│   ├── 📄 SQL Table Scripts
│   └── 📄 Schema Files
│
├── 📄 .gitignore
├── 📄 README.md
│
├── 📄 TMSfinal1.sln
└── 📄 TMSfinal1.slnx
```

---

# 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| **C#** | Main application programming language |
| **Windows Forms** | Desktop application user interface |
| **.NET Framework 4.7.2** | Application framework |
| **Microsoft SQL Server** | Centralized database |
| **ADO.NET** | Database connectivity |
| **PDF Reporting** | Multi-page report generation |
| **Excel Export** | Spreadsheet-based reporting |
| **CSV Export** | Standard data export |
| **Visual Studio** | Development environment |

---

# 🎯 Project Objective Summary

The Train Management System is designed to support the digital transformation of railway station operational record management.

The system provides a centralized approach for:

```text
Manual Paper Registers
        │
        ▼
Digital Register Management
        │
        ▼
Centralized SQL Server Storage
        │
        ▼
Searchable Operational Records
        │
        ▼
Dynamic Reporting
        │
        ├── 📄 PDF
        ├── 📊 Excel
        ├── 📋 CSV
        └── 🖨️ Print
```

---

# 🚀 Benefits of the System

The Train Management System helps improve railway operational record management by providing:

- 📚 Reduced dependency on physical paper registers
- 🔍 Faster searching of historical records
- 🗄️ Centralized SQL Server data storage
- 🔄 Improved shift handover management
- 📊 Easier audit and inspection reporting
- 📄 Instant report generation
- 📈 Better operational visibility
- 🛡️ Improved record organization
- ⚡ Faster access to important operational information

---

<div align="center">

# 🚆 INDIAN RAILWAYS — TRAIN MANAGEMENT SYSTEM

### *Engineered for Operational Safety, Digital Record Management, and Centralized Reporting.*

<br/>

**41 Operational Registers • Centralized SQL Server • Dynamic Reports • PDF • Excel • CSV**

<br/>

© 2026 All Rights Reserved

</div>