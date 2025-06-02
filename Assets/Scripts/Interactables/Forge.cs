using UnityEngine;

public class Forge : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    private bool forgeRunning;
    public GameObject forgeFire;
    public float forgeTime;


    public bool Interact(Interactor interactor)
    {
        if(ResourceTracker.numberOfIronOre > 0)
        {
            if (!forgeRunning)
            {
                forgeRunning = true;
                ResourceTracker.numberOfIronOre--;
                Invoke("ResetForge", forgeTime);
            } 
        }
        
        return true;
    }

    public void Update()
    {
        if (ResourceTracker.numberOfIronOre == 0)
        {
            PROMPT = "Out of iron ore";
        }

        if (ResourceTracker.numberOfIronOre > 0)
        {
            PROMPT = "Craft iron bars";
        }

        if (forgeRunning)
        {
            forgeFire.SetActive(true);
        }
        else
        {
            forgeFire.SetActive(false);
        }
    }

    public void ResetForge()
    {
        ResourceTracker.numberOfIronBars++;
        forgeRunning = false;
    }
}
