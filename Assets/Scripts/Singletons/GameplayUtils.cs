using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine.InputSystem;
using CI.PowerConsole;

public class GameplayUtils : MonoBehaviour
{
    public static GameplayUtils instance;

    [SerializeField] public Transform PlayerTransform;
    [SerializeField] MouseLock mouseLock_script;
    [SerializeField] CinemachineInputAxisController cameraInputComponent;
    [SerializeField] public CinemachineBrain cinemachineBrain;
    //[SerializeField] public PlayerMovement playerMovement_script;
    [SerializeField] public InventoryManager inventoryManager;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject[] UI_Canvases;
    [SerializeField] Slider Main_volume_slider;
    [SerializeField] public ItemPickupNotifcationScript itemPickupNotifcationScript;
    [SerializeField] public ItemPickupNotifcationScript centerScreenNotifcationScript;
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
    float pauseMenuCooldown;
    bool open_menu;
    public bool CanPause;
    bool UI_Is_Hidden;
    public RecipeDatabase RecipeDatabase;
    public Dictionary<string, int> ItemsCrafted = new Dictionary<string, int>();
    [Header("DemoEndScreen")]
    public GameObject DemoEndScreenUI;
    public GameObject DemoEndScreenDefaultButton;

    [Header("CollectAllGemsScreen")]
    public GameObject CollectAllGemsScreenUI;
    public GameObject CollectAllGemsScreenButton;

    [Header("MainMenuDefault")]
    public GameObject MainMenuDemoScreenButton;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
        Reset();
        RecipeDatabase = Resources.Load<RecipeDatabase>("Recipes/RecipeDatabase");
        StartCoroutine(InitalFade());
        PowerConsole.Initialise();

        PowerConsole.CommandEntered += CommandCheck;
        //PowerConsole.OpenCloseHotkeys = new List<KeyCode> { KeyCode.LeftControl,KeyCode.BackQuote};

        GameplayInput.instance.playerInput.actions["console"].performed += (context) => { ToggleConsole(); };
        GameplayInput.instance.playerInput.actions["consoleUI"].performed += (context) => { ToggleConsole(); };
    }

    void ToggleConsole()
    {
        print("toggling console");
        if (!PowerConsole.IsVisible)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
        PowerConsole.IsVisible = !PowerConsole.IsVisible;
        PowerConsole.Clear();
    }

    void CommandCheck(object sender, CommandEnteredEventArgs args)
    {
        print(args.Command);
        string[] arguments = args.Command.Split(" ");
        if (arguments.Length <= 0) return;

        switch (arguments[0].ToLower())
        {
            case "giveitem":
                if (arguments.Length > 2)
                {
                    add_items_to_inventory(arguments[1], int.Parse(arguments[2]));
                }
                else if (arguments.Length > 1)
                {
                    add_items_to_inventory(arguments[1], 1);
                }
                break;    
        }
        // string id = callback.Args["-i"];
        // int amount = int.Parse(callback.Args["-a"]);
        // add_items_to_inventory(id, amount);
    }

    void Reset()
    {
        can_use_dialog = true;
        DialogIsOpen = false;
        PauseMenuIsOpen = false;
        open_menu = false;
        UI_Is_Hidden = false;
    }

    void Start()
    {
        Invoke("InitalFade", 0.5f);
        // -- Reset Flags --
        FlagManager.wipe_flag_list();
        //cameraInputComponent = playerMovement_script.transform.GetComponentInChildren<CinemachineInputAxisController>();
        AudioListener.volume = Main_volume_slider.value;
        GameSettings.instance.OnVsyncChanged += SetVsync;
        GameSettings.instance.OnAntiAliasingChanged += SetAntiAliasing;
        GameSettings.instance.OnRenderScaleChanged += SetRenderScale;

        GameplayInput.instance.playerInput.actions["HideUI"].performed += (context) => { ToggleUI(); };

        inventoryManager.OnInventoryUpdated += gemScreenCheck;
        if (MainMenuDemoScreenButton != null)
        {
            UIInputHandler.instance.ClosedMenu();
            UIInputHandler.instance.defaultButton = MainMenuDemoScreenButton;
            UIInputHandler.instance.OpenedMenu();
        }
    }
    IEnumerator InitalFade()
    {
        yield return null;
        FadeUIToBlack fadeUIToBlack = GetComponent<FadeUIToBlack>();
        fadeUIToBlack.SetToBlack();
        yield return new WaitForSecondsRealtime(0.5f);
        fadeUIToBlack.Fade_From_Black();
    }

    void gemScreenCheck()
    {
        if (get_item_holding_amount("gem") >= 20)
        {
            CollectAllGemsScreenUI.SetActive(true);
            OpenMenu();
            UIInputHandler.instance.ClosedMenu();
            UIInputHandler.instance.defaultButton = CollectAllGemsScreenButton;
            UIInputHandler.instance.OpenedMenu();
        }
    }

    public void SetVsync(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }

    public void ToggleUI()
    {
        if (UI_Is_Hidden)
        {
            ShowUI();
        }
        else
        {
            HideUI();
        }
        UI_Is_Hidden = !UI_Is_Hidden;
    }


    public void SetAntiAliasing(bool value)
    {
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        if (urpAsset != null)
        {
            urpAsset.msaaSampleCount = value ? 4 : 1;
        }
    }

    public void SetRenderScale(float value)
    {
        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        if (urpAsset != null)
        {
            urpAsset.renderScale = Mathf.Clamp(value,0.1f,1f);
        }
    }

    public void SetCanPause(bool value)
    {
        CanPause = value;
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
        //if (PauseMenuIsOpen) return false;
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

    public void OpenDemoEndScreen()
    {
        PauseMenuIsOpen = true;
        DemoEndScreenUI.SetActive(true);
        OpenMenu();
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = DemoEndScreenDefaultButton;
        UIInputHandler.instance.OpenedMenu();

        Invoke("OpenDemoEndScreenDelay", 0.1f);
    }

    void OpenDemoEndScreenDelay()
    {
        OpenMenu();
        Time.timeScale = 0;
    }

    public void CloseDemoEndScreen()
    {
        PauseMenuIsOpen = false;
        CloseMenu();
        DemoEndScreenUI.SetActive(false);
        Time.timeScale = 1;
        UIInputHandler.instance.ClosedMenu();
    }

    public bool OpenPauseMenu()
    {
        if (!CanPause) return false;
        if (DialogIsOpen) return false;
        PauseMenuIsOpen = true;
        PauseMenu.SetActive(true);
        PauseMenu.GetComponent<PauseMenu>().OpenMainScreen();
        OpenMenu();
        Time.timeScale = 0;



        pauseMenuCooldown = Time.realtimeSinceStartup + 0.1f;
        return true;
    }


    public void ClosePauseMenu()
    {
        PauseMenuIsOpen = false;
        CloseMenu();
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        pauseMenuCooldown = Time.realtimeSinceStartup + 0.1f;

    }

    public void Toggle_Pause_Menu()
    {

        if (Time.realtimeSinceStartup < pauseMenuCooldown) return;
        if (DialogIsOpen) return;
        if (open_menu && CanPause)
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
        CanPause = true;
        PauseMenuIsOpen = false;
        CloseMenu();

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
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

    public int GetItemCraftedAmount(string item_id)
    {
        if (ItemsCrafted.ContainsKey(item_id))
        {
            return ItemsCrafted[item_id];
        }
        return 0;
     }

    public void AddItemCraftedAmount(string item_id, int amount)
    {
        if (ItemsCrafted.ContainsKey(item_id))
        {
            ItemsCrafted[item_id] += amount;
        }
        ItemsCrafted[item_id] = amount;
        inventoryManager.QuestsObjectiveCheck();
    }

    public int get_item_holding_amount(string item_id)
    {
        //return inventoryItems.GetItemAmount(item_id);
        int specialItemAmount = inventoryManager.GetSpecialItemAmount(item_id);
        if (specialItemAmount != -1) return specialItemAmount;
        return inventoryManager.getAmountOfItemByID(item_id);
    }

    public int add_items_to_inventory(string item_id, int amount, bool show_notif = true,bool playSound = false)
    {
        //inventoryItems.AddToItemAmount(item_id, amount);
        ItemData itemData = GetItemDataByID(item_id);
        int loosePiece = inventoryManager.AddItemToInventory(itemData, amount);
        if (loosePiece >= amount) return -1;
        if (show_notif)
        {
            itemPickupNotifcationScript.ShowItem(itemData, amount);
        }
        else if (playSound)
        {
            itemPickupNotifcationScript.PlayNotificationSound();
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

    public void ShowCustomNotifCenter(string custom_text,float duration = 4)
    {
        centerScreenNotifcationScript.ShowCustomText(custom_text, duration);
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
