using UnityEngine;
using Paps.UnityToolbarExtenderUIToolkit;
using UnityEngine.UIElements;

[MainToolbarElement(id: "SaveTestFileButton", alignment: ToolbarAlign.Right)]
public class LoadTestFileButton : Button
{
    public void InitializeElement()
    {
        text = "Load Test File";
        clicked += () => { ScriptRefrenceSingleton.instance.saveLoadManager.LoadTest(); };
    }
}
