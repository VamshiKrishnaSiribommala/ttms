using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrainWorkingApp
{
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

    public static class TrainWorkingValidationEngine
    {
        public static ValidationResult Validate(string regCode, Dictionary<string, object> values)
        {
            if (string.IsNullOrEmpty(regCode)) return ValidationResult.Success();
            string code = regCode.Trim().Replace("REG-", "").Replace("AUTH-", "").PadLeft(3, '0');

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
                default: return ValidationResult.Success();
            }
        }

        private static string NormalizeKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            return key.Replace("_", "").Replace(" ", "").Replace("-", "").Replace("/", "").ToLowerInvariant();
        }

        private static object FindValue(Dictionary<string, object> vals, string key)
        {
            if (vals == null || string.IsNullOrEmpty(key)) return null;
            if (vals.TryGetValue(key, out object directVal) && directVal != null && !string.IsNullOrWhiteSpace(directVal.ToString()) && directVal.ToString() != "-")
                return directVal;

            string normKey = NormalizeKey(key);
            foreach (var kvp in vals)
            {
                if (NormalizeKey(kvp.Key) == normKey && kvp.Value != null && !string.IsNullOrWhiteSpace(kvp.Value.ToString()) && kvp.Value.ToString() != "-")
                    return kvp.Value;
            }

            // Semantic alias mappings
            if (normKey.Contains("train"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("train") && k.Value != null && !string.IsNullOrWhiteSpace(k.Value.ToString()) && k.Value.ToString() != "-");
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("signal") && !normKey.Contains("loc"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("signal") && !NormalizeKey(k.Key).Contains("loc") && k.Value != null && !string.IsNullOrWhiteSpace(k.Value.ToString()) && k.Value.ToString() != "-");
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("loc") || normKey.Contains("section"))
            {
                var match = vals.FirstOrDefault(k => (NormalizeKey(k.Key).Contains("loc") || NormalizeKey(k.Key).Contains("section")) && k.Value != null && !string.IsNullOrWhiteSpace(k.Value.ToString()) && k.Value.ToString() != "-");
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("station"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("station") && k.Value != null && !string.IsNullOrWhiteSpace(k.Value.ToString()) && k.Value.ToString() != "-");
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("officer") || normKey.Contains("staff"))
            {
                var match = vals.FirstOrDefault(k => (NormalizeKey(k.Key).Contains("officer") || NormalizeKey(k.Key).Contains("staff")) && k.Value != null && !string.IsNullOrWhiteSpace(k.Value.ToString()) && k.Value.ToString() != "-");
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("speed"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("speed") && k.Value != null && !string.IsNullOrWhiteSpace(k.Value.ToString()) && k.Value.ToString() != "-");
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("route"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("route") && k.Value != null);
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("protection") || normKey.Contains("protect"))
            {
                var match = vals.FirstOrDefault(k => (NormalizeKey(k.Key).Contains("protect") || NormalizeKey(k.Key).Contains("line")) && k.Value != null);
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("block"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("block") && k.Value != null);
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }
            if (normKey.Contains("ack"))
            {
                var match = vals.FirstOrDefault(k => NormalizeKey(k.Key).Contains("ack") && k.Value != null);
                if (!string.IsNullOrEmpty(match.Key)) return match.Value;
            }

            return null;
        }

        private static string GetStr(Dictionary<string, object> vals, string key)
        {
            object v = FindValue(vals, key);
            if (v != null) return v.ToString().Trim();
            return string.Empty;
        }

        private static bool GetBool(Dictionary<string, object> vals, string key)
        {
            object v = FindValue(vals, key);
            if (v != null)
            {
                if (bool.TryParse(v.ToString(), out bool b)) return b;
                string s = v.ToString().Trim();
                return s.Equals("1") || s.Equals("true", StringComparison.OrdinalIgnoreCase) || s.Equals("yes", StringComparison.OrdinalIgnoreCase) || s.StartsWith("YES", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private static bool IsTrainNum(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return false;
            return Regex.IsMatch(val.Trim(), @"^[0-9]{1,6}[A-Za-z]?$");
        }

        // REG-001: Advance Authority to Overcome Defective Signal
        private static ValidationResult ValidateReg001(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train))
                return ValidationResult.Fail("Train No", "Train number is mandatory and must be digits with optional single letter suffix.", "12727, 2, 12727A");

            string sig = GetStr(v, "Signal_No");
            if (string.IsNullOrWhiteSpace(sig))
                return ValidationResult.Fail("Signal No", "Signal identification number is mandatory.", "SIG-45, HOME-1, STARTER-A");

            string loc = GetStr(v, "Signal_Location");
            if (string.IsNullOrWhiteSpace(loc))
                return ValidationResult.Fail("Signal Location", "Signal location within station limits or section is required.", "STATION-A, KM 125/4");

            string reason = GetStr(v, "Reason");
            if (string.IsNullOrWhiteSpace(reason))
                return ValidationResult.Fail("Reason", "Reason for authority issuance cannot be empty.", "Signal Defective, Signal ON");

            string station = GetStr(v, "Issuing_Station");
            if (string.IsNullOrWhiteSpace(station))
                return ValidationResult.Fail("Issuing Station", "Issuing station name is required.", "STATION-A");

            string off = GetStr(v, "Issuing_Officer");
            if (string.IsNullOrWhiteSpace(off))
                return ValidationResult.Fail("Issuing Officer", "Authorized issuing officer name/ID is required.", "SM-101, Station Master");

            string speedStr = GetStr(v, "Permitted_Speed");
            if (!int.TryParse(speedStr, out int speed) || speed <= 0 || speed > 160)
                return ValidationResult.Fail("Permitted Speed", "Permitted speed must be a valid positive number up to 160 kmph.", "15 (fixed rule for defective signal)");

            if (!GetBool(v, "Route_Clearance"))
                return ValidationResult.Fail("Route Clearance", "Route clearance confirmation is mandatory prior to issuance.", "Checked (Route Confirmed Clear)");

            if (!GetBool(v, "Line_Protection"))
                return ValidationResult.Fail("Line Protection", "Line protection arrangement must be ensured and checked.", "Checked (Protection Ensured)");

            return ValidationResult.Success();
        }

        // REG-002: Authority to Pass Signal at ON
        private static ValidationResult ValidateReg002(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train))
                return ValidationResult.Fail("Train No", "Train number is required.", "12727, 2");

            string sig = GetStr(v, "Signal_No");
            if (string.IsNullOrWhiteSpace(sig))
                return ValidationResult.Fail("Signal No", "Signal number is mandatory.", "HOME-1, STARTER-2");

            string speedStr = GetStr(v, "Permitted_Speed");
            if (!int.TryParse(speedStr, out int speed) || speed <= 0 || speed > 160)
                return ValidationResult.Fail("Permitted Speed", "Permitted speed must be between 1 and 160 kmph.", "15 kmph");

            if (!GetBool(v, "Route_Clearance"))
                return ValidationResult.Fail("Route Clearance", "Route clearance must be verified and checked.", "Checked");

            return ValidationResult.Success();
        }

        // REG-003: Caution Order Entry
        private static ValidationResult ValidateReg003(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train) && !train.Equals("ALL", StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Fail("Train No", "Train number or 'ALL' for general caution order is required.", "12727 or ALL");

            string sec = GetStr(v, "Section_Location");
            if (string.IsNullOrWhiteSpace(sec))
                return ValidationResult.Fail("Section / Location", "Track section or kilometer location is required.", "KM 142/10 - 143/02");

            string spd = GetStr(v, "Speed_Restriction");
            if (!int.TryParse(spd, out int s) || s <= 0 || s > 130)
                return ValidationResult.Fail("Speed Restriction", "Speed restriction must be between 1 and 130 kmph.", "30 or 45");

            string reason = GetStr(v, "Reason");
            if (string.IsNullOrWhiteSpace(reason))
                return ValidationResult.Fail("Reason", "Reason for caution restriction is mandatory.", "Track Renewal, Bridge Maintenance");

            return ValidationResult.Success();
        }

        // REG-004: Authority to Receive Train on Obstructed Line
        private static ValidationResult ValidateReg004(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train No", "Train number required.", "12727");

            string line = GetStr(v, "Line_No");
            if (string.IsNullOrWhiteSpace(line)) return ValidationResult.Fail("Line No", "Line number required.", "Line 1, Platform 2");

            string obs = GetStr(v, "Nature_of_Obstruction");
            if (string.IsNullOrWhiteSpace(obs)) return ValidationResult.Fail("Nature of Obstruction", "Nature of obstruction required.", "Stabled Rake, Track Machine");

            return ValidationResult.Success();
        }

        // REG-005: Authority to Receive on Non-Signalled Line
        private static ValidationResult ValidateReg005(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train No", "Train number required.", "12727");

            string line = GetStr(v, "Line_No");
            if (string.IsNullOrWhiteSpace(line)) return ValidationResult.Fail("Line No", "Line number required.", "Line 3");

            string stn = GetStr(v, "Receiving_Station");
            if (string.IsNullOrWhiteSpace(stn)) return ValidationResult.Fail("Receiving Station", "Receiving station required.", "STATION-A");

            return ValidationResult.Success();
        }

        // REG-006: Authority to Start from Non-Signalled Line
        private static ValidationResult ValidateReg006(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train No", "Train number required.", "12727");

            string line = GetStr(v, "Line_No");
            if (string.IsNullOrWhiteSpace(line)) return ValidationResult.Fail("Line No", "Line number required.", "Goods Line 2");

            return ValidationResult.Success();
        }

        // REG-007: Authority for Common Starter Signal
        private static ValidationResult ValidateReg007(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train No", "Train number required.", "12727");

            string sig = GetStr(v, "Starter_Signal_ID");
            if (string.IsNullOrWhiteSpace(sig)) return ValidationResult.Fail("Starter Signal ID", "Starter signal ID is required.", "CS-01");

            return ValidationResult.Success();
        }

        // REG-008: Relief Train Authorization
        private static ValidationResult ValidateReg008(Dictionary<string, object> v)
        {
            string rTrain = GetStr(v, "Relief_Train_No");
            if (string.IsNullOrWhiteSpace(rTrain)) return ValidationResult.Fail("Relief Train No", "Relief train number is required.", "RT-901, 12728");

            string fTrain = GetStr(v, "Failed_Train_No");
            if (string.IsNullOrWhiteSpace(fTrain)) return ValidationResult.Fail("Failed Train No", "Failed train number is required.", "12727");

            string sec = GetStr(v, "Block_Section");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("Block Section", "Block section of failure required.", "Section A-B");

            return ValidationResult.Success();
        }

        // REG-009: Communication Failure Log
        private static ValidationResult ValidateReg009(Dictionary<string, object> v)
        {
            string sec = GetStr(v, "Section_Affected");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("Section Affected", "Affected section is required.", "Section A-B");

            string type = GetStr(v, "Type_of_Failure");
            if (string.IsNullOrWhiteSpace(type)) return ValidationResult.Fail("Type of Failure", "Type of communication failure required.", "Block Instrument, Magneto Phone, OFC");

            return ValidationResult.Success();
        }

        // REG-010 / REG-017: Line Clear Inquiry TFC
        private static ValidationResult ValidateReg010(Dictionary<string, object> v)
        {
            string send = GetStr(v, "Sending_Station");
            string recv = GetStr(v, "Receiving_Station");
            if (string.IsNullOrWhiteSpace(send)) return ValidationResult.Fail("Sending Station", "Sending station required.", "STATION-A");
            if (string.IsNullOrWhiteSpace(recv)) return ValidationResult.Fail("Receiving Station", "Receiving station required.", "STATION-B");
            if (send.Equals(recv, StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Fail("Receiving Station", "Sending and receiving stations cannot be identical.", "Different station name");

            return ValidationResult.Success();
        }

        // REG-011: Temporary Single Line Working
        private static ValidationResult ValidateReg011(Dictionary<string, object> v)
        {
            string sec = GetStr(v, "Affected_Section");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("Affected Section", "Affected double line section required.", "Section A-B");

            string line = GetStr(v, "Blocked_Line");
            if (string.IsNullOrWhiteSpace(line)) return ValidationResult.Fail("Blocked Line", "Blocked line name required.", "UP Line / DN Line");

            return ValidationResult.Success();
        }

        // REG-012: Shunting Order Management
        private static ValidationResult ValidateReg012(Dictionary<string, object> v)
        {
            string stn = GetStr(v, "Station");
            if (string.IsNullOrWhiteSpace(stn)) return ValidationResult.Fail("Station", "Station name is required.", "STATION-A");

            string eng = GetStr(v, "Engine_No");
            if (string.IsNullOrWhiteSpace(eng)) return ValidationResult.Fail("Engine No", "Shunting locomotive/engine number required.", "WDG4-12345");

            return ValidationResult.Success();
        }

        // REG-013: Signal Passing Authority
        private static ValidationResult ValidateReg013(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train No", "Train number required.", "12727");

            string sig = GetStr(v, "Signal_No");
            if (string.IsNullOrWhiteSpace(sig)) return ValidationResult.Fail("Signal No", "Signal number required.", "SIG-12");

            return ValidationResult.Success();
        }

        // REG-014: ABS Proceed Without Line Clear
        private static ValidationResult ValidateReg014(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_No");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train No", "Train number required.", "12727");

            string sec = GetStr(v, "ABS_Section");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("ABS Section", "Automatic Block Signalling section required.", "ABS-SEC-01");

            return ValidationResult.Success();
        }

        // REG-015: ABS Relief Engine Authority
        private static ValidationResult ValidateReg015(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Relief_Train_No");
            if (string.IsNullOrWhiteSpace(train)) return ValidationResult.Fail("Relief Train / Engine No", "Relief locomotive number required.", "RT-101");

            string sec = GetStr(v, "Block_Section");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("Block Section", "Block section required.", "ABS Section 1-2");

            return ValidationResult.Success();
        }

        // REG-016: ABS Prolonged Signal Failure
        private static ValidationResult ValidateReg016(Dictionary<string, object> v)
        {
            string sigs = GetStr(v, "Affected_Signals");
            if (string.IsNullOrWhiteSpace(sigs)) return ValidationResult.Fail("Affected Signals", "Affected signals list is required.", "AS-1, AS-2, AS-3");

            return ValidationResult.Success();
        }

        // REG-017: Line Clear Inquiry Reply
        private static ValidationResult ValidateReg017(Dictionary<string, object> v)
        {
            return ValidateReg010(v);
        }

        // REG-018: Line Clear Tickets (T/A 1425 / T/B 1425)
        private static ValidationResult ValidateReg018(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_Number");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train Number", "Train number required.", "12727");

            string dir = GetStr(v, "Direction");
            if (string.IsNullOrWhiteSpace(dir)) return ValidationResult.Fail("Direction", "Train direction required (UP / DN).", "UP or DN");

            return ValidationResult.Success();
        }

        // REG-019: Maintenance Trolley Notice
        private static ValidationResult ValidateReg019(Dictionary<string, object> v)
        {
            string sec = GetStr(v, "Section");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("Section", "Working section required.", "KM 120/0 - 125/0");

            string staff = GetStr(v, "Staff_In_Charge");
            if (string.IsNullOrWhiteSpace(staff)) return ValidationResult.Fail("Staff In-Charge", "Name/Designation of staff in-charge required.", "SSE/P-Way Sharma");

            return ValidationResult.Success();
        }

        // REG-020: Motor Trolley Permit
        private static ValidationResult ValidateReg020(Dictionary<string, object> v)
        {
            string sec = GetStr(v, "Section");
            if (string.IsNullOrWhiteSpace(sec)) return ValidationResult.Fail("Section", "Section required.", "Section A-B");

            string staff = GetStr(v, "Staff_In_Charge");
            if (string.IsNullOrWhiteSpace(staff)) return ValidationResult.Fail("Staff In-Charge", "Staff in charge required.", "SSE/Signal Rao");

            return ValidationResult.Success();
        }

        // REG-021: Signal & Telecom Disconnection Notice
        private static ValidationResult ValidateReg021(Dictionary<string, object> v)
        {
            string eq = GetStr(v, "Equipment_ID");
            if (string.IsNullOrWhiteSpace(eq)) return ValidationResult.Fail("Equipment ID", "Signal/Point equipment ID required.", "Point No 102A, Signal S-5");

            string reason = GetStr(v, "Reason");
            if (string.IsNullOrWhiteSpace(reason)) return ValidationResult.Fail("Reason", "Reason for disconnection required.", "Motor Maintenance, Relay Testing");

            return ValidationResult.Success();
        }

        // REG-022: Train Movement Log
        private static ValidationResult ValidateReg022(Dictionary<string, object> v)
        {
            string train = GetStr(v, "Train_Number");
            if (!IsTrainNum(train)) return ValidationResult.Fail("Train Number", "Train number required.", "12727");

            string name = GetStr(v, "Train_Name");
            if (string.IsNullOrWhiteSpace(name)) return ValidationResult.Fail("Train Name", "Train name required.", "Godavari Express, Vande Bharat");

            return ValidationResult.Success();
        }
    }
}
