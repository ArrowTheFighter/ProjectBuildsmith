using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    //Writes a file to where the game is on the computer
    public static void WriteStringToGamePath(string file_name,string file_contents)
    {
        string path = Application.dataPath + "/" + file_name;
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(file_contents);
        writer.Close();
        StreamReader reader = new StreamReader(path);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();

    }

    //Reads a file from where the game is on the computer
    public static string ReadStringFromGamePath(string file_name)
    {
        string path = Application.dataPath + "/test.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string file_contents = reader.ReadToEnd();
        reader.Close();
        return file_contents;
    }


    
}
