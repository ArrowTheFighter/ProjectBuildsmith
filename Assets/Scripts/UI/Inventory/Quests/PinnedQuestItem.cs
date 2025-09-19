using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedQuestItem : MonoBehaviour
{
    public TextMeshProUGUI QuestName;
    public TextMeshProUGUI QuestObjectivesTextBox;
    public QuestData storedQuestData;

    public void SetQuestText(QuestData questData)
    {
        storedQuestData = questData;
        QuestName.text = questData.QuestName;
        string QuestText = "";
        QuestObjectivesTextBox.text = "";
        // for (int i = 0; i < questData.questObjectives.Count; i++)
        // {
        //     QuestObjectives.text += QuestInfoBox.GetObjectiveText(questData.questObjectives[i]);
        //     QuestObjectives.text += "\n";
        //     if (questData.questObjectives[i].StopAtThisObjective && !questData.questObjectives[i].ObjectiveComplete())
        //     {
        //         break;
        //     }
        // }
        List<QuestObjective> questsToShow = new List<QuestObjective>();
        for (int i = 0; i < questData.questObjectives.Count; i++)
        {
            // QuestObjectivesTextBox.text += GetObjectiveText(questData.questObjectives[i]);
            // QuestObjectivesTextBox.text += "\n";
            if (questData.questObjectives[i].StopAtThisObjective && !questData.questObjectives[i].ObjectiveComplete())
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
        bool allQuestsInCollectionComplete = true;
        foreach (QuestObjective objective in questsToShow)
        {
            QuestText += QuestInfoBox.GetObjectiveText(objective);
            QuestText += "\n";

            if (!objective.ObjectiveComplete()) allQuestsInCollectionComplete = false;
            if (objective.StopAtThisObjective && !allQuestsInCollectionComplete) break;

            if (objective.StopAtThisObjective && !objective.ObjectiveComplete())
            {
                break;
            }
        }

        QuestObjectivesTextBox.GetComponent<TextFormater>().SetText(QuestText);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        StartCoroutine(ForceRebuildNextFrame());
    }

    IEnumerator ForceRebuildNextFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    public void UpdateText()
    {
        if (storedQuestData == null) return;
        SetQuestText(storedQuestData);
    }
}
