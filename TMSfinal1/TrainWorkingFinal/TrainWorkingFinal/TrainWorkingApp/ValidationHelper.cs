using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public static class ValidationHelper
    {
        public static bool LastValidationPassed { get; set; } = true;

        public static bool ValidateForm(string regCode, Dictionary<string, object> formValues)
        {
            ValidationResult result = TrainWorkingValidationEngine.Validate(regCode, formValues);
            if (!result.IsValid)
            {
                LastValidationPassed = false;
                MessageBox.Show(result.GetFormattedMessage(), "Input Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            LastValidationPassed = true;
            return true;
        }

        public static bool IsNotEmpty(string value, string fieldName, string exampleGuidance = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                LastValidationPassed = false;
                string msg = $"❌ {fieldName} cannot be empty!\n\n💡 Reason: This field is mandatory for operational safety and audit.";
                if (!string.IsNullOrEmpty(exampleGuidance)) msg += $"\n\n✅ Valid Example: {exampleGuidance}";
                MessageBox.Show(msg, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            LastValidationPassed = true;
            return true;
        }

        public static bool IsSelected(ComboBox comboBox, string fieldName, string exampleGuidance = "")
        {
            if (comboBox.SelectedItem == null || string.IsNullOrWhiteSpace(comboBox.SelectedItem.ToString()) || comboBox.SelectedItem.ToString().StartsWith("--"))
            {
                LastValidationPassed = false;
                string msg = $"❌ Please select a valid option for '{fieldName}'!\n\n💡 Reason: Unselected or placeholder option cannot be processed.";
                if (!string.IsNullOrEmpty(exampleGuidance)) msg += $"\n\n✅ Valid Example: {exampleGuidance}";
                MessageBox.Show(msg, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            LastValidationPassed = true;
            return true;
        }

        public static bool IsEndAfterStart(DateTime start, DateTime end, string startField, string endField)
        {
            if (end < start)
            {
                LastValidationPassed = false;
                MessageBox.Show($"❌ {endField} cannot be earlier than {startField}!\n\n" +
                                $"💡 Reason: Chronological sequence violation ({end:hh:mm tt} is before {start:hh:mm tt}).\n\n" +
                                $"✅ Valid Example: Ensure {endField} is equal to or after {startField}.", 
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            LastValidationPassed = true;
            return true;
        }

        public static bool IsValidTrainNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                LastValidationPassed = false;
                MessageBox.Show("❌ Train Number cannot be empty!\n\n💡 Reason: Required to identify train movement.\n\n✅ Valid Example: '12727', '2', or '12727A'.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            Regex regex = new Regex(@"^[0-9]{1,6}[A-Za-z]?$");
            if (!regex.IsMatch(value.Trim()))
            {
                LastValidationPassed = false;
                MessageBox.Show("❌ Invalid Train Number format!\n\n💡 Reason: Must contain digits (1-6) with an optional single letter suffix. Special characters and alphabets-only are rejected.\n\n✅ Valid Example: '12727' or '12727A'.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            LastValidationPassed = true;
            return true;
        }

        public static bool IsInRange(int value, string fieldName, int min, int max)
        {
            if (value < min || value > max)
            {
                LastValidationPassed = false;
                MessageBox.Show($"❌ {fieldName} is out of allowable range!\n\n💡 Reason: Value {value} is outside valid operating limits ({min} to {max}).\n\n✅ Valid Example: Enter a number between {min} and {max}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            LastValidationPassed = true;
            return true;
        }
    }
}
