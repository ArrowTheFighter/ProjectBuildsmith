using Sirenix.OdinInspector;
using UnityEngine;

public class TownCraneEvents : MonoBehaviour
{

    public GameObject CraneBridge;
    public GameObject InitalBridge;
    public GameObject Droppingridge;

    public void ShowCraneBridge()
    {
        InitalBridge.SetActive(false);
        CraneBridge.SetActive(true);
    }

    public void ShowDroppingBridge()
    {
        CraneBridge.SetActive(false);
        Droppingridge.SetActive(true);
    }

    [Button]
    public void StartAnimation()
    {
        GetComponent<Animator>().SetTrigger("Start");
    }
    
}
