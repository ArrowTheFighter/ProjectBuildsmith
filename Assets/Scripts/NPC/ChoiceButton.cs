using System;
using DS.Data;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour
{
    [Obsolete]
    public NPC_Script npc_Script;
    [Obsolete]
    public string next_dialog_id;
    [Obsolete]
    public dialog_struct button_dialog_struct;
    public DialogWorker dialogWorker;
    public DSDialogueChoiceData dialogueChoiceData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Button_Pressed);
    }

    void Button_Pressed()
    {
        print("button pressed");
        dialogWorker.GetAndShowNextDialog(dialogueChoiceData.NextDialogue);
    }

    [Obsolete]
    void button_clicked()
    {
        if(button_dialog_struct.flag_requirement != null && button_dialog_struct.flag_requirement != "")
        {
            bool flag_check = FlagManager.Get_Flag_Value(button_dialog_struct.flag_requirement);
            if(!flag_check)
            {
                if(button_dialog_struct.no_flag_reply != null && button_dialog_struct.no_flag_reply != "")
                {
                    string localized_string = LocalizationManager.GetLocalizedString("NPC",button_dialog_struct.no_flag_reply);
                    ScriptRefrenceSingleton.instance.dialogManager.text_box_text = localized_string;
                    ScriptRefrenceSingleton.instance.dialogManager.Clear_choices();
                    npc_Script.close_on_next_interact = true;
                    return;
                }
            }
        }
        if(button_dialog_struct.item_requirement != null && button_dialog_struct.item_requirement != "")
        {
            int current_item_amount = ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(button_dialog_struct.item_requirement);
            if(current_item_amount < button_dialog_struct.item_amount)
            {
                if(button_dialog_struct.not_enough_reply != null && button_dialog_struct.not_enough_reply != "")
                {
                    string localized_string = LocalizationManager.GetLocalizedString("NPC",button_dialog_struct.not_enough_reply);
                    ScriptRefrenceSingleton.instance.dialogManager.text_box_text = localized_string;
                    ScriptRefrenceSingleton.instance.dialogManager.Clear_choices();
                    npc_Script.close_on_next_interact = true;
                    return;
                }
            }
            else if(button_dialog_struct.remove_item)
            {
                ScriptRefrenceSingleton.instance.gameplayUtils.remove_items_from_inventory(button_dialog_struct.item_requirement,button_dialog_struct.item_amount);
            }
        }
         if(button_dialog_struct.set_flag != null && button_dialog_struct.set_flag != "")
        {
            FlagManager.Set_Flag(button_dialog_struct.set_flag);
        }
        if(button_dialog_struct.run_event_id != null && button_dialog_struct.run_event_id != "")
        {
            foreach (NPC_Event e in npc_Script.npc_events)
            {
                if(e.id == button_dialog_struct.run_event_id)
                {
                    e.run_event.Invoke();
                }
            }
        }
        if (button_dialog_struct.close_dialog)
        {
            if (button_dialog_struct.next_dialog_id != null && button_dialog_struct.next_dialog_id != "")
            {
                npc_Script.SetNextDialogWithID(next_dialog_id, false);
            }
            npc_Script.CloseDialog();
        }
        else
        {
            npc_Script.SetNextDialogWithID(next_dialog_id, true);
        }
    }
}
