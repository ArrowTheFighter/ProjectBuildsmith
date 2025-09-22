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
            buttonName.text = questData.QuestName;
            HasViewed = true;
        }
    }

    public void SetComplete()
    {
        HasViewed = true;
        buttonName.text = $"<font-weight=700>[x]</font-weight> {questData.QuestName}";
    }
}
