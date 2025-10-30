using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScriptRefrenceSingleton : MonoBehaviour
{
    public static ScriptRefrenceSingleton instance;

    public GameplayUtils gameplayUtils;

    public SoundFXManager soundFXManager;

    public CutsceneManager cutsceneManager;

    public TimeManager timeManager;

    public ItemRespawnManager itemRespawnManager;

    public DialogManager dialogManager;

    public PlayerAudioManager playerAudioManager;

    public PlayerParticlesManager playerParticlesManager;

    public GameplayInput gameplayInput;

    public GameSettings gameSettings;

    public SaveLoadManager saveLoadManager;

    public SpinningCoin spinningCoin;

    public UIIconHandler uIIconHandler;

    public UIInputHandler uIInputHandler;

    public HotbarManager hotbarManager;

    public ItemTitlePopupManager itemTitlePopupManager;

    public CompassScript compassScript;

    static bool notFirstTimeStarted;

    public static bool is_ready;

    [Header("Hide Demo Start Screen")]
    public UnityEvent OnNotFirstTimeStartingEvent;

    public static event Action OnScriptLoaded;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            print("destorying old instance");
            instance.Cleanup();
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    void Start()
    {
        if (notFirstTimeStarted)
        {
            print("running not first time playing event");
            OnNotFirstTimeStartingEvent?.Invoke();
        }
        notFirstTimeStarted = true;

        is_ready = true;
        OnScriptLoaded?.Invoke();

    }

    public void Cleanup()
    {
        OnScriptLoaded = null;
    }

    [Button]
    public void ResetScene()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }

    public static IEnumerator Wait_Until_Script_is_Ready()
    {
        yield return new WaitUntil(() => is_ready);
    }
}
