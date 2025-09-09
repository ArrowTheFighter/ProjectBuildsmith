using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoButton : MonoBehaviour
{
    public string QuestID;
    public QuestData questData;
    public TextMeshProUGUI buttonName;
    public bool HasViewed;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShowInfo);
    }

    void ShowInfo()
    {
        GameplayUtils.instance.inventoryManager.ShowQuestInfo(questData);
        if (!HasViewed)
        {
            buttonName.text = questData.Name;
            HasViewed = true;
         }
     }
}
