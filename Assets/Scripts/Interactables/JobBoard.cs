using UnityEngine;

public class JobBoard : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;
    
    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    public GameObject jobBoardPanel;
    public bool Interact(Interactor interactor)
    {
        jobBoardPanel.SetActive(true);
        return true;
    }

    public void CloseJobBoard()
    {
        jobBoardPanel.SetActive(false);
    }

    private void Update()
    {
        if(jobBoardPanel.activeInHierarchy == true)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                jobBoardPanel.SetActive(false);
            }
        }
    }
}
