
using UnityEngine.Localization.Settings;

public class LocalizationManager
{
       public static string GetLocalizedString(string table, string key)
       {
          string localized_string = LocalizationSettings.StringDatabase.GetLocalizedString(table,key);
          return localized_string;
       }
}
