using TMPro;
using UnityEngine;

public class BuildVersionText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent(out TextMeshProUGUI component))
        {
            component.text = "Build # " + Application.version;
         }
    }

}
