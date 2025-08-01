using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseSlot : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] Canvas canvas;
    [SerializeField] float controllerIconOffset = 30;
    CanvasGroup canvasGroup;
    InventorySlotComponent inventorySlotComponent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        inventorySlotComponent = GetComponent<InventorySlotComponent>();
        InventorySlot inventorySlot = new InventorySlot();
        inventorySlotComponent.inventorySlot = inventorySlot;
        inventorySlot.inventorySlotComponent = inventorySlotComponent;
        inventorySlotComponent.slotEmptied += hideSlot;
        inventorySlotComponent.slotFilled += showSlot;

        GameplayUtils.instance.inventoryManager.OnInventoryClosed += InventoryClosed;

    }

    // Update is called once per frame
    void Update()
    {
        if (UIInputHandler.instance.currentScheme == "Keyboard&Mouse")
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
        else if (UIInputHandler.instance.currentScheme == "Gamepad")
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
                RectTransform selectedRectTransform = selectedObj.GetComponent<RectTransform>();
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, selectedRectTransform.position);
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
        

        // -- Drop item if clicking off the ui with an item stack --
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!inventorySlotComponent.inventorySlot.isEmpty)
            {
                if (!IsPointerOverUI())
                {
                    inventorySlotComponent.RemoveItemFromSlot(true);
                }
            }
        }
        // -- Drop one item if clicking off the ui with an item stack --
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (!inventorySlotComponent.inventorySlot.isEmpty)
            {
                if (!IsPointerOverUI())
                {
                    if (inventorySlotComponent.inventorySlot.inventoryItemStack.Amount > 1)
                    {
                        ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlotComponent.inventorySlot.inventoryItemStack.ID);
                        GameplayUtils.instance.inventoryManager.AddItemToMouseSlot(itemData, inventorySlotComponent.inventorySlot.inventoryItemStack.Amount - 1, true);
                        inventorySlotComponent.DropItem(1);
                    }
                    // if there is only one item left drop it and remove from inventory
                    else
                    {
                        inventorySlotComponent.RemoveItemFromSlot(true);
                    }
                }
            }
        }
    }

    bool ContainsNaN(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }

    void InventoryClosed()
    {
        if (!inventorySlotComponent.inventorySlot.isEmpty)
        {
            inventorySlotComponent.RemoveItemFromSlot(true);
         }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void showSlot()
    {
        canvasGroup.alpha = 1;
    }

    void hideSlot()
    {
        canvasGroup.alpha = 0;
    }

   
}
