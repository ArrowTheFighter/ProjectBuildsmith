using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class FlagManager : MonoBehaviour
{
    public static Action OnFlagSet;
    public static void wipe_flag_list()
    {
        Write_Dictionary_To_File(new Dictionary<string, bool>());
     }

    public static bool Set_Flag(string flag_id, bool flag_value = true)
    {
        Dictionary<string, bool> flag_dictionary = Get_Flag_Dictionary();
        bool ContainsKey = false;
        if (flag_dictionary.ContainsKey(flag_id)) ContainsKey = true;
        flag_dictionary[flag_id] = flag_value;
        Write_Dictionary_To_File(flag_dictionary);
        OnFlagSet?.Invoke();
        return ContainsKey;
    }

    public static bool Get_Flag_Value(string flag_id)
    {
        Dictionary<string,bool> flag_dictionary = Get_Flag_Dictionary();
        if(flag_dictionary.ContainsKey(flag_id))
        {
            return flag_dictionary[flag_id];
        }
        return false;
    }


    public static void Write_Dictionary_To_File(Dictionary<string,bool> dictionary)
    {
        if(!FlagsFolderExists())
        {
            CreateFlagDirectory();
        }
        string json = JsonConvert.SerializeObject(dictionary,Formatting.Indented);
        File.WriteAllText(Application.streamingAssetsPath + "/Flags/Flags.txt",json);
    }

    public static Dictionary<string,bool> Get_Flag_Dictionary()
    {
        if(!FlagsFolderExists())
        {
            CreateFlagDirectory();
        }
        if(!File.Exists(Application.streamingAssetsPath + "/Flags/Flags.txt"))
        {
            File.WriteAllText(Application.streamingAssetsPath + "/Flags/Flags.txt","{}");
            return new Dictionary<string,bool>();
        }
        string all_flag_lines = File.ReadAllText(Application.streamingAssetsPath + "/Flags/Flags.txt");
        Dictionary<string,bool> flag_dictionary = JsonConvert.DeserializeObject<Dictionary<string,bool>>(all_flag_lines);
        return flag_dictionary;
    }

    public static bool FlagsFolderExists()
    {
        return Directory.Exists(Application.streamingAssetsPath + "/Flags");
    }

    public static void CreateFlagDirectory()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/Flags");
    }
}
