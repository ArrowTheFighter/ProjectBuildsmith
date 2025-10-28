using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;


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
        Close();
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
        if (item_requirement_parent == null || required_items == null)
            return;


        if (item_requirement_parent != null)
        {
            for (int i = 0; i < item_requirement_parent.childCount; i++)
            {
                GameObject item_requirement_ui_object = item_requirement_parent.GetChild(i).gameObject;
                if (i < required_items.Length)
                {
                    if (required_items[i].item_amount <= 0)
                    {
                        item_requirement_ui_object.SetActive(false);
                        continue;
                    }
                    item_requirement_ui_object.SetActive(true);
                    item_requirement_ui_object.GetComponentInChildren<TextMeshProUGUI>().text = "<font-weight=500>x" + required_items[i].item_amount;
                    ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(required_items[i].item_id);
                    Image iconImage = null;
                    for (int o = 0; o < item_requirement_ui_object.transform.childCount; o++)
                    {
                        if (item_requirement_ui_object.transform.GetChild(o).GetComponent<Image>() != null)
                        {
                            iconImage = item_requirement_ui_object.transform.GetChild(o).GetComponent<Image>();
                        }
                    }
                    if (iconImage != null)
                    {
                        iconImage.sprite = itemData.item_ui_image;
                    }
                }
                else
                {
                    item_requirement_ui_object.SetActive(false);
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
