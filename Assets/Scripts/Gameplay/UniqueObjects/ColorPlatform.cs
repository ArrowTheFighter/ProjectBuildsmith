using DG.Tweening;
using UnityEngine;

public class ColorPlatform : MonoBehaviour
{
    [SerializeField] CrystalPlatformColors platformColor;
    [SerializeField] bool SpawnDisabled;
    private CrystalPlatformColors currentColorToCheck;
    Collider platformCollider;
    Vector3 platformScale;

    void Awake()
    {
        platformScale = transform.localScale;
        if(SpawnDisabled)
        {
            transform.localScale = Vector3.zero;
        }
    }

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

    public void ScaleInPlatform()
    {
        transform.DOScale(platformScale,0.5f).SetEase(Ease.InOutQuad);
    }
}