using UnityEngine;

public class MainMenuObjectToggle : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] objectsToHide;

    public void HideObjects()
    {
        foreach (MeshRenderer obj in objectsToHide)
        {
            obj.enabled = false;
        }
    }

    public void ShowObjects()
    {
        foreach (MeshRenderer obj in objectsToHide)
        {
            obj.enabled = true;
        }
    }
}
