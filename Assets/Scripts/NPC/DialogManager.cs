using UnityEngine;
using System.IO;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DS.Data;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    public TextMeshProUGUI text_box;
    [SerializeField] GameObject DialogUI;
    public string text_box_text
    {
        get
        {
            return text_box.text;
        }
        set
        {
            if(value != "" && !DialogUI.activeInHierarchy)
            {
                DialogUI.SetActive(true);
            }
            text_box.text = value;
            if(value == null || value == "")
            {
                Close_Dialog();
            }
        }
    }

    public TextMeshProUGUI name_text;
    [HideInInspector] public string Dialog_Name
    {
        get
        {
            return name_text.text;
        }
        set
        {
            if (name_text != null)
            {
                name_text.text = value;
            }
        }
     }

    public Transform Choices_Parent_Transform;
    
    static string Dialog_dir = "/Dialogue_Files";

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    void Start()
    {
        // string read_string;
        // read_string = FileManager.ReadStringFromGamePath("test.txt");
        // print(read_string);
    }

    public void Setup_Choices(List<DSDialogueChoiceData> data, DialogWorker dialogWorker, bool UseLocalization = true)
    {
        UIInputHandler.instance.ClosedMenu();
        for (int i = 0; i < Choices_Parent_Transform.childCount; i++)
        {
            Choices_Parent_Transform.GetChild(i).gameObject.SetActive(false);
        }
        Choices_Parent_Transform.gameObject.SetActive(true);
        bool button_is_selected = false;
        for (int i = 0; i < 4; i++)
        {
            if (i >= data.Count)
            {
                continue;
            }

            bool validChoice = DialogRetriever.Choice_is_valid(data[i].NextDialogue, dialogWorker);
            if (!validChoice) continue;
            //If node is disableChoice then we should contine

            Transform choice = Choices_Parent_Transform.GetChild(i);
            if (!button_is_selected)
            {
                //choice.GetComponent<Button>().Select();
                UIInputHandler.instance.defaultButton = choice.gameObject;
                button_is_selected = true;
            }
            choice.gameObject.SetActive(true);
            string text = data[i].Text;
            if(UseLocalization) text = LocalizationManager.GetLocalizedString("NPC", data[i].LocalizeKey);
            choice.GetComponentInChildren<TextMeshProUGUI>().text = text;
            ChoiceButton choiceButton = choice.GetComponentInChildren<ChoiceButton>();
            choiceButton.dialogueChoiceData = data[i];
            choiceButton.dialogWorker = dialogWorker;
            UIInputHandler.instance.OpenedMenu();
        }
    }

    

    public int get_active_choices()
    {
        int active_choices = 0;
        for (int i = 0; i < Choices_Parent_Transform.childCount; i++)
        {
            if (Choices_Parent_Transform.GetChild(i).gameObject.activeInHierarchy)
            {
                active_choices++;
            }
        }
        return active_choices;
    }

    [Obsolete]
    public void Set_Choices(dialog_struct[] choices, NPC_Script npc)
    {
        for (int i = 0; i < Choices_Parent_Transform.childCount; i++)
        {
            Choices_Parent_Transform.GetChild(i).gameObject.SetActive(false);
        }
        Choices_Parent_Transform.gameObject.SetActive(true);
        int current_choice_obj = 0;
        bool button_is_selected = false;
        for (int i = 0; i < 4; i++)
        {
            if (i >= choices.Length)
            {
                continue;
            }
            if (choices[i].item_requirement != null && choices[i].item_requirement != "")
            {
                int current_item_amount = GameplayUtils.instance.get_item_holding_amount(choices[i].item_requirement);
                if (current_item_amount < choices[i].item_amount)
                {
                    continue;
                }
            }
            if (choices[i].flag_requirement != null && choices[i].flag_requirement != "")
            {
                bool flag_check = FlagManager.Get_Flag_Value(choices[i].flag_requirement);
                if (!flag_check)
                {
                    continue;
                }
            }

            Transform choice = Choices_Parent_Transform.GetChild(i);
            if (!button_is_selected)
            {
                choice.GetComponent<Button>().Select();
                button_is_selected = true;
            }
            if (i >= choices.Length)
            {
                choice.gameObject.SetActive(false);
                continue;
            }
            choice.gameObject.SetActive(true);
            string localized_string = LocalizationManager.GetLocalizedString("NPC", choices[i].dialog_content);
            choice.GetComponentInChildren<TextMeshProUGUI>().text = localized_string;
            ChoiceButton choiceButton = choice.GetComponentInChildren<ChoiceButton>();
            choiceButton.button_dialog_struct = choices[i];
            choiceButton.next_dialog_id = choices[i].next_dialog_id;
            choiceButton.npc_Script = npc;
            current_choice_obj++;
        }
    }

    public void Clear_choices()
    {
        Choices_Parent_Transform.gameObject.SetActive(false);
        UIInputHandler.instance.ClosedMenu();
    }

    public void Close_Dialog()
    {
        DialogUI.SetActive(false);
        GameplayUtils.instance.CloseMenu();
    }

    public void Show_Dialog()
    {
        DialogUI.SetActive(true);
    }

    public void Hide_Dialog()
    {
        DialogUI.SetActive(false);
    }

    public static string GetDialogFileByDialogueID(string File_ID,bool return_path = false)
    {
        var info = new DirectoryInfo(Application.streamingAssetsPath + Dialog_dir);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            if(!file.Name.EndsWith(".txt")) continue;
            StreamReader reader = file.OpenText();
            string first_line = reader.ReadLine();
            if(first_line.Split("'").Length <= 1) continue;
            string id = first_line.Split("'")[1];
            id = id.Split("'")[0];
            if (id == File_ID)
            {
                StreamReader r = file.OpenText();
                string file_content = r.ReadToEnd();
                r.Close();
                reader.Close();
                if(return_path) return file.FullName;
                return file_content;
            }
            reader.Close();
        }
        return "";
    }

    public static dialog_struct GetDialogByID(string File_ID,string Dialog_ID)
    {
        if(File_ID == "" || Dialog_ID == "")
        {
            Debug.Log("Tried to get dialog info but the id was empty");
            return new dialog_struct();
        }
        dialog_struct dialog_obj = new dialog_struct();
        string file_path = GetDialogFileByDialogueID(File_ID,true);
        
        string[] lines = File.ReadAllLines(file_path);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("id"))
            {
                string line_dialog_id = GetDataInApostrophe(lines[i]);
                if (line_dialog_id == Dialog_ID)
                {
                    dialog_obj.dialog_id = line_dialog_id;
                    for(int x = 1; x < 20; x++)
                    {
                        if(i + x > lines.Length - 1) break;
                        if(lines[i + x].StartsWith("id")) break;
                        switch(lines[i + x].Split(":")[0])
                        {
                            case "content":
                                dialog_obj.dialog_content = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "next-dialog-id":
                                dialog_obj.next_dialog_id = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "close-dialog":
                                dialog_obj.close_dialog = GetDataInApostrophe(lines[i + x]) == "true";
                            break;
                            case "item-requirement":
                                dialog_obj.item_requirement = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "amount":
                                dialog_obj.item_amount = int.Parse(GetDataInApostrophe(lines[i + x]));
                            break;
                            case "remove-item":
                            case "remove-items":
                                dialog_obj.remove_item = GetDataInApostrophe(lines[i + x]) == "true";
                            break;
                            case "not-enough-reply":
                                dialog_obj.not_enough_reply = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "require-flag":
                                dialog_obj.flag_requirement = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "no-flag-reply":
                                dialog_obj.no_flag_reply = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "set-flag":
                                dialog_obj.set_flag = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "run-event":
                                dialog_obj.run_event_id = GetDataInApostrophe(lines[i + x]);
                            break;
                            case "choices":
                                dialog_obj = add_choices(dialog_obj,lines,i + x);
                                goto out_of_loop;
                            //break;
                        }
                    }
                    
                }
            }
        }
        out_of_loop:
        return dialog_obj;
    }

    static dialog_struct add_choices(dialog_struct _dialog_obj,string[] lines, int start_pos)
    {
        dialog_struct new_dialog_obj = _dialog_obj;
        dialog_struct[] dialog_choices = new dialog_struct[1];
        //A list for all the lines to check for the current choices
        List<string> choices_lines = new List<string>();
        for (int i = start_pos + 1; i < lines.Length; i++)
        {
            if(lines[i].Split(":")[0] == "id") break;
            choices_lines.Add(lines[i]);
        }
        List<dialog_struct> dialog_choices_list = new List<dialog_struct>();
        int line_number = 1;
        for (int i = 0; i < choices_lines.Count; i++)
        {
            if(choices_lines[i].Split(":")[0] == line_number.ToString())
            {
                List<string> current_choice_lines = new List<string>();
                for (int x = i; x < choices_lines.Count; x++)
                {
                    if(choices_lines[x].Split(":")[0] == (line_number + 1).ToString())
                    {
                        break;
                    }
                    current_choice_lines.Add(choices_lines[x]);
                }
                dialog_struct choice = new dialog_struct();
                for (int x = 0; x < current_choice_lines.Count; x++)
                {
                    if(current_choice_lines[x].Split(":")[0] == line_number.ToString())
                    {
                        choice.dialog_content = GetDataInApostrophe(current_choice_lines[x]);
                    }
                    switch(current_choice_lines[x].Split(":")[0])
                    {
                        case "next-dialog-id":
                            choice.next_dialog_id = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "item-requirement":
                            choice.item_requirement = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "amount":
                            choice.item_amount = int.Parse(GetDataInApostrophe(current_choice_lines[x]));
                        break;
                        case "remove-item":
                        case "remove-items":
                            choice.remove_item = GetDataInApostrophe(current_choice_lines[x]) == "true";
                        break;
                        case "not-enough-reply":
                            choice.not_enough_reply = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "require-flag":
                            choice.flag_requirement = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "no-flag-reply":
                            choice.no_flag_reply = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "set-flag":
                            choice.set_flag = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "run-event":
                            choice.run_event_id = GetDataInApostrophe(current_choice_lines[x]);
                        break;
                        case "close-dialog":
                            choice.close_dialog = GetDataInApostrophe(current_choice_lines[x]) == "true";
                        break;
                    }
                }
                dialog_choices_list.Add(choice);
                line_number ++;
            }
        }

        new_dialog_obj.choices = dialog_choices_list.ToArray();
        return new_dialog_obj;
    }

    public static string GetDataInApostrophe(string text)
    {
        //string tempt_formated_text = text.Replace("/'","/*");
        string inside_Apostrophe = text.Split("'")[1].Split("'")[0];
        //string formated_text = inside_Apostrophe.Replace("/*","'");
        return inside_Apostrophe;
    }

    public static string Format_Dialog(string text)
    {
        string formated_text = text.Replace("/'","'");
        return formated_text;
    }

}



public struct dialog_struct
{
    public string dialog_id;
    public string dialog_content;
    public string dialog_content_localized;
    public string next_dialog_id;
    public dialog_struct[] choices;
    public string item_requirement;
    public int item_amount;
    public bool remove_item;
    public string not_enough_reply;
    public string flag_requirement;
    public string no_flag_reply;
    public string set_flag;
    public string run_event_id;
    public bool close_dialog;
}
