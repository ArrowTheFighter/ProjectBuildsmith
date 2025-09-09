using System.Collections.Generic;
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
        List<QuestObjective> questsToShow = new List<QuestObjective>();
        for (int i = 0; i < questData.questObjectives.Count; i++)
        {
            // QuestObjectivesTextBox.text += GetObjectiveText(questData.questObjectives[i]);
            // QuestObjectivesTextBox.text += "\n";
            if (!questData.questObjectives[i].ObjectiveComplete() && questData.questObjectives[i].StopAtThisObjective)
            {
                foreach (QuestObjective questObjective in questData.questObjectives)
                {
                    if (questObjective.ObjectiveIDCollection == questData.questObjectives[i].ObjectiveIDCollection)
                    {
                        questsToShow.Add(questObjective);
                    }
                }
                break;
            }
        }
        foreach (QuestObjective objective in questsToShow)
        {
            QuestObjectivesTextBox.text += GetObjectiveText(objective);
            QuestObjectivesTextBox.text += "\n";

            if (objective.StopAtThisObjective && !objective.ObjectiveComplete())
            {
                break;
            }
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
        if(questObjective.ObjectiveComplete()) completionStatus = "<sprite name=\"CheckBoxMarked\"> ";
        returnText = completionStatus + questObjective.Description;
        // switch (questObjective)
        // {
        //     case ObjectiveCollectItems collectItems:

        //         if (collectItems.ObjectiveComplete()) completionStatus = "<sprite name=\"CheckBoxMarked\"> ";
        //         returnText = completionStatus + collectItems.Description;
        //         break;

        //     case ObjectiveTalkToNPCFlag talkToNPCFlag:
        //         if (talkToNPCFlag.ObjectiveComplete()) completionStatus = "<sprite name=\"CheckBoxMarked\"> ";
        //         returnText = completionStatus + talkToNPCFlag.Description;
        //         break;
        //     case ObjectiveUseInput objectiveUseInput:
        //         if (objectiveUseInput.ObjectiveComplete()) completionStatus =
        // }
        return returnText;
    }
}
