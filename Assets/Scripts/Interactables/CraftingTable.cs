using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftingTable : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    
    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    public GameObject craftingTablePanel;
    public bool Interact(Interactor interactor)
    {
        if (!craftingTablePanel.activeInHierarchy)
        {
            craftingTablePanel.SetActive(true);
            GameplayUtils.instance.OpenMenu();
            Button[] buttons = GetComponentsInChildren<Button>();
            if (buttons.Length > 1)
            {
                buttons[1].Select();
            }
            else if (buttons.Length > 0)
            {
                buttons[0].Select();
            }
        }
        else
        {
            craftingTablePanel.SetActive(false);
            GameplayUtils.instance.CloseMenu();
        }
        return true;
    }

    public void CloseCraftingTable()
    {
        craftingTablePanel.SetActive(false);
        GameplayUtils.instance.CloseMenu();
    }

    private void Update()
    {
        if (craftingTablePanel.activeInHierarchy == true)
        {
            if (GameplayInput.instance.playerInput.actions["Pause"].ReadValue<float>() > 0)
            {
                CloseCraftingTable();
            }
            if(GameplayInput.instance.playerInput.actions["Inventory"].ReadValue<float>() > 0)
            {
                CloseCraftingTable();
            }
        }
    }
}
