using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class TownCraneEvents : MonoBehaviour
{

    public GameObject CraneBridge;
    public GameObject InitalBridge;
    public GameObject Droppingridge;
    [Header("Cogwheels")]
    public GameObject[] topCogs;
    public GameObject[] bottomCogs;
    public UnityEvent onCogsBroken;
    [Header("Force Cutscene")]
    public NewCutsceneBuilder ForceCutscene;

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

    public void SetCompleteBridgeFlag()
    {
        FlagManager.Set_Flag("SullivanCraneBridgeRepaired");
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
        onCogsBroken?.Invoke();
        FlagManager.Set_Flag("SullivanCraneBroke");
    }

    public void ForceCutsceneOfCogsBreaking()
    {
        print("Playing cog breaking cutscene");
        ForceCutscene.PlayCutscene();
    }

    [Button]
    public void StartAnimation()
    {
        GetComponent<Animator>().SetTrigger("Start");
    }

    [Button]
    public void StartFirstHalfAnimation()
    {
        GetComponent<Animator>().Play("PickupBridgeAnimationFirstHalf");
    }

    public void SkipFirstHalfAnimation()
    {
        GetComponent<Animator>().Play("PickupBridgeAnimationFirstHalf",0,1);
        ShowCraneBridge();
        SwitchToGroundCogs();
    }

    public void StartSecondHalfAnimation()
    {
        GetComponent<Animator>().Play("PickupBridgeAnimationSecondHalf");
    }

    public void SkipSecondHalfAnimation()
    {
        GetComponent<Animator>().Play("PickupBridgeAnimationSecondHalf",0,1);
        ShowDroppingBridge();
    }


}
