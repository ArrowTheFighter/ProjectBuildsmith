using UnityEngine;

[CreateAssetMenu(menuName = "UI/Icon Database")]
public class IconDatabase : ScriptableObject
{
    [System.Serializable]
    public class IconEntry
    {
        public string Action_Name;
        public string Icon_Name;
        public string Playstation_Icon_Name;
    }

    public IconEntry[] icons;

    public string GetIconString(string action_name)
    {
        foreach (var entry in icons)
        {
            if (entry.Action_Name == action_name)
            {
                return entry.Icon_Name;
            }
        }
        return "";
    }
}
