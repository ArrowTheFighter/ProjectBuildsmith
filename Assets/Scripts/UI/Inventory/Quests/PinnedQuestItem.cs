using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedQuestItem : MonoBehaviour
{
    public TextMeshProUGUI QuestName;
    public TextMeshProUGUI QuestObjectives;
    public QuestData storedQuestData;

    public void SetQuestText(QuestData questData)
    {
        storedQuestData = questData;
        QuestName.text = questData.Name;
        QuestObjectives.text = "";
        for (int i = 0; i < questData.questObjectives.Count; i++)
        {
            QuestObjectives.text += QuestInfoBox.GetObjectiveText(questData.questObjectives[i]);
            QuestObjectives.text += "\n";
        }

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
