using UnityEngine;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

[MainToolbarElement(id: "SaveTestFileButton",alignment: ToolbarAlign.Right)]
public class SaveTestFileButton : Button
{
    public void InitializeElement()
    {
        text = "Save Test File";
        clicked += () => { ScriptRefrenceSingleton.instance.saveLoadManager.SaveTest(); };
    }
}
