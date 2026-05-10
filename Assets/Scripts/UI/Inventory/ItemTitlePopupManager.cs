using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ItemTitlePopupManager : MonoBehaviour
{

    public RectTransform rectTransform;
    public TextMeshProUGUI Textbox;
    public TextMeshProUGUI TextboxDescription;
    CanvasGroup canvasGroup;
    Canvas canvas;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        //rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (canvasGroup.alpha != 1) return;
        if (ScriptRefrenceSingleton.instance.uIInputHandler.currentScheme == "Keyboard&Mouse")
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out pos
            );


            if (!ContainsNaN(pos))
            {
                rectTransform.localPosition = pos;
            }

        }
        else if (ScriptRefrenceSingleton.instance.uIInputHandler.currentScheme == "Gamepad")
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
                RectTransform selectedRectTransform = selectedObj.GetComponent<RectTransform>();
                //Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, selectedRectTransform.position);
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    selectedRectTransform.position,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                    out pos
                );
                pos += Vector2.right * 30 + Vector2.up * 30;
                rectTransform.localPosition = pos;
            }
        }
    }

    public void ShowPopup(string item_name,string item_description)
    {
        if (!ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.inventoryIsOpen) return;
        Textbox.text = item_name;
        TextboxDescription.text = item_description;
        canvasGroup.alpha = 1;
        rectTransform.DOScale(1, 0.1f).From(0f).SetEase(Ease.InOutQuad);

    }

    public void HidePopup()
    {
        canvasGroup.alpha = 0;
    }

    bool ContainsNaN(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }
}
