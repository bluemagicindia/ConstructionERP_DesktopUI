using System.Collections.Generic;
using System.Windows;

namespace ConstructionERP_DesktopUI.Helpers
{
    public class FieldValidation
    {
        public static bool ValidateFields(List<KeyValuePair<string, string>> fields)
        {
            foreach (var field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                {
                    MessageBox.Show($"Please enter {field.Key} field", "Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            return true;
        }
    }
}
