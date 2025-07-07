using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI QuestNameTextBox;
    [SerializeField] TextMeshProUGUI QuestDescriptionTextBox;
    [SerializeField] TextMeshProUGUI QuestObjectivesTextBox;

    public void ShowQuestInfo(QuestData questData)
    {
        QuestNameTextBox.text = questData.Name;
        QuestDescriptionTextBox.text = questData.Description;
        QuestObjectivesTextBox.text = "";
        for (int i = 0; i < questData.questObjectives.Count; i++)
        {
            QuestObjectivesTextBox.text += GetObjectiveText(questData.questObjectives[i]);
            QuestObjectivesTextBox.text += "\n";
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(QuestDescriptionTextBox.transform.parent.GetComponent<RectTransform>());
    }

    public void ClearQuestInfo()
    {
        QuestNameTextBox.text = "";
        QuestDescriptionTextBox.text = "";
        QuestObjectivesTextBox.text = "";
    }

    public static string GetObjectiveText(QuestObjective questObjective)
    {
        string completionStatus = "<sprite name=\"CheckBoxEmpty\"> ";
        string returnText = "";
        switch (questObjective)
        {
            case ObjectiveCollectItems collectItems:

                if (collectItems.ObjectiveComplete()) completionStatus = "<sprite name=\"CheckBoxMarked\"> ";
                returnText = completionStatus + collectItems.Description;
                break;

            case ObjectiveTalkToNPCFlag talkToNPCFlag:
                if (talkToNPCFlag.ObjectiveComplete()) completionStatus = "<sprite name=\"CheckBoxMarked\"> ";
                returnText = completionStatus + talkToNPCFlag.Description;
                break;
        }
        return returnText;
    }
}
