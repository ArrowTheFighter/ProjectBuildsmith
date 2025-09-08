using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameplayUtils : MonoBehaviour
{
    public static GameplayUtils instance;

    [SerializeField] public Transform PlayerTransform;
    [SerializeField] MouseLock mouseLock_script;
    [SerializeField] CinemachineInputAxisController cameraInputComponent;
    //[SerializeField] public PlayerMovement playerMovement_script;
    [SerializeField] public InventoryManager inventoryManager;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject[] UI_Canvases;
    [SerializeField] Slider Main_volume_slider;
    [SerializeField] ItemPickupNotifcationScript itemPickupNotifcationScript;
    [SerializeField] PlayerAudio playerAudio;
    [Header("Crafting stations")]
    [SerializeField] CanvasGroup CraftingTableUI;
    [SerializeField] GameObject ToolCraftingUI;
    [SerializeField] GameObject SawmillCraftingUI;
    [Header("Inventory UI")]
    [SerializeField] GameObject PedistalUI;
    public event Action OnInventoryClosed;
    [Header("AnimationEvents")]
    public AnimationEvents animationEvents;
    public List<Transform> respawnPoints = new List<Transform>();
    public bool can_use_dialog = true;
    public bool DialogIsOpen;
    public bool PauseMenuIsOpen;
    bool open_menu;
    public RecipeDatabase RecipeDatabase;
    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
        RecipeDatabase = Resources.Load<RecipeDatabase>("Recipes/RecipeDatabase");
    }

    void Start()
    {
        // -- Reset Flags --
        FlagManager.wipe_flag_list();
        //cameraInputComponent = playerMovement_script.transform.GetComponentInChildren<CinemachineInputAxisController>();
        AudioListener.volume = Main_volume_slider.value;
    }

    public bool OpenDialogMenu()
    {
        if (PauseMenuIsOpen) return false;
        DialogIsOpen = true;
        OpenMenu();
        return true;
    }

    public bool CloseDialogMenu()
    {
        if (PauseMenuIsOpen) return false;
        DialogIsOpen = false;
        CloseMenu();
        return true;
    }

    public bool GetOpenMenu()
    {
        return open_menu;
    }

    public void OpenInventoryUI(InventroyTypes inventroyType)
    {
        switch (inventroyType)
        {
            case InventroyTypes.Pedistal:

                inventoryManager.OpenInventory();
                PedistalUI.SetActive(true);
                OpenMenu();
                break;
         }
    }

    public void SetSawmillProgressBar(float progress)
    {
        SawmillCraftingUI.GetComponentInChildren<Slider>().value = progress;
    }

    void EnableCraftingTableUI()
    {
        CraftingTableUI.gameObject.SetActive(true);
        CraftingTableUI.alpha = 1;
        CraftingTableUI.blocksRaycasts = true;
        CraftingTableUI.interactable = true;
     }

    void DisableCraftingTableUI()
    {
        CraftingTableUI.alpha = 0;
        CraftingTableUI.blocksRaycasts = false;
        CraftingTableUI.interactable = false;
        CraftingTableUI.gameObject.SetActive(false);
        OnInventoryClosed?.Invoke();
    }

    public void ToggleCraftingMenu(CraftingStationTypes stationType)
    {
        switch (stationType)
        {
            case CraftingStationTypes.Tool:
                if (ToolCraftingUI.activeInHierarchy)
                {
                    OpenCraftingMenu(stationType);
                }
                else
                {
                    CloseCraftingMenu(stationType);
                }
                break;
        }
    }



    public void OpenCraftingMenu(CraftingStationTypes stationType)
    {
        CloseAllCraftingMenus();
        switch (stationType)
        {
            case CraftingStationTypes.Tool:
                inventoryManager.OpenInventory();
                EnableCraftingTableUI();
                ToolCraftingUI.SetActive(true);
                OpenMenu();
                break;
            case CraftingStationTypes.Sawmill:
            case CraftingStationTypes.Furnace:
            case CraftingStationTypes.StoneCutter:
                inventoryManager.OpenInventory();
                EnableCraftingTableUI();
                SawmillCraftingUI.SetActive(true);
                OpenMenu();
                break;
        }
    }
    public void CloseCraftingMenu(CraftingStationTypes stationType)
    {
        switch (stationType)
        {
            case CraftingStationTypes.Tool:
                inventoryManager.CloseInventory();
                DisableCraftingTableUI();
                ToolCraftingUI.SetActive(false);
                CloseMenu();
                break;
            case CraftingStationTypes.Sawmill:
                inventoryManager.CloseInventory();
                DisableCraftingTableUI();
                SawmillCraftingUI.SetActive(false);
                CloseMenu();
                break;
        }
    }

    public void CloseAllCraftingMenus()
    {
        DisableCraftingTableUI();
        ToolCraftingUI.SetActive(false);
        SawmillCraftingUI.SetActive(false);
        PedistalUI.SetActive(false);
    }


    public void OpenMenu()
    {
        mouseLock_script.Release_Mouse();
        cameraInputComponent.enabled = false;
        GameplayInput.instance.SwitchToUI();
        //playerMovement_script.can_control_player = false;
        open_menu = true;
        UIInputHandler.instance.OpenedMenu();
    }

    public void CloseMenu()
    {
        if (!open_menu) return;
        mouseLock_script.Capture_Mouse();
        GameplayInput.instance.SwitchToGameplay();
        cameraInputComponent.enabled = true;
        //playerMovement_script.can_control_player = true;
        open_menu = false;
        UIInputHandler.instance.ClosedMenu();
    }

    public void Freeze_Player(bool release_mouse = false)
    {
        if (release_mouse) mouseLock_script.Release_Mouse();
        cameraInputComponent.enabled = false;
        //playerMovement_script.can_control_player = false;
    }

    public void HideUI()
    {
        foreach (GameObject obj in UI_Canvases)
        {
            if (obj.GetComponent<CanvasGroup>() != null)
            {
                obj.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                obj.SetActive(false);

            }
        }
    }

    public void ShowUI()
    {
        foreach (GameObject obj in UI_Canvases)
        {
            if (obj.GetComponent<CanvasGroup>() != null)
            {
                obj.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                obj.SetActive(true);

            }
        }
    }

    public void Unfreeze_Player()
    {
        mouseLock_script.Capture_Mouse();
        cameraInputComponent.enabled = true;
        //playerMovement_script.can_control_player = true;
    }

    public void set_can_use_dialog(bool can_use)
    {
        can_use_dialog = can_use;
    }

    public bool OpenPauseMenu()
    {
        if (DialogIsOpen) return false;
        PauseMenuIsOpen = true;
        PauseMenu.SetActive(true);
        PauseMenu.GetComponent<PauseMenu>().OpenMainScreen();
        OpenMenu();
        Time.timeScale = 0;



        return true;
    }


    public void ClosePauseMenu()
    {
        PauseMenuIsOpen = false;
        CloseMenu();
        PauseMenu.SetActive(false);
        Time.timeScale = 1;

    }

    public void Toggle_Pause_Menu()
    {
        if (open_menu)
        {
            CloseAllCraftingMenus();
            inventoryManager.CloseInventory();
            ClosePauseMenu();
            return;
         }
        if (PauseMenu.activeInHierarchy)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void MoveToMainMenu()
    {
        SceneManager.LoadScene(0);
    }


    public void SetMainVolume(System.Single vol)
    {
        AudioListener.volume = vol;
    }

    public void add_respawn_point(Transform point)
    {
        respawnPoints.Add(point);
    }

    public Transform get_closest_respawn_point(Vector3 pos)
    {
        if (respawnPoints.Count <= 0)
        {
            print("No respawn points");
            return null;
        }
        Transform closest_point = respawnPoints[0];
        foreach (Transform point in respawnPoints)
        {
            if (Vector3.Distance(point.position, pos) < Vector3.Distance(closest_point.position, pos))
            {
                closest_point = point;
            }
        }

        return closest_point;
    }

    public void Move_Player_To_Closest_Respawn_Point()
    {
        Transform respawn_point = get_closest_respawn_point(PlayerTransform.position);
        if (respawn_point == null) return;
        Time.timeScale = 1;
        ClosePauseMenu();
        //TODO \/ \/ \/
        //playerMovement_script.Set_Player_Position(respawn_point.position + Vector3.up * 1);
    }

    public ItemData GetItemDataByID(string item_id)
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("ItemData");
        foreach (var data in allItems)
        {
            if (data.item_id == item_id)
            {
                return data;
            }
        }
        return null;
    }

    public int get_item_holding_amount(string item_id)
    {
        //return inventoryItems.GetItemAmount(item_id);
        return inventoryManager.getAmountOfItemByID(item_id);
    }

    public int add_items_to_inventory(string item_id, int amount, bool show_notif = true)
    {
        //inventoryItems.AddToItemAmount(item_id, amount);
        ItemData itemData = GetItemDataByID(item_id);
        int loosePiece = inventoryManager.AddItemToInventory(itemData, amount);
        if (loosePiece >= amount) return -1;
        if (show_notif)
        {
            itemPickupNotifcationScript.ShowItem(itemData, amount);
        }
        // TODO FIX THIS
        if (item_id == "gold_coin")
        {
            SpinningCoin.instance.SpeedUp();
        }
        return loosePiece;
    }

    //TODO
    public void remove_items_from_inventory(string item_id, int amount)
    {
        //add_items_to_inventory(item_id, -amount, false);
        inventoryManager.removeItemsByID(item_id, amount);
    }

    public void Play_Audio_On_Player(int audio_id, float volume = 1f, float pitch = 1f)
    {
        playerAudio.PlayClip(audio_id, volume, pitch);
    }

    public void ShowCustomNotif(string custom_text, float duration = 4)
    {
        itemPickupNotifcationScript.ShowCustomText(custom_text, duration);
    }

    public void PlayerDropItem(string item_id, int item_amount)
    {
        ItemData itemData = GetItemDataByID(item_id);
        print(itemData);
        GameObject spawned_item = Instantiate(itemData.item_pickup_object, PlayerTransform.position + Vector3.up * 1.5f + PlayerTransform.forward, Quaternion.identity);
        spawned_item.GetComponent<ItemPickup>().amount = item_amount;
        Rigidbody spawned_item_rigidbody = spawned_item.GetComponent<Rigidbody>();
        spawned_item_rigidbody.useGravity = true;
        Vector3 direction = PlayerTransform.forward;
        direction = new Vector3(direction.x + UnityEngine.Random.Range(-0.25f, 0.25f), 1f, direction.z + UnityEngine.Random.Range(-0.25f, 0.25f));
        float spawn_force = 5;
        spawned_item_rigidbody.linearVelocity = direction.normalized * spawn_force;
        if (GetComponent<AudioSource>() != null)
        {
            //GetComponent<AudioSource>().PlayOneShot(finished_sound, finished_sound_volume);
        }
    }

    public static void DrawDebugBox(Vector3 center, Vector3 size, Quaternion rotation, Color color)
    {
        Vector3 half = size * 0.5f;

        // Local corners of a unit cube
        Vector3[] localCorners = new Vector3[8]
        {
        new Vector3(-half.x, -half.y, -half.z),
        new Vector3( half.x, -half.y, -half.z),
        new Vector3( half.x, -half.y,  half.z),
        new Vector3(-half.x, -half.y,  half.z),
        new Vector3(-half.x,  half.y, -half.z),
        new Vector3( half.x,  half.y, -half.z),
        new Vector3( half.x,  half.y,  half.z),
        new Vector3(-half.x,  half.y,  half.z),
        };

        // Apply rotation and translate to center
        for (int i = 0; i < localCorners.Length; i++)
            localCorners[i] = center + rotation * localCorners[i];

        // Bottom face
        Debug.DrawLine(localCorners[0], localCorners[1], color);
        Debug.DrawLine(localCorners[1], localCorners[2], color);
        Debug.DrawLine(localCorners[2], localCorners[3], color);
        Debug.DrawLine(localCorners[3], localCorners[0], color);

        // Top face
        Debug.DrawLine(localCorners[4], localCorners[5], color);
        Debug.DrawLine(localCorners[5], localCorners[6], color);
        Debug.DrawLine(localCorners[6], localCorners[7], color);
        Debug.DrawLine(localCorners[7], localCorners[4], color);

        // Vertical edges
        Debug.DrawLine(localCorners[0], localCorners[4], color);
        Debug.DrawLine(localCorners[1], localCorners[5], color);
        Debug.DrawLine(localCorners[2], localCorners[6], color);
        Debug.DrawLine(localCorners[3], localCorners[7], color);
    }

}
