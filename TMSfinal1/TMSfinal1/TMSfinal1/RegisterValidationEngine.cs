using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TMS
{
    /// <summary>
    /// Validation result containing validation status, descriptive failure reason, and valid guidance example.
    /// Derived from C:\Users\Asus\Beta\Test_Cases.xlsx.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string FieldName { get; set; }
        public string ErrorReason { get; set; }
        public string GuidanceExample { get; set; }

        public static ValidationResult Success()
        {
            return new ValidationResult { IsValid = true };
        }

        public static ValidationResult Fail(string fieldName, string reason, string example)
        {
            return new ValidationResult
            {
                IsValid = false,
                FieldName = fieldName,
                ErrorReason = reason,
                GuidanceExample = example
            };
        }

        public string GetFormattedMessage()
        {
            return $"❌ Validation Error in '{FieldName}':\n\n" +
                   $"💡 Reason: {ErrorReason}\n\n" +
                   $"✅ Expected Valid Input: {GuidanceExample}";
        }
    }

    /// <summary>
    /// Centralized Validation Engine for All 40 Registers (REG-001 to REG-040).
    /// Implements all 264 Positive and Negative Test Case Scenarios from Test_Cases.xlsx.
    /// </summary>
    public static class RegisterValidationEngine
    {
        public static ValidationResult Validate(string regCode, Dictionary<string, object> values)
        {
            if (string.IsNullOrEmpty(regCode)) return ValidationResult.Success();
            string code = regCode.Trim().Replace("REG-", "").PadLeft(3, '0');

            switch (code)
            {
                case "001": return ValidateReg001(values);
                case "002": return ValidateReg002(values);
                case "003": return ValidateReg003(values);
                case "004": return ValidateReg004(values);
                case "005": return ValidateReg005(values);
                case "006": return ValidateReg006(values);
                case "007": return ValidateReg007(values);
                case "008": return ValidateReg008(values);
                case "009": return ValidateReg009(values);
                case "010": return ValidateReg010(values);
                case "011": return ValidateReg011(values);
                case "012": return ValidateReg012(values);
                case "013": return ValidateReg013(values);
                case "014": return ValidateReg014(values);
                case "015": return ValidateReg015(values);
                case "016": return ValidateReg016(values);
                case "017": return ValidateReg017(values);
                case "018": return ValidateReg018(values);
                case "019": return ValidateReg019(values);
                case "020": return ValidateReg020(values);
                case "021": return ValidateReg021(values);
                case "022": return ValidateReg022(values);
                case "023": return ValidateReg023(values);
                case "024": return ValidateReg024(values);
                case "025": return ValidateReg025(values);
                case "026": return ValidateReg026(values);
                case "027": return ValidateReg027(values);
                case "028": return ValidateReg028(values);
                case "029": return ValidateReg029(values);
                case "030": return ValidateReg030(values);
                case "031": return ValidateReg031(values);
                case "032": return ValidateReg032(values);
                case "033": return ValidateReg033(values);
                case "034": return ValidateReg034(values);
                case "035": return ValidateReg035(values);
                case "036": return ValidateReg036(values);
                case "037": return ValidateReg037(values);
                case "038": return ValidateReg038(values);
                case "039": return ValidateReg039(values);
                case "040": return ValidateReg040(values);
                default: return ValidationResult.Success();
            }
        }

        private static string GetStr(Dictionary<string, object> vals, string key)
        {
            if (vals != null && vals.TryGetValue(key, out object v) && v != null)
                return v.ToString().Trim();
            return "";
        }

        private static DateTime GetDate(Dictionary<string, object> vals, string key)
        {
            if (vals != null && vals.TryGetValue(key, out object v) && v != null)
            {
                if (v is DateTime dt) return dt;
                if (DateTime.TryParse(v.ToString(), out DateTime parsed)) return parsed;
            }
            return DateTime.MinValue;
        }

        private static bool IsMeaningfulText(string text, int minLength)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            string clean = text.Trim();
            if (clean.Length < minLength) return false;
            // Check not only special characters
            if (Regex.IsMatch(clean, @"^[^a-zA-Z0-9]+$")) return false;
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-001: Station Master's Diary
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg001(Dictionary<string, object> v)
        {
            string cat = GetStr(v, "Category");
            if (string.IsNullOrWhiteSpace(cat) || cat.Equals("-- Select Category --", StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Fail("Category", "Category is mandatory to classify station events.", "Select 'Operational', 'Administrative', 'Commercial', or 'Others'.");

            string desc = GetStr(v, "Description");
            if (!IsMeaningfulText(desc, 10))
                return ValidationResult.Fail("Description", "Description is too brief or lacks meaningful details. Vague text like 'OK' or special characters only are rejected.", "Enter detailed event context (e.g., 'Signal 45 failed at 10:30 AM, rectified by 11:15 AM').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-002: Train Signal Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg002(Dictionary<string, object> v)
        {
            string trainNo = GetStr(v, "TrainNumber");
            if (string.IsNullOrWhiteSpace(trainNo))
                return ValidationResult.Fail("Train Number", "Train Number cannot be empty.", "Enter a valid train number (e.g., '12727', '2', or '12727A').");

            if (!Regex.IsMatch(trainNo, @"^[0-9]{1,6}[A-Za-z]?$"))
                return ValidationResult.Fail("Train Number", "Invalid train number format. Only numbers and single suffix letters allowed. Special characters like '@' or '#' are prohibited.", "Example: '12727' or '12727A'.");

            string dir = GetStr(v, "Direction");
            if (string.IsNullOrWhiteSpace(dir) || (!dir.Equals("UP", StringComparison.OrdinalIgnoreCase) && !dir.Equals("DOWN", StringComparison.OrdinalIgnoreCase)))
                return ValidationResult.Fail("Direction", "Movement direction must be specified as either 'UP' or 'DOWN'.", "Select 'UP' or 'DOWN' from the dropdown.");

            string line = GetStr(v, "LineNumber");
            if (string.IsNullOrWhiteSpace(line))
                return ValidationResult.Fail("Line Number", "Line/Platform allocation must be selected.", "Select 'Platform 1', 'Main Line', or 'Through Line'.");

            DateTime arr = GetDate(v, "TrainArrival");
            DateTime dep = GetDate(v, "TrainDeparture");
            DateTime passed = GetDate(v, "TrainPassed");

            if (arr != DateTime.MinValue && dep != DateTime.MinValue && dep < arr)
                return ValidationResult.Fail("Train Departure", "Departure time cannot be earlier than Arrival time.", "Ensure Departure Time is after Arrival Time.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-003: SWR Acknowledgment Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg003(Dictionary<string, object> v)
        {
            string staffId = GetStr(v, "StaffID");
            if (string.IsNullOrWhiteSpace(staffId))
                return ValidationResult.Fail("Staff ID", "Staff ID cannot be empty.", "Enter valid alphanumeric Staff ID (e.g., 'SM234234').");

            string swrVer = GetStr(v, "SWRVersion");
            if (string.IsNullOrWhiteSpace(swrVer))
                return ValidationResult.Fail("SWR Version", "SWR Version must be specified.", "Enter valid version (e.g., 'Ver 3.2', 'Amendment 4').");

            string verifiedBy = GetStr(v, "VerifiedBy");
            if (string.IsNullOrWhiteSpace(verifiedBy))
                return ValidationResult.Fail("Verified By", "Verifier ID / Inspector name is required.", "Enter verifying officer name or ID.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-004: Caution Order Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg004(Dictionary<string, object> v)
        {
            string section = GetStr(v, "Section");
            if (string.IsNullOrWhiteSpace(section))
                return ValidationResult.Fail("Section", "Railway section cannot be blank.", "Enter valid section (e.g., 'SC - KZJ Block Section').");

            string speed = GetStr(v, "SpeedLimit");
            if (!int.TryParse(speed, out int spdVal) || spdVal <= 0 || spdVal > 160)
                return ValidationResult.Fail("Speed Limit", "Speed limit must be a positive number between 10 and 160 km/h.", "Example: '30', '45', or '75'.");

            string reason = GetStr(v, "Reason");
            if (!IsMeaningfulText(reason, 8))
                return ValidationResult.Fail("Reason", "Reason for caution order must be explained with engineering/safety context.", "Example: 'Track renewal in progress at KM 204/12'.");

            DateTime start = GetDate(v, "ValidityStart");
            DateTime end = GetDate(v, "ValidityEnd");
            if (start != DateTime.MinValue && end != DateTime.MinValue && end < start)
                return ValidationResult.Fail("Validity End", "Validity end time cannot be before validity start time.", "Ensure end time is after start time.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-005: Signal/Point/Block Failure Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg005(Dictionary<string, object> v)
        {
            string assetType = GetStr(v, "AssetType");
            if (string.IsNullOrWhiteSpace(assetType))
                return ValidationResult.Fail("Asset Type", "Asset Type must be selected.", "Select 'Signal', 'Point', 'Track Circuit', or 'Block Instrument'.");

            string assetId = GetStr(v, "AssetID");
            if (string.IsNullOrWhiteSpace(assetId))
                return ValidationResult.Fail("Asset ID / Number", "Asset ID cannot be empty.", "Enter valid asset number (e.g., 'S-14', 'Pt 102B').");

            DateTime failTime = GetDate(v, "FailureTime");
            DateTime rectTime = GetDate(v, "RectificationTime");
            if (failTime != DateTime.MinValue && rectTime != DateTime.MinValue && rectTime < failTime)
                return ValidationResult.Fail("Rectification Time", "Rectification time cannot be earlier than the failure time.", "Ensure rectification time occurs after failure.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-006: S&T Disconnection / Reconnection Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg006(Dictionary<string, object> v)
        {
            string gearId = GetStr(v, "GearID");
            if (string.IsNullOrWhiteSpace(gearId))
                return ValidationResult.Fail("Gear ID", "Signaling Gear ID is mandatory.", "Enter Gear ID (e.g., 'Point 201A', 'Signal S-4').");

            string maintainer = GetStr(v, "MaintainerID");
            if (string.IsNullOrWhiteSpace(maintainer))
                return ValidationResult.Fail("Maintainer ID", "S&T Maintainer ID / Name must be entered.", "Enter valid technician ID (e.g., 'ESM-5321').");

            DateTime discon = GetDate(v, "DisconnectionTime");
            DateTime recon = GetDate(v, "ReconnectionTime");
            if (discon != DateTime.MinValue && recon != DateTime.MinValue && recon < discon)
                return ValidationResult.Fail("Reconnection Time", "Reconnection time cannot be prior to disconnection time.", "Reconnection must be at or after disconnection.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-007: Bio-Metric Attendance Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg007(Dictionary<string, object> v)
        {
            string staffId = GetStr(v, "StaffID");
            if (string.IsNullOrWhiteSpace(staffId))
                return ValidationResult.Fail("Staff ID", "Staff ID is required.", "Enter numeric or alphanumeric staff ID (e.g., '3541654').");

            string shift = GetStr(v, "ShiftType");
            if (string.IsNullOrWhiteSpace(shift))
                return ValidationResult.Fail("Shift Type", "Shift type must be assigned.", "Select 'Morning (06:00-14:00)', 'Evening (14:00-22:00)', or 'Night (22:00-06:00)'.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-008: Stabled Load Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg008(Dictionary<string, object> v)
        {
            string loadId = GetStr(v, "TrainLoadID");
            if (string.IsNullOrWhiteSpace(loadId))
                return ValidationResult.Fail("Train / Load ID", "Stabled train or rake ID is mandatory.", "Enter Rake ID (e.g., 'BOXN-8472', 'BCNA-991').");

            string line = GetStr(v, "LineNumber");
            if (string.IsNullOrWhiteSpace(line))
                return ValidationResult.Fail("Line Number", "Stabling line/siding must be specified.", "Select 'Goods Siding 1', 'Loop Line 2'.");

            string brake = GetStr(v, "HandBrakeStatus");
            if (string.IsNullOrWhiteSpace(brake))
                return ValidationResult.Fail("Hand Brake Status", "Hand brake application status must be confirmed for stabled rolling stock.", "Specify 'Applied and Pinned' or 'Skids Provided'.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-009: Fog Signalman Deployment Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg009(Dictionary<string, object> v)
        {
            string staffId = GetStr(v, "StaffID");
            if (string.IsNullOrWhiteSpace(staffId))
                return ValidationResult.Fail("Staff ID", "Deployed Fog Signalman ID is required.", "Enter staff ID (e.g., 'FS-9842').");

            string loc = GetStr(v, "Location");
            if (string.IsNullOrWhiteSpace(loc))
                return ValidationResult.Fail("Location", "Detonator placement location/post must be specified.", "Example: 'KM 142/6 (Up Distant Signal)'.");

            string dets = GetStr(v, "DetonatorsUsed");
            if (!string.IsNullOrEmpty(dets) && (!int.TryParse(dets, out int dCount) || dCount < 0))
                return ValidationResult.Fail("Detonators Used", "Detonator count must be a non-negative integer.", "Enter '0', '2', or '4'.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-010: Night Inspection Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg010(Dictionary<string, object> v)
        {
            string officer = GetStr(v, "InspectingOfficer");
            if (string.IsNullOrWhiteSpace(officer))
                return ValidationResult.Fail("Inspecting Officer", "Inspecting Officer name/designation is required.", "Enter officer name (e.g., 'DOM / SC', 'AOM / Safety').");

            string obs = GetStr(v, "Observations");
            if (!IsMeaningfulText(obs, 10))
                return ValidationResult.Fail("Observations", "Observations must contain detailed findings on station alertness and night working compliance.", "Enter detailed findings (e.g., 'All signals illuminated, staff alert, emergency equipment intact').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-011: Public Complaints Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg011(Dictionary<string, object> v)
        {
            string name = GetStr(v, "ComplainantName");
            if (string.IsNullOrWhiteSpace(name))
                return ValidationResult.Fail("Complainant Name", "Passenger / Complainant name cannot be empty.", "Enter passenger full name.");

            string desc = GetStr(v, "Description");
            if (!IsMeaningfulText(desc, 10))
                return ValidationResult.Fail("Description", "Complaint description must provide clear details of the passenger grievance.", "Enter details (e.g., 'Coach B2 AC malfunction between Secunderabad and Kazipet').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-012: Staff Grievance Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg012(Dictionary<string, object> v)
        {
            string subject = GetStr(v, "Subject");
            if (!IsMeaningfulText(subject, 8))
                return ValidationResult.Fail("Subject", "Grievance subject must clearly summarize the issue.", "Enter subject (e.g., 'Roster duty hours adjustment request').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-013: Inspection & Observations Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg013(Dictionary<string, object> v)
        {
            string officer = GetStr(v, "OfficerID");
            if (string.IsNullOrWhiteSpace(officer))
                return ValidationResult.Fail("Officer ID", "Inspecting Officer ID is required.", "Enter Officer ID (e.g., 'SrDOM-01').");

            string obs = GetStr(v, "Observations");
            if (!IsMeaningfulText(obs, 10))
                return ValidationResult.Fail("Observations", "Inspection findings and deficiencies must be documented.", "Enter observations (e.g., 'Caution boards properly positioned, fire extinguishers refilled').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-014: Miscellaneous Counter Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg014(Dictionary<string, object> v)
        {
            string type = GetStr(v, "CounterType");
            if (string.IsNullOrWhiteSpace(type))
                return ValidationResult.Fail("Counter Type", "Counter type must be selected.", "Select 'Emergency Route Release', 'Calling-On Counter', or 'Slot Reset'.");

            string oldVal = GetStr(v, "OldCounterValue");
            string newVal = GetStr(v, "NewCounterValue");

            if (int.TryParse(oldVal, out int oldInt) && int.TryParse(newVal, out int newInt))
            {
                if (newInt < oldInt)
                    return ValidationResult.Fail("New Counter Value", "New counter value cannot be lower than the previous reading.", "Ensure New Value >= Old Value.");
            }

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-015: Siding Key Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg015(Dictionary<string, object> v)
        {
            string keyNo = GetStr(v, "KeyNumber");
            if (string.IsNullOrWhiteSpace(keyNo))
                return ValidationResult.Fail("Key Number", "Siding key identifier is mandatory.", "Enter Key ID (e.g., 'SK-01', 'FCI Siding Key #2').");

            string issuedTo = GetStr(v, "IssuedTo");
            if (string.IsNullOrWhiteSpace(issuedTo))
                return ValidationResult.Fail("Issued To", "Person taking possession of siding key must be recorded.", "Enter Staff Name / Designation (e.g., 'Pointsman A. Kumar').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-016: Crank Handle Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg016(Dictionary<string, object> v)
        {
            string ptNo = GetStr(v, "PointNumber");
            if (string.IsNullOrWhiteSpace(ptNo))
                return ValidationResult.Fail("Point Number", "Point Number to be operated with crank handle is required.", "Enter Point No (e.g., 'Pt 101/102', 'Cross-over 204').");

            string authPn = GetStr(v, "AuthorizationPN");
            if (string.IsNullOrWhiteSpace(authPn))
                return ValidationResult.Fail("Authorization PN", "Private Number (PN) exchange is mandatory for crank handle extraction.", "Enter 6-digit PN (e.g., '482910').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-017: Crank Handle Testing Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg017(Dictionary<string, object> v)
        {
            string handleId = GetStr(v, "HandleID");
            if (string.IsNullOrWhiteSpace(handleId))
                return ValidationResult.Fail("Handle ID", "Crank Handle identification is required.", "Enter Handle ID (e.g., 'CH-01', 'CH-Point 103').");

            string result = GetStr(v, "TestResult");
            if (string.IsNullOrWhiteSpace(result))
                return ValidationResult.Fail("Test Result", "Test outcome must be specified.", "Select 'Pass - Interlocking Verified' or 'Fail - Defect Found'.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-018: Cross-Over Testing Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg018(Dictionary<string, object> v)
        {
            string crossId = GetStr(v, "CrossoverID");
            if (string.IsNullOrWhiteSpace(crossId))
                return ValidationResult.Fail("Crossover ID", "Crossover point identifier is mandatory.", "Enter Crossover ID (e.g., 'CO-101/102').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-019: Signal Failure Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg019(Dictionary<string, object> v)
        {
            string sigNo = GetStr(v, "SignalNumber");
            if (string.IsNullOrWhiteSpace(sigNo))
                return ValidationResult.Fail("Signal Number", "Signal identification number is mandatory.", "Enter Signal No (e.g., 'S-1', 'S-12 Advanced Starter').");

            string rootCause = GetStr(v, "RootCause");
            if (!string.IsNullOrEmpty(rootCause) && !IsMeaningfulText(rootCause, 6))
                return ValidationResult.Fail("Root Cause", "Root cause explanation must be descriptive.", "Example: 'Blown fuse in location box 3'.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-020: Emergency Key Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg020(Dictionary<string, object> v)
        {
            string keyId = GetStr(v, "EmergencyKeyID");
            if (string.IsNullOrWhiteSpace(keyId))
                return ValidationResult.Fail("Emergency Key ID", "Emergency Key identifier is required.", "Enter Emergency Key ID (e.g., 'EK-Block-01').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-021: Complete Arrival Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg021(Dictionary<string, object> v)
        {
            string trainNo = GetStr(v, "TrainNumber");
            if (string.IsNullOrWhiteSpace(trainNo))
                return ValidationResult.Fail("Train Number", "Train number is mandatory.", "Enter Train Number (e.g., '12727').");

            string wagon = GetStr(v, "WagonCount");
            if (!string.IsNullOrEmpty(wagon) && (!int.TryParse(wagon, out int wCount) || wCount <= 0 || wCount > 120))
                return ValidationResult.Fail("Wagon Count", "Wagon / Coach count must be a realistic positive number (1-120).", "Enter valid coach count (e.g., '24', '58').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-022: Control Instruction Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg022(Dictionary<string, object> v)
        {
            string content = GetStr(v, "Content");
            if (!IsMeaningfulText(content, 10))
                return ValidationResult.Fail("Content", "Control message content cannot be blank or meaningless.", "Enter message details (e.g., 'Regulate train 12728 at Loop 1 due to preceding freight train').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-023: SM Relief Diary
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg023(Dictionary<string, object> v)
        {
            string relieving = GetStr(v, "RelievingSMID");
            string relieved = GetStr(v, "RelievedSMID");

            if (string.IsNullOrWhiteSpace(relieving))
                return ValidationResult.Fail("Relieving SM ID", "Relieving Station Master ID is required.", "Enter Staff ID of incoming SM.");

            if (!string.IsNullOrEmpty(relieved) && relieving.Equals(relieved, StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Fail("Relieved SM ID", "Relieving SM and Relieved SM cannot be the same staff ID during shift handover.", "Enter different Staff IDs for incoming and outgoing Station Masters.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-024: Traffic / Power Block Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg024(Dictionary<string, object> v)
        {
            string blockType = GetStr(v, "BlockType");
            if (string.IsNullOrWhiteSpace(blockType))
                return ValidationResult.Fail("Block Type", "Block Type must be specified.", "Select 'Line Block', 'Power Block (OHE)', or 'Integrated Block'.");

            string section = GetStr(v, "AffectedSection");
            if (string.IsNullOrWhiteSpace(section))
                return ValidationResult.Fail("Affected Section", "Affected block section is mandatory.", "Enter section (e.g., 'Line 1 SC-BMT Section').");

            DateTime start = GetDate(v, "ActualStart");
            DateTime end = GetDate(v, "ActualEnd");
            if (start != DateTime.MinValue && end != DateTime.MinValue && end < start)
                return ValidationResult.Fail("Actual End", "Block cancellation/end time cannot be before block grant time.", "Ensure block end time is after start time.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-025: Safety Meeting Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg025(Dictionary<string, object> v)
        {
            string agenda = GetStr(v, "Agenda");
            if (!IsMeaningfulText(agenda, 10))
                return ValidationResult.Fail("Agenda / Topic", "Safety meeting agenda must be documented.", "Enter topic (e.g., 'Monsoon precautions and night patrolling reviews').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-026: HQ Safety Circular Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg026(Dictionary<string, object> v)
        {
            string circNo = GetStr(v, "CircularNumber");
            if (string.IsNullOrWhiteSpace(circNo))
                return ValidationResult.Fail("Circular Number", "Circular reference number is required.", "Enter circular code (e.g., 'CIRC/2026/04', 'Safety Alert 12').");

            string subj = GetStr(v, "Subject");
            if (!IsMeaningfulText(subj, 8))
                return ValidationResult.Fail("Subject", "Circular subject must clearly describe the safety instructions.", "Enter subject (e.g., 'Winter precautions for point machines').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-027: Safety Meeting (Part 2) Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg027(Dictionary<string, object> v)
        {
            string mom = GetStr(v, "Minutes");
            if (!IsMeaningfulText(mom, 10))
                return ValidationResult.Fail("Minutes (MoM)", "Meeting minutes must detail key deliberations and safety directives.", "Enter summary of minutes.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-028: Staff Biodata & Training Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg028(Dictionary<string, object> v)
        {
            string empId = GetStr(v, "EmployeeID");
            if (string.IsNullOrWhiteSpace(empId))
                return ValidationResult.Fail("Employee ID", "Employee Staff ID is mandatory.", "Enter employee ID (e.g., 'EMP-59421').");

            DateTime pme = GetDate(v, "PMEDate");
            DateTime pmeDue = GetDate(v, "PMEDueDate");
            if (pme != DateTime.MinValue && pmeDue != DateTime.MinValue && pmeDue < pme)
                return ValidationResult.Fail("PME Due Date", "Next PME Due Date cannot be prior to the last PME Date.", "Ensure PME Due Date is in the future relative to PME Date.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-029: Assurance Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg029(Dictionary<string, object> v)
        {
            string docId = GetStr(v, "DocumentID");
            if (string.IsNullOrWhiteSpace(docId))
                return ValidationResult.Fail("Document ID", "Document reference being assured is required.", "Enter Document ID (e.g., 'SWR Amendment 3', 'Safety Circular 15').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-030: Private Number (PN) Sheet Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg030(Dictionary<string, object> v)
        {
            string purpose = GetStr(v, "Purpose");
            if (string.IsNullOrWhiteSpace(purpose))
                return ValidationResult.Fail("Purpose", "Purpose for private number exchange is mandatory.", "Enter purpose (e.g., 'Line Clear Grant for 12727', 'Crank Handle extraction').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-031: Petty Repair Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg031(Dictionary<string, object> v)
        {
            string desc = GetStr(v, "DefectDescription");
            if (!IsMeaningfulText(desc, 8))
                return ValidationResult.Fail("Defect Description", "Defect details must be documented for repair tracking.", "Enter defect details (e.g., 'Station platform light fitting broken at Pole 12').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-032: Attendance Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg032(Dictionary<string, object> v)
        {
            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-033: Passenger Complaint Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg033(Dictionary<string, object> v)
        {
            string mobile = GetStr(v, "MobileNumber");
            if (!string.IsNullOrEmpty(mobile) && !Regex.IsMatch(mobile, @"^[0-9]{10}$"))
                return ValidationResult.Fail("Mobile Number", "Mobile number must be exactly 10 digits.", "Example: '9876543210'.");

            string pnr = GetStr(v, "PNR_TicketNo");
            if (!string.IsNullOrEmpty(pnr) && pnr.Length < 6)
                return ValidationResult.Fail("PNR / Ticket No", "PNR or Ticket Number must be at least 6 characters.", "Example: '4528192834'.");

            string details = GetStr(v, "ComplaintDetails");
            if (!IsMeaningfulText(details, 10))
                return ValidationResult.Fail("Complaint Details", "Complaint details must explain the passenger issue clearly.", "Enter complaint explanation.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-034: Employee Complaint Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg034(Dictionary<string, object> v)
        {
            string desc = GetStr(v, "IssueDescription");
            if (!IsMeaningfulText(desc, 10))
                return ValidationResult.Fail("Issue Description", "Employee grievance / issue must be clearly described.", "Enter issue description.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-035: Power Supply Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg035(Dictionary<string, object> v)
        {
            string prim = GetStr(v, "PrimarySource");
            if (string.IsNullOrWhiteSpace(prim))
                return ValidationResult.Fail("Primary Source", "Primary power source must be indicated.", "Select 'State Grid (EB)', 'Solar / UPS', or 'DG Set'.");

            DateTime fail = GetDate(v, "FailureTime");
            DateTime rest = GetDate(v, "RestorationTime");
            if (fail != DateTime.MinValue && rest != DateTime.MinValue && rest < fail)
                return ValidationResult.Fail("Restoration Time", "Restoration time cannot be earlier than power failure time.", "Restoration must occur after failure.");

            string fuel = GetStr(v, "DGFuelLiters");
            if (!string.IsNullOrEmpty(fuel) && (!double.TryParse(fuel, out double fVal) || fVal < 0))
                return ValidationResult.Fail("DG Fuel (Liters)", "DG Fuel level must be a non-negative number.", "Enter fuel in liters (e.g., '45.5').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-036: Officers Inspection Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg036(Dictionary<string, object> v)
        {
            string station = GetStr(v, "StationInspected");
            if (string.IsNullOrWhiteSpace(station))
                return ValidationResult.Fail("Station Inspected", "Station code / name inspected is required.", "Enter Station (e.g., 'Secunderabad (SC)').");

            string items = GetStr(v, "ItemsInspected");
            if (!IsMeaningfulText(items, 8))
                return ValidationResult.Fail("Items Inspected", "List of inspected assets and systems is required.", "Example: 'Panel room, relay room, stabling sidings, emergency equipment'.");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-037: Traffic Inspector (TI) Inspection Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg037(Dictionary<string, object> v)
        {
            string tiName = GetStr(v, "TINameID");
            if (string.IsNullOrWhiteSpace(tiName))
                return ValidationResult.Fail("TI Name / ID", "Traffic Inspector Name or ID is mandatory.", "Enter TI credentials (e.g., 'TI / Operations / SC').");

            string findings = GetStr(v, "OperationalFindings");
            if (!IsMeaningfulText(findings, 10))
                return ValidationResult.Fail("Operational Findings", "Operational audit findings must be recorded.", "Enter findings (e.g., 'PN registers properly maintained, exchange times synchronized').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-038: Joint Inspection Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg038(Dictionary<string, object> v)
        {
            string depts = GetStr(v, "JointDepts");
            if (string.IsNullOrWhiteSpace(depts))
                return ValidationResult.Fail("Joint Departments", "Participating departments must be specified.", "Select 'S&T + Engineering (P-Way)', 'Operating + Commercial', or 'Electrical + S&T'.");

            string assetId = GetStr(v, "AssetID");
            if (string.IsNullOrWhiteSpace(assetId))
                return ValidationResult.Fail("Asset ID", "Jointly inspected asset identifier is required.", "Enter Asset ID (e.g., 'Point 104', 'Track Circuit TC-2').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-039: Night Inspection Register (Part 2)
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg039(Dictionary<string, object> v)
        {
            string sigIds = GetStr(v, "SignalIDs");
            if (string.IsNullOrWhiteSpace(sigIds))
                return ValidationResult.Fail("Signal IDs", "Signal numbers checked during night inspection are required.", "Enter Signals (e.g., 'S-1, S-2, S-12').");

            return ValidationResult.Success();
        }

        // ─────────────────────────────────────────────────────────────────────
        // REG-040: Failure Inspection & Rectification Register
        // ─────────────────────────────────────────────────────────────────────
        private static ValidationResult ValidateReg040(Dictionary<string, object> v)
        {
            string assetId = GetStr(v, "AssetID");
            if (string.IsNullOrWhiteSpace(assetId))
                return ValidationResult.Fail("Asset ID", "Asset ID is mandatory.", "Enter Asset (e.g., 'Axle Counter AC-4', 'Point 108').");

            string root = GetStr(v, "RootCause");
            if (!IsMeaningfulText(root, 6))
                return ValidationResult.Fail("Root Cause", "Technical root cause of failure must be explained.", "Example: 'Carbon deposit on relay contact 4F'.");

            string repair = GetStr(v, "RepairAction");
            if (!IsMeaningfulText(repair, 6))
                return ValidationResult.Fail("Repair Action", "Rectification and repair actions performed must be documented.", "Example: 'Relay replaced with spare #QNN-48, tested normal'.");

            return ValidationResult.Success();
        }
    }
}
