using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Obsolete]
public class NPC_Script : MonoBehaviour, IInteractable
{
    [SerializeField] string NPC_Name;
    [SerializeField] string Dialog_File_ID;
    [SerializeField] string initial_dialog_id;
    [SerializeField] string Localized_Table = "NPC";
    dialog_struct current_dialog_struct;
    dialog_struct open_dialog;
    public bool close_on_next_interact;

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    [SerializeField] public NPC_Event[] npc_events;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayUtils.instance.playerMovement_script.GetComponent<PlayerInput>().actions["Pause"].performed += EscapePressed;
        current_dialog_struct = DialogManager.GetDialogByID(Dialog_File_ID, initial_dialog_id);
    }

    void Get_Next_Dialog()
    {
        if (current_dialog_struct.next_dialog_id == "" || current_dialog_struct.next_dialog_id == null) return;
        dialog_struct old_dialog = current_dialog_struct;
        current_dialog_struct = DialogManager.GetDialogByID(Dialog_File_ID, old_dialog.next_dialog_id);
    }

    void Set_Text_Box(string text)
    {
        DialogManager.instance.text_box_text = text;
        DialogManager.instance.Dialog_Name = NPC_Name;
    }

    void Set_Text_Box_Localized(string localized_key)
    {
        string localized_string = LocalizationManager.GetLocalizedString(Localized_Table, localized_key);
        Set_Text_Box(localized_string);
    }

    void ShowCurrentDialog()
    {
        //if the dialog is the same and we try to interact then close the dialog
        // if(current_dialog_struct.dialog_id == open_dialog.dialog_id)
        // {
        //     DialogManager.instance.Close_Dialog();
        //     open_dialog = new dialog_struct();
        //     return;
        // }

        //Check if there is a flag requirement
        if (current_dialog_struct.dialog_id == open_dialog.dialog_id)
        {
            return;
        }
        if (open_dialog.flag_requirement != null && open_dialog.flag_requirement != "")
        {
            bool flag_check = FlagManager.Get_Flag_Value(open_dialog.flag_requirement);
            if (!flag_check)
            {
                if (open_dialog.no_flag_reply != null && open_dialog.no_flag_reply != "")
                {
                    Set_Text_Box_Localized(open_dialog.no_flag_reply);
                    DialogManager.instance.Clear_choices();
                    close_on_next_interact = true;
                    current_dialog_struct = open_dialog;
                    return;
                }
                else
                {
                    open_dialog = new dialog_struct();
                    DialogManager.instance.Close_Dialog();
                    close_on_next_interact = false;
                    return;
                }
            }
        }
        if (current_dialog_struct.item_requirement != null && current_dialog_struct.item_requirement != "")
        {
            int current_item_amount = GameplayUtils.instance.get_item_holding_amount(current_dialog_struct.item_requirement);
            if (current_item_amount < current_dialog_struct.item_amount)
            {
                if (current_dialog_struct.not_enough_reply != null && current_dialog_struct.not_enough_reply != "")
                {
                    Set_Text_Box_Localized(open_dialog.not_enough_reply);
                    DialogManager.instance.Clear_choices();
                    close_on_next_interact = true;
                    current_dialog_struct = open_dialog;
                    return;
                }
                else
                {
                    CloseDialog();
                }
            }
            else
            {
                if (current_dialog_struct.remove_item)
                {
                    print("removing items");
                    GameplayUtils.instance.remove_items_from_inventory(current_dialog_struct.item_requirement, current_dialog_struct.item_amount);
                }
            }
        }
        if (open_dialog.set_flag != null && open_dialog.set_flag != "")
        {
            FlagManager.Set_Flag(open_dialog.set_flag);
        }
        Set_Text_Box_Localized(current_dialog_struct.dialog_content);
        if (current_dialog_struct.choices != null)
        {
            DialogManager.instance.Set_Choices(current_dialog_struct.choices, this);
        }
        else
        {
            DialogManager.instance.Clear_choices();
        }
        open_dialog = current_dialog_struct;

        if (current_dialog_struct.run_event_id != null && current_dialog_struct.run_event_id != "")
        {
            foreach (NPC_Event e in npc_events)
            {
                if (e.id == current_dialog_struct.run_event_id)
                {
                    e.run_event.Invoke();
                }
            }
        }
        Get_Next_Dialog();
    }

    public void CloseDialog()
    {
        open_dialog = new dialog_struct();
        DialogManager.instance.Close_Dialog();
        close_on_next_interact = false;
        return;
    }

    public void SetNextDialogWithID(string id, bool show_next_dialog = false)
    {
        current_dialog_struct = DialogManager.GetDialogByID(Dialog_File_ID, id);
        if (show_next_dialog)
        {
            ShowCurrentDialog();
        }
    }

    public bool Interact(Interactor interactor)
    {
        if (!GameplayUtils.instance.can_use_dialog) return false;
        if (close_on_next_interact)
        {
            open_dialog = new dialog_struct();
            DialogManager.instance.Close_Dialog();
            close_on_next_interact = false;
            return true;
        }
        if (open_dialog.close_dialog)
        {
            open_dialog = new dialog_struct();
            DialogManager.instance.Close_Dialog();
            return true;
        }
        if (open_dialog.dialog_content != "" && open_dialog.dialog_content != null && open_dialog.next_dialog_id == null)
        {
            print(DialogManager.instance.get_active_choices());
            if (DialogManager.instance.get_active_choices() <= 0)
            {
                CloseDialog();
                return true;
            }
        }
        GameplayUtils.instance.OpenMenu();
        ShowCurrentDialog();
        return true;
    }

    public void EscapePressed(InputAction.CallbackContext action)
    {
        if (open_dialog.dialog_id != null && open_dialog.dialog_id != "")
        {
            CloseDialog();

        }
    }
}

[Serializable]
public class NPC_Event
{
    [SerializeField] public string id;
    [SerializeField] public UnityEvent run_event;
}


