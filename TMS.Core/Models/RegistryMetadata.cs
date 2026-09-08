using System;
using System.Collections.Generic;

namespace TMS.Core.Models
{
    public class RegisterDefinition
    {
        public string Code { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogIdPrefix { get; set; } = string.Empty;
        public List<RegisterField> Fields { get; set; } = new List<RegisterField>();
    }

    public class RegisterField
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = "text"; // text, textarea, number, date, datetime, select, bool
        public bool Required { get; set; } = true;
        public string[] Options { get; set; } = Array.Empty<string>();
        public string DefaultValue { get; set; } = string.Empty;
    }

    public static class RegistryMetadata
    {
        public static readonly List<RegisterDefinition> AllRegisters = new List<RegisterDefinition>
        {
            new RegisterDefinition
            {
                Code = "REG-001",
                Number = "001",
                Title = "Station Master's Diary",
                Category = "Operational",
                TableName = "Reg001_StationDiary",
                LogIdPrefix = "TMS-REG-001",
                Description = "Daily record of operational and administrative station events.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "EventTime", Label = "Event Time", Type = "datetime", Required = true },
                    new RegisterField { Name = "Category", Label = "Category", Type = "select", Required = true, Options = new[] { "Administrative", "Operational", "Commercial", "Safety", "Maintenance", "Emergency", "Others" } },
                    new RegisterField { Name = "Description", Label = "Description", Type = "textarea", Required = true },
                    new RegisterField { Name = "ReportedBy", Label = "Reported By", Type = "text", Required = true },
                    new RegisterField { Name = "Remarks", Label = "Remarks", Type = "text", Required = false },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID (Submitted By)", Type = "number", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "REG-002",
                Number = "002",
                Title = "Train Signal Register (TSR)",
                Category = "Operational",
                TableName = "Reg002_TrainSignal",
                LogIdPrefix = "TMS-REG-002",
                Description = "Log of all train arrivals, departures, line numbers, and passing private numbers.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "TrainNumber", Label = "Train Number", Type = "text", Required = true },
                    new RegisterField { Name = "TrainDescription", Label = "Train Name / Description", Type = "text", Required = false },
                    new RegisterField { Name = "LineNumber", Label = "Line Number", Type = "text", Required = true },
                    new RegisterField { Name = "Direction", Label = "Direction", Type = "select", Required = true, Options = new[] { "UP", "DOWN" } },
                    new RegisterField { Name = "DepartureStation", Label = "From Station", Type = "text", Required = true },
                    new RegisterField { Name = "DestinationStation", Label = "To Station", Type = "text", Required = true },
                    new RegisterField { Name = "ArrivalTime", Label = "Arrival Time", Type = "datetime", Required = false },
                    new RegisterField { Name = "DepartureTime", Label = "Departure Time", Type = "datetime", Required = false },
                    new RegisterField { Name = "PrivateNumberGiven", Label = "Private Number Given", Type = "text", Required = false },
                    new RegisterField { Name = "PrivateNumberReceived", Label = "Private Number Received", Type = "text", Required = false },
                    new RegisterField { Name = "Remarks", Label = "Remarks", Type = "text", Required = false },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID", Type = "number", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "REG-003",
                Number = "003",
                Title = "Station Working Rules (SWR)",
                Category = "Operational",
                TableName = "Reg003_SWR",
                LogIdPrefix = "TMS-REG-003",
                Description = "Declaration and compliance register for station working rules.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "StaffName", Label = "Staff Name", Type = "text", Required = true },
                    new RegisterField { Name = "Designation", Label = "Designation", Type = "text", Required = true },
                    new RegisterField { Name = "RuleSection", Label = "SWR Rule Section", Type = "text", Required = true },
                    new RegisterField { Name = "AssuranceGiven", Label = "Assurance Verified", Type = "select", Required = true, Options = new[] { "Yes", "No" } },
                    new RegisterField { Name = "DeclarationDate", Label = "Declaration Date", Type = "date", Required = true },
                    new RegisterField { Name = "Remarks", Label = "Remarks", Type = "text", Required = false },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID", Type = "number", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "REG-004",
                Number = "004",
                Title = "Caution Order Register",
                Category = "Safety",
                TableName = "Reg004_CautionOrder",
                LogIdPrefix = "TMS-REG-004",
                Description = "Speed restriction and track warning notices for train crews.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "NoticeNo", Label = "Caution Notice Number", Type = "text", Required = true },
                    new RegisterField { Name = "IssueTime", Label = "Issue Time", Type = "datetime", Required = true },
                    new RegisterField { Name = "SectionFrom", Label = "Section From (Station / KM)", Type = "text", Required = true },
                    new RegisterField { Name = "SectionTo", Label = "Section To (Station / KM)", Type = "text", Required = true },
                    new RegisterField { Name = "SpeedRestriction", Label = "Speed Restriction (KMPH)", Type = "text", Required = true },
                    new RegisterField { Name = "Reason", Label = "Reason for Restriction", Type = "textarea", Required = true },
                    new RegisterField { Name = "ValidTill", Label = "Valid Till", Type = "datetime", Required = false },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID", Type = "number", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "REG-005",
                Number = "005",
                Title = "Gear Failure Register",
                Category = "Maintenance",
                TableName = "Reg005_Failure",
                LogIdPrefix = "TMS-REG-005",
                Description = "Failures of signaling, point machines, track circuits, and block instruments.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "GearType", Label = "Gear / Equipment Type", Type = "select", Required = true, Options = new[] { "Signal", "Point Machine", "Track Circuit", "Axle Counter", "Block Instrument", "Interlocking", "OHE / Power", "Other" } },
                    new RegisterField { Name = "GearID", Label = "Gear ID / Point No / Signal No", Type = "text", Required = true },
                    new RegisterField { Name = "FailureTime", Label = "Failure Time", Type = "datetime", Required = true },
                    new RegisterField { Name = "NatureOfFailure", Label = "Nature of Failure", Type = "textarea", Required = true },
                    new RegisterField { Name = "ReportedTo", Label = "Reported To (Designation / Department)", Type = "text", Required = true },
                    new RegisterField { Name = "RectificationTime", Label = "Rectification Time", Type = "datetime", Required = false },
                    new RegisterField { Name = "RectifiedBy", Label = "Rectified By", Type = "text", Required = false },
                    new RegisterField { Name = "DurationMinutes", Label = "Total Downtime (Minutes)", Type = "number", Required = false },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID", Type = "number", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "REG-006",
                Number = "006",
                Title = "Disconnection & Reconnection Register",
                Category = "Maintenance",
                TableName = "Reg006_DisconRecon",
                LogIdPrefix = "TMS-REG-006",
                Description = "Formal S&T memo records for disconnecting and reconnecting interlocking equipment.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "MemoNumber", Label = "Memo Number", Type = "text", Required = true },
                    new RegisterField { Name = "Department", Label = "Department Requesting", Type = "select", Required = true, Options = new[] { "S&T", "Engineering (P-Way)", "Electrical (TRD)", "Operating" } },
                    new RegisterField { Name = "EquipmentDetails", Label = "Equipment / Gears Disconnected", Type = "textarea", Required = true },
                    new RegisterField { Name = "DisconnectionTime", Label = "Disconnection Time", Type = "datetime", Required = true },
                    new RegisterField { Name = "ReconnectionTime", Label = "Reconnection Time", Type = "datetime", Required = false },
                    new RegisterField { Name = "StaffSignName", Label = "Engineer / ESM In-charge", Type = "text", Required = true },
                    new RegisterField { Name = "StationMasterName", Label = "Station Master On Duty", Type = "text", Required = true },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID", Type = "number", Required = true }
                }
            }
        };

        public static readonly List<RegisterDefinition> AllAuthorities = new List<RegisterDefinition>
        {
            new RegisterDefinition
            {
                Code = "AUTH-001",
                Number = "001",
                Title = "Advance Defective Signal Authority",
                Category = "Signaling Authority",
                TableName = "Auth001_AdvanceDefectiveSignal",
                LogIdPrefix = "TW-AUTH-001",
                Description = "T/369(3b) Authority to pass advance starter signal in defective position.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "TrainNumber", Label = "Train Number", Type = "text", Required = true },
                    new RegisterField { Name = "LocoNumber", Label = "Loco Engine Number", Type = "text", Required = true },
                    new RegisterField { Name = "DriverName", Label = "Loco Pilot Name", Type = "text", Required = true },
                    new RegisterField { Name = "DefectiveSignalNo", Label = "Defective Signal No", Type = "text", Required = true },
                    new RegisterField { Name = "PrivateNumber", Label = "Line Clear PN Received", Type = "text", Required = true },
                    new RegisterField { Name = "SpeedLimit", Label = "Speed Limit (KMPH)", Type = "text", Required = true, DefaultValue = "15 KMPH" },
                    new RegisterField { Name = "IssueTime", Label = "Time of Issue", Type = "datetime", Required = true },
                    new RegisterField { Name = "StationMaster", Label = "Station Master", Type = "text", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "AUTH-002",
                Number = "002",
                Title = "Authority to Pass Signals in 'ON' Position",
                Category = "Signaling Authority",
                TableName = "Auth002_PassSignalON",
                LogIdPrefix = "TW-AUTH-002",
                Description = "T/369(3b) Authority to pass Home, Starter, or Intermediate Stop Signals at Danger.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "TrainNumber", Label = "Train Number", Type = "text", Required = true },
                    new RegisterField { Name = "LocoNumber", Label = "Loco Engine Number", Type = "text", Required = true },
                    new RegisterField { Name = "DriverName", Label = "Loco Pilot Name", Type = "text", Required = true },
                    new RegisterField { Name = "SignalDescription", Label = "Signal Name / Number (e.g. S-4 Home)", Type = "text", Required = true },
                    new RegisterField { Name = "PrivateNumber", Label = "Private Number", Type = "text", Required = true },
                    new RegisterField { Name = "Instructions", Label = "Special Caution Instructions", Type = "textarea", Required = false },
                    new RegisterField { Name = "IssueTime", Label = "Time of Issue", Type = "datetime", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "AUTH-003",
                Number = "003",
                Title = "Caution Order (T/409)",
                Category = "Track Safety",
                TableName = "Auth003_CautionOrder",
                LogIdPrefix = "TW-AUTH-003",
                Description = "Official speed restriction caution notice issued to train loco pilots.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "TrainNumber", Label = "Train Number", Type = "text", Required = true },
                    new RegisterField { Name = "FromStation", Label = "Section From", Type = "text", Required = true },
                    new RegisterField { Name = "ToStation", Label = "Section To", Type = "text", Required = true },
                    new RegisterField { Name = "KilometerRange", Label = "Kilometer Range (KM/TP)", Type = "text", Required = true },
                    new RegisterField { Name = "SpeedRestriction", Label = "Speed Restriction", Type = "text", Required = true },
                    new RegisterField { Name = "Reason", Label = "Cause / Reason", Type = "textarea", Required = true },
                    new RegisterField { Name = "IssueTime", Label = "Issue Time", Type = "datetime", Required = true }
                }
            },
            new RegisterDefinition
            {
                Code = "AUTH-018",
                Number = "018",
                Title = "Paper Line Clear Ticket (UP / DOWN)",
                Category = "Block Working",
                TableName = "Auth018_LineClearTickets",
                LogIdPrefix = "TW-AUTH-018",
                Description = "T/C.1425 (UP) or T/D.1425 (DOWN) Paper Line Clear Ticket for total block failure.",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "Direction", Label = "Direction", Type = "select", Required = true, Options = new[] { "UP (T/C 1425)", "DOWN (T/D 1425)" } },
                    new RegisterField { Name = "TrainNumber", Label = "Train Number", Type = "text", Required = true },
                    new RegisterField { Name = "LocoNumber", Label = "Engine / Loco Number", Type = "text", Required = true },
                    new RegisterField { Name = "FromStation", Label = "Station From", Type = "text", Required = true },
                    new RegisterField { Name = "ToStation", Label = "Station To", Type = "text", Required = true },
                    new RegisterField { Name = "PrivateNumber", Label = "Line Clear Private Number", Type = "text", Required = true },
                    new RegisterField { Name = "IssueTime", Label = "Departure / Issue Time", Type = "datetime", Required = true },
                    new RegisterField { Name = "StationMasterName", Label = "Station Master Signature / Name", Type = "text", Required = true }
                }
            }
        };

        public static RegisterDefinition? GetRegisterByNumber(string number)
        {
            var cleanNum = number.PadLeft(3, '0');
            var found = AllRegisters.Find(r => r.Number == cleanNum || r.Code.EndsWith(cleanNum));
            if (found != null) return found;

            // Generate dynamic placeholder definition for any of the 41 registers if not statically configured
            return new RegisterDefinition
            {
                Code = $"REG-{cleanNum}",
                Number = cleanNum,
                Title = $"Station Register {cleanNum}",
                Category = "General Register",
                TableName = $"Reg{cleanNum}_Register",
                LogIdPrefix = $"TMS-REG-{cleanNum}",
                Description = $"Official Station Register #{cleanNum}",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "EventTime", Label = "Event Time", Type = "datetime", Required = true },
                    new RegisterField { Name = "TitleOrSubject", Label = "Subject / Reference", Type = "text", Required = true },
                    new RegisterField { Name = "Details", Label = "Details / Observations", Type = "textarea", Required = true },
                    new RegisterField { Name = "ActionTaken", Label = "Action Taken / Remarks", Type = "text", Required = false },
                    new RegisterField { Name = "SubmittedBy", Label = "Staff ID", Type = "number", Required = true }
                }
            };
        }

        public static RegisterDefinition? GetAuthorityByNumber(string number)
        {
            var cleanNum = number.PadLeft(3, '0');
            var found = AllAuthorities.Find(a => a.Number == cleanNum || a.Code.EndsWith(cleanNum));
            if (found != null) return found;

            return new RegisterDefinition
            {
                Code = $"AUTH-{cleanNum}",
                Number = cleanNum,
                Title = $"Train Working Authority {cleanNum}",
                Category = "Authority Form",
                TableName = $"Auth{cleanNum}_Authority",
                LogIdPrefix = $"TW-AUTH-{cleanNum}",
                Description = $"Official Train Working Authority Form #{cleanNum}",
                Fields = new List<RegisterField>
                {
                    new RegisterField { Name = "TrainNumber", Label = "Train Number", Type = "text", Required = true },
                    new RegisterField { Name = "LocoNumber", Label = "Loco / Engine Number", Type = "text", Required = true },
                    new RegisterField { Name = "DriverName", Label = "Loco Pilot Name", Type = "text", Required = true },
                    new RegisterField { Name = "PrivateNumber", Label = "Private Number", Type = "text", Required = false },
                    new RegisterField { Name = "IssueTime", Label = "Time of Issue", Type = "datetime", Required = true },
                    new RegisterField { Name = "StationMaster", Label = "Station Master", Type = "text", Required = true }
                }
            };
        }
    }
}
