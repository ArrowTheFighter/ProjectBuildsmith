using UnityEngine;
using DG.Tweening;

public class StoneCutter : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    private bool stoneCutterRunning;

    public GameObject rock;
    public Transform startPos;
    public Transform endPos;
    public float rockTime;
    private bool resetPos;

    public bool Interact(Interactor interactor)
    {
        if(ResourceTracker.numberOfRocks > 0)
        {
            if (!stoneCutterRunning)
            {
                stoneCutterRunning = true;
                ResourceTracker.numberOfRocks--;
                rock.gameObject.SetActive(true);
                rock.transform.DOMoveX(endPos.position.x, rockTime - 0.1f);
                Invoke("ResetStoneCutting", rockTime);
            } 
        }
        
        return true;
    }

    public void Update()
    {
        if(ResourceTracker.numberOfRocks == 0)
        {
            PROMPT = "Out of rocks";
        }

        if (ResourceTracker.numberOfRocks > 0)
        {
            PROMPT = "Craft Bricks";
        }

        if (resetPos)
        {
            rock.transform.position = startPos.position;
            resetPos = false;
        }
    }

    public void ResetStoneCutting()
    {
        ResourceTracker.numberOfBricks++;
        rock.gameObject.SetActive(false);
        resetPos = true;
        stoneCutterRunning = false;
    }
}
