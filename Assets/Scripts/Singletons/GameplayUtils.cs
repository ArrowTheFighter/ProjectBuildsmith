using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameplayUtils : MonoBehaviour
{
    public static GameplayUtils instance;

    [SerializeField] MouseLock mouseLock_script;
    CinemachineInputAxisController cameraInputComponent;
    [SerializeField] public PlayerMovement playerMovement_script;
    [SerializeField] InventoryItems inventoryItems;
    [SerializeField] GameObject inventory;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject[] UI_Canvases;
    [SerializeField] Slider Main_volume_slider;
    [SerializeField] ItemPickupNotifcationScript itemPickupNotifcationScript;
    [SerializeField] PlayerAudio playerAudio;
    [SerializeField] UnityEvent InitalCutsceneEvent;
    [SerializeField] CutsceneBuilder InitialCutscene;
    [SerializeField] CanvasGroup black_fade_cover;
    public List<Transform> respawnPoints = new List<Transform>();
    public bool can_use_dialog = true;
    bool open_menu;
    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    void Start()
    {
        cameraInputComponent = playerMovement_script.transform.GetComponentInChildren<CinemachineInputAxisController>();
        AudioListener.volume = Main_volume_slider.value;
        if (SceneManagerUtils.instance != null)
        {
            print(SceneManagerUtils.instance.ShowStartCutscene);
            if (SceneManagerUtils.instance.ShowStartCutscene)
            {
                InitalCutsceneEvent?.Invoke();
                black_fade_cover.DOFade(0, 5).From(2);
            }
        }
    }

    public void OpenMenu()
    {
        if (open_menu) return;
        mouseLock_script.Release_Mouse();
        cameraInputComponent.enabled = false;
        playerMovement_script.can_control_player = false;
        open_menu = true;
    }

    public void CloseMenu()
    {
        if (!open_menu) return;
        mouseLock_script.Capture_Mouse();
        cameraInputComponent.enabled = true;
        playerMovement_script.can_control_player = true;
        open_menu = false;
    }

    public void Freeze_Player(bool release_mouse = false)
    {
        if (release_mouse) mouseLock_script.Release_Mouse();
        cameraInputComponent.enabled = false;
        playerMovement_script.can_control_player = false;
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
        playerMovement_script.can_control_player = true;
    }

    public void OpenPauseMenu()
    {
        OpenMenu();
        PauseMenu.SetActive(true);
        PauseMenu.GetComponent<PauseMenu>().OpenMainScreen();
        Time.timeScale = 0;
    }

    public void set_can_use_dialog(bool can_use)
    {
        can_use_dialog = can_use;
    }


    public void ClosePauseMenu()
    {
        CloseMenu();
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Toggle_Pause_Menu()
    {
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
        Transform player_transform = playerMovement_script.transform;
        Transform respawn_point = get_closest_respawn_point(player_transform.position);
        if (respawn_point == null) return;
        Time.timeScale = 1;
        ClosePauseMenu();
        playerMovement_script.Set_Player_Position(respawn_point.position + Vector3.up * 1);
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
        return inventoryItems.GetItemAmount(item_id);
    }

    public void add_items_to_inventory(string item_id, int amount, bool show_notif = true)
    {
        inventoryItems.AddToItemAmount(item_id, amount);
        if (show_notif)
        {
            ItemData itemData = GetItemDataByID(item_id);
            itemPickupNotifcationScript.ShowItem(itemData, amount);
        }
        if (item_id == "gold_coin")
        {
            SpinningCoin.instance.SpeedUp();
        }
    }

    public void remove_items_from_inventory(string item_id, int amount)
    {
        add_items_to_inventory(item_id, -amount, false);
    }

    public void Play_Audio_On_Player(int audio_id, float volume = 1f, float pitch = 1f)
    {
        playerAudio.PlayClip(audio_id, volume, pitch);
    }

    public void ShowCustomNotif(string custom_text)
    {
        itemPickupNotifcationScript.ShowCustomText(custom_text);
    }
}
