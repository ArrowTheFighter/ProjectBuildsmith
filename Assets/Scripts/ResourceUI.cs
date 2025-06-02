using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    //References to the UI elements for each type of resource
    public TextMeshProUGUI rocksAmountText;
    public TextMeshProUGUI woodAmountText;
    public TextMeshProUGUI planksAmountText;
    public TextMeshProUGUI bricksAmountText;
    public TextMeshProUGUI ironOreAmountText;
    public TextMeshProUGUI ironBarsAmountText;

    public TextMeshProUGUI ironNailsAmountText;

    // Update is called once per frame
    void Update()
    {
        //Keeps the UI text updated to the current amount of each resource
        rocksAmountText.text = "Rocks: " + ResourceTracker.numberOfRocks;
        woodAmountText.text = "Wood: " + ResourceTracker.numberOfWood;
        planksAmountText.text = "Planks: " + ResourceTracker.numberOfPlanks;
        bricksAmountText.text = "Bricks: " + ResourceTracker.numberOfBricks;
        ironOreAmountText.text = "Iron Ore: " + ResourceTracker.numberOfIronOre;
        ironBarsAmountText.text = "Iron Bars: " + ResourceTracker.numberOfIronBars;

        ironNailsAmountText.text = "Nails: " + ResourceTracker.numberOfIronNails;

        if (ResourceTracker.numberOfRocks == 0)
        {
            rocksAmountText.enabled = false;
        }
        else
        {
            rocksAmountText.enabled = true;
        }

        if (ResourceTracker.numberOfWood == 0)
        {
            woodAmountText.enabled = false;
        }
        else
        {
            woodAmountText.enabled = true;
        }

        if (ResourceTracker.numberOfPlanks == 0)
        {
            planksAmountText.enabled = false;
        }
        else
        {
            planksAmountText.enabled = true;
        }

        if (ResourceTracker.numberOfBricks == 0)
        {
            bricksAmountText.enabled = false;
        }
        else
        {
            bricksAmountText.enabled = true;
        }

        if (ResourceTracker.numberOfIronOre == 0)
        {
            ironOreAmountText.enabled = false;
        }
        else
        {
            ironOreAmountText.enabled = true;
        }

        if (ResourceTracker.numberOfIronBars == 0)
        {
            ironBarsAmountText.enabled = false;
        }
        else
        {
            ironBarsAmountText.enabled = true;
        }

        if (ResourceTracker.numberOfIronNails == 0)
        {
            ironNailsAmountText.enabled = false;
        }
        else
        {
            ironNailsAmountText.enabled = true;
        }
    }
}