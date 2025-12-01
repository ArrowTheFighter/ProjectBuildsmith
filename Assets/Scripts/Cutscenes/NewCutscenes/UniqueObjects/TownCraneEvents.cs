using Sirenix.OdinInspector;
using UnityEngine;

public class TownCraneEvents : MonoBehaviour
{

    public GameObject CraneBridge;
    public GameObject InitalBridge;
    public GameObject Droppingridge;
    [Header("Cogwheels")]
    public GameObject[] topCogs;
    public GameObject[] bottomCogs;

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

    public void SwitchToGroundCogs()
    {
        foreach(var cog in topCogs)
        {
            cog.SetActive(false);
        }

        foreach(var cog in bottomCogs)
        {
            cog.SetActive(true);
        }
        FlagManager.Set_Flag("SullivanCraneBroke");
    }

    [Button]
    public void StartAnimation()
    {
        GetComponent<Animator>().SetTrigger("Start");
    }

    public void StartFirstHalfAnimation()
    {
        GetComponent<Animator>().Play("PickupBridgeAnimationFirstHalf");
    }

    public void StartSecondHalfAnimation()
    {
        GetComponent<Animator>().Play("PickupBridgeAnimationSecondHalf");
    }

}
