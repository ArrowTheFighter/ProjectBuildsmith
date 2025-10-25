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

    static bool notFirstTimeStarted;

    [Header("Hide Demo Start Screen")]
    public UnityEvent OnNotFirstTimeStartingEvent;

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
        if (notFirstTimeStarted)
        {
            print("running not first time playing event");
            OnNotFirstTimeStartingEvent?.Invoke();
        }
        notFirstTimeStarted = true;
    }

    [Button]
    public void ResetScene()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }
}
