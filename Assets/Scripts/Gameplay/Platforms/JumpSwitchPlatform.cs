using UnityEngine;
using DG.Tweening;

public class JumpSwitchPlatform : MonoBehaviour
{
    [SerializeField] bool StartSpawned = true;
    bool isSpawned;
    [HideInInspector] public bool platformEnabled = true;
    [SerializeField] float scaleDuration = 0.5f;
    Vector3 startScale;
    
    void Start()
    {
        startScale = transform.localScale;
        ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.GetComponent<CharacterMovement>().OnJump += PlayerJumped;
        if(!StartSpawned)
        {
            TogglePlatform();
        }

    }

    void PlayerJumped()
    {
        if (!platformEnabled) return;
        TogglePlatform();
    }

    void TogglePlatform()
    {
        if (isSpawned)
        {
            ScaleOut();
        }
        else
        {
            ScaleIn();
        }
        isSpawned = !isSpawned;
    }

    void ScaleOut()
    {
        transform.DOScale(Vector3.zero,scaleDuration);
    }

    void ScaleIn()
    {
        transform.DOScale(startScale, scaleDuration);
    }
}
