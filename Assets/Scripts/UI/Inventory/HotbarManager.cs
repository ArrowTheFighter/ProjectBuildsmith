using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Reflection;


public class HotbarManager : MonoBehaviour
{
    public static HotbarManager instance;
    [Header("Hotbar")]
    public RectTransform centerImage;

    public int totalSlots;
    public GameObject slotPrefab;

    public float radius;

    float currentAngle;
    float targetAngle;
    public float extraPushOutDistance = 100;
    public float CornerMaxDistance;

    int SelectedSlot = 0;
    ItemData activeItem;

    List<hotbarSlot> Slots = new List<hotbarSlot>();

    [Header("HoldingItem")]
    public Transform RightHandObj;


    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlaceIconsInCircle();
        GameplayInput.instance.playerInput.actions["HotbarNext"].performed += context => { RotateHotbar(true); };
        GameplayInput.instance.playerInput.actions["HotbarPrevious"].performed += context => { RotateHotbar(false); };
        Slots[SelectedSlot].rectTransform.localScale = Vector3.one * 1.5f;
        Slots[SelectedSlot].rectTransform.position = new Vector3(Slots[SelectedSlot].rectTransform.position.x, Slots[SelectedSlot].rectTransform.position.y, -1);
    }

    void DestroySlots()
    {
        foreach (hotbarSlot hotbarSlot in Slots)
        {
            if (hotbarSlot != null && hotbarSlot.rectTransform.gameObject != null)
                Destroy(hotbarSlot.rectTransform.gameObject);
        }
        Slots = new List<hotbarSlot>();
    }

    void PlaceIconsInCircle()
    {
        int id = 0;
        for (int i = 0; i < totalSlots; i++)
        {

            GameObject slot = Instantiate(slotPrefab, centerImage.transform);
            RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
            Slots.Add(new hotbarSlot(id, slotRectTransform));

            id++;
            if (id >= 5) id = 0;
        }
        PositionIcons();
    }

    void PositionIcons()
    {
        float startAngle = Mathf.PI / 4f;
        float angleRange = Mathf.PI * 2;

        for (int i = 0; i < Slots.Count; i++)
        {
            RectTransform slotRectTransform = Slots[i].rectTransform;

            float angle = startAngle - (i * angleRange / Slots.Count + currentAngle);


            float angleDeg = Mathf.Repeat(angle * Mathf.Rad2Deg, 360f);


            float minDistanceToDiagonal = float.MaxValue;
            float[] diagonalAngles = { 45f, 135f, 225f, 315f };

            foreach (float diag in diagonalAngles)
            {
                float dist = Mathf.Abs(Mathf.DeltaAngle(angleDeg, diag));
                if (dist < minDistanceToDiagonal)
                    minDistanceToDiagonal = dist;
            }

            float pushAmount = Mathf.Clamp01(1f - (minDistanceToDiagonal / 30f)); // 30° tolerance
            float dynamicRadius = radius + (pushAmount * extraPushOutDistance); // Push out max 30 units

            float x = Mathf.Cos(angle) * dynamicRadius;
            float y = Mathf.Sin(angle) * dynamicRadius;

            Vector2 currentPos = new Vector2(x, y);

            Vector2 topRightCorner = new Vector2(radius, radius);

            float maxDistance = CornerMaxDistance;
            float minDistance = 0f;

            float distance = Vector2.Distance(currentPos, topRightCorner);
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            t = Mathf.SmoothStep(0f, 1f, t);

            Vector2 finalPos = Vector2.Lerp(currentPos, topRightCorner, t);

            slotRectTransform.anchorMax = Vector2.one * 0.5f;
            slotRectTransform.anchorMin = Vector2.one * 0.5f;


            slotRectTransform.anchoredPosition = finalPos;
        }



    }

    void RotateHotbar(bool clockwise)
    {
        if (GameplayUtils.instance.PlayerTransform.GetComponent<CharacterMovement>().MovementControlledByAbility) return;
        Slots[SelectedSlot].rectTransform.DOScale(Vector3.one, 0.25f);
        Slots[SelectedSlot].rectTransform.position = new Vector3(Slots[SelectedSlot].rectTransform.position.x, Slots[SelectedSlot].rectTransform.position.y, 0);
        float angleStep = 2 * Mathf.PI / totalSlots;
        if (clockwise)
        {
            SelectedSlot = GetNextSelectedSlot(SelectedSlot);
            angleStep *= -1;
        }
        else
        {
            SelectedSlot = GetPreviousSelectedSlot(SelectedSlot);
        }

        Slots[SelectedSlot].rectTransform.DOScale(Vector3.one * 1.5f, 0.25f);
        Slots[SelectedSlot].rectTransform.position = new Vector3(Slots[SelectedSlot].rectTransform.position.x, Slots[SelectedSlot].rectTransform.position.y, -1);
        DOVirtual.Float(currentAngle, targetAngle + angleStep, 0.25f, context =>
        {
            currentAngle = context;
            PositionIcons();
        });
        targetAngle += angleStep;
        RemoveOldHoldingItem();
        ShowHoldingItem();
    }

    public void HotBarUpdated(int slotID)
    {
        int tempSlot = SelectedSlot;
        if (SelectedSlot > 4) tempSlot -= 5;
        if (slotID != tempSlot) return;
        RemoveOldHoldingItem();
        ShowHoldingItem();
     }

    void RemoveOldHoldingItem()
    {
        if (activeItem != null)
        {
            for (int i = 0; i < activeItem.abilityConfigs.Count; i++)
            {
                Type type = activeItem.abilityConfigs[i].GetAbilityType();
                Component component = GameplayUtils.instance.PlayerTransform.GetComponent(type);

                if (component != null)
                {
                    if (component is PlayerAbility)
                    {
                        PlayerAbility playerAbility = (PlayerAbility)component;
                        playerAbility.ResetAbility();
                     }
                    DestroyImmediate(component);
                }
                CharacterMovement characterMovement = GameplayUtils.instance.PlayerTransform.GetComponent<CharacterMovement>();
                characterMovement.playerAbilities.RemoveAll(ability => ability.GetType() == type);
            }
        }
       
        for (int i = RightHandObj.childCount - 1; i >= 0; i--)
        {
            Destroy(RightHandObj.GetChild(i).gameObject);
        }
     }

    void ShowHoldingItem()
    {
        int tempSlot = SelectedSlot;
        if (SelectedSlot > 4) tempSlot -= 5;
        InventorySlot inventorySlot = GameplayUtils.instance.inventoryManager.inventorySlots[tempSlot];
        if (inventorySlot.isEmpty) return;
        ItemData itemData = GameplayUtils.instance.GetItemDataByID(inventorySlot.inventoryItemStack.ID);
        activeItem = itemData;
        if (itemData.holdingItem != null)
        {
            GameObject heldItem = Instantiate(itemData.holdingItem, RightHandObj);
            heldItem.transform.localPosition = itemData.PositionOffset;

            Quaternion quaternion = Quaternion.Euler(itemData.Rotation);
            heldItem.transform.localRotation = quaternion;

            for (int i = 0; i < itemData.abilityConfigs.Count; i++)
            {
                CharacterMovement characterMovement = GameplayUtils.instance.PlayerTransform.GetComponent<CharacterMovement>();
                Type abilityType = itemData.abilityConfigs[i].GetAbilityType();
                MethodInfo methodInfo = typeof(CharacterMovement).GetMethod("AddAbility");
                MethodInfo genericMethod = methodInfo.MakeGenericMethod(abilityType);
                genericMethod.Invoke(characterMovement, null);

                switch (itemData.abilityConfigs[i])
                {
                    case QuickChopAbilityData quickChopAbilityData:
                        foreach (PlayerAbility playerAbility in characterMovement.playerAbilities)
                        {
                            if (playerAbility is QuickChopAbility)
                            {
                                QuickChopAbility chopAbility = (QuickChopAbility)playerAbility;
                                QuickChopAbilityData data = (QuickChopAbilityData)itemData.abilityConfigs[i];
                                chopAbility.damageStrength = data.damage;
                                chopAbility.attackTypes = data.attackTypes;
                            }
                        }
                        break;
                    case ChopSlamAbilityData chopSlamAbilityData:
                        foreach (PlayerAbility playerAbility in characterMovement.playerAbilities)
                        {
                            switch (playerAbility)
                            {
                                case ChopSlamAbility chopSlamAbility:
                                    chopSlamAbility.DamageStrength = chopSlamAbilityData.damage;
                                    chopSlamAbility.attackTypes = chopSlamAbilityData.attackTypes;
                                    break;
                            }
                        }

                        break;
                    case BoomerangAbilityData boomerangAbilityData:
                        foreach (PlayerAbility playerAbility in characterMovement.playerAbilities)
                        {
                            switch (playerAbility)
                            {
                                case BoomerangThrowAbility boomerangThrowAbility:
                                    //boomerangThrowAbility.DamageStrength = chopSlamAbilityData.damage;
                                    //boomerangThrowAbility.attackTypes = chopSlamAbilityData.attackTypes;
                                    break;
                            }
                        }
                        break;
                 }
                
                
            }
            // foreach (string abilityName in itemData.playerAbilities)
            // {
            //     CharacterMovement characterMovement = GameplayUtils.instance.PlayerTransform.GetComponent<CharacterMovement>();
            //     Type abilityType = Type.GetType(abilityName);
            //     MethodInfo methodInfo = typeof(CharacterMovement).GetMethod("AddAbility");
            //     MethodInfo genericMethod = methodInfo.MakeGenericMethod(abilityType);
            //     genericMethod.Invoke(characterMovement, null);
            // }
         }
     }

    public void HideActiveItem()
    {
        for (int i = RightHandObj.childCount - 1; i >= 0; i--)
        {
            RightHandObj.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ShowActiveItem()
    {
        for (int i = RightHandObj.childCount - 1; i >= 0; i--)
        {
            RightHandObj.GetChild(i).gameObject.SetActive(true);
        }
    }

    int GetNextSelectedSlot(int slot)
    {
        int tempSlot = slot + 1;
        if (tempSlot > Slots.Count - 1) return 0;
        return tempSlot;
    }

    int GetPreviousSelectedSlot(int slot)
    {
        int tempSlot = slot - 1;
        if (tempSlot < 0) return Slots.Count - 1;
        return tempSlot;
    }

    public void SetSlotVisuals(int slotID, InventorySlotComponent inventorySlotComponent)
    {
        foreach (hotbarSlot slot in Slots)
        {
            if (slot.slotID == slotID)
            {
                HotbarSlotComponent hotbarSlot = slot.rectTransform.GetComponent<HotbarSlotComponent>();
                hotbarSlot.slotImage.sprite = inventorySlotComponent.slotImage.sprite;
                hotbarSlot.slotAmount.text = inventorySlotComponent.slotAmountText.text;
                hotbarSlot.slotImage.color = new Color(1, 1, 1, 1);
                HotBarUpdated(slotID);
            }
            
        }
        
    }

    public void ClearSlotVisuals(int slotID)
    {
        foreach (hotbarSlot slot in Slots)
        {
            if (slot.slotID == slotID)
            {
                HotbarSlotComponent hotbarSlot = slot.rectTransform.GetComponent<HotbarSlotComponent>();
                hotbarSlot.slotImage.sprite = null;
                hotbarSlot.slotAmount.text = "";
                hotbarSlot.slotImage.color = new Color(1, 1, 1, 0);
                HotBarUpdated(slotID);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

class hotbarSlot
{
    public int slotID;
    public RectTransform rectTransform;
    public ItemData itemData;

    public hotbarSlot(int _slotID, RectTransform _rectTransform)
    {
        slotID = _slotID;
        rectTransform = _rectTransform;
    }
 }
