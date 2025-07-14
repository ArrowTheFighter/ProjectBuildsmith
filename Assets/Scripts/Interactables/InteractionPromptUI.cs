using UnityEngine;
using TMPro;


public class InteractionPromptUI : MonoBehaviour
{
    private Camera mainCam;
    public GameObject uiPanel;
    public TextMeshProUGUI promptTextUI;
    item_requirement[] item_Requirements;
    [SerializeField] Transform item_requirement_parent;
    string fontWeight = "<font-weight=500>";

    private void Start()
    {
        mainCam = Camera.main;
        uiPanel.SetActive(false);
    }


    //This was used to make the interaction UI face the camera, but if it's in a static position on the canvas then we shouldn't need this but I'll keep it in case we revert
    /*private void LateUpdate()
    {
        var rotation = mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }*/

    public bool IsDisplayed = false;

    public void SetUp(string promptText)
    {
        promptTextUI.text = fontWeight + promptText;
        uiPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void SetUp(string promptText, item_requirement[] required_items)
    {
        SetUp(promptText);
        item_Requirements = required_items;
        setup_item_requirements(required_items);
    }

    public void ChangeText(string promptText, item_requirement[] required_items)
    {
        promptTextUI.text = fontWeight + promptText;
        setup_item_requirements(required_items);
    }

    void setup_item_requirements(item_requirement[] required_items)
    {
        if (item_requirement_parent != null)
        {
            for (int i = 0; i < item_requirement_parent.childCount; i++)
            {
                GameObject item_requiremenjt_ui_object = item_requirement_parent.GetChild(i).gameObject;
                if (i < required_items.Length)
                {

                    item_requiremenjt_ui_object.SetActive(true);
                    item_requiremenjt_ui_object.GetComponentInChildren<TextMeshProUGUI>().text = item_Requirements[i].item_name + " x" + item_Requirements[i].item_amount;
                }
                else
                {
                    item_requiremenjt_ui_object.SetActive(false);
                }
            }
         }
     }

    public void Close()
    {
        for (int i = 0; i < item_requirement_parent.childCount; i++)
        {
            item_requirement_parent.GetChild(i).gameObject.SetActive(false);
        }
        uiPanel.SetActive(false);
        IsDisplayed = false;
    }
}
