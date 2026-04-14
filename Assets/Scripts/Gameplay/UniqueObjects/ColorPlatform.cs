using UnityEngine;

public class ColorPlatform : MonoBehaviour
{
    [SerializeField] CrystalPlatformColors platformColor;
    private CrystalPlatformColors currentColorToCheck;
    Collider platformCollider;

    bool _isEnabled
    {
        set
        {
            if(platformCollider == null)
            {
                platformCollider = GetComponent<Collider>();
            }
            platformCollider.isTrigger = !value;
            
        }
    }

    public void SetPlatformCheckColor(CrystalPlatformColors color)
    {
        currentColorToCheck = color;

        _isEnabled = currentColorToCheck == platformColor;
    }
}