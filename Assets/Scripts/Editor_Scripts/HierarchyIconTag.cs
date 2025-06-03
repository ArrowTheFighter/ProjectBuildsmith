using UnityEngine;

public class HierarchyIconTag : MonoBehaviour
{
    public IconType icon = IconType.None;

    public enum IconType
    {
        None,
        Star,
        Warning,
        Skull,
        Custom
    }

    [Tooltip("Assign a custom icon if using the 'Custom' option.")]
    public Texture2D customIcon;
}
