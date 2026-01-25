using UnityEngine;

public class EnableParticleWhenPlayerIsNearby : MonoBehaviour
{
    public float playerRange = 30f;
    public float searchInterval = 5f;
    ParticleSystem ps;
     
    Transform playerTransform;
    Coroutine routine;
    
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        playerTransform = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform;

        routine = StartCoroutine(
            DistanceCoroutines.DistanceCheck(
                transform,
                playerTransform,
                playerRange,
                onEnter: PlayerEntered,
                onExit: PlayerExited,
                searchInterval));
    }

    void PlayerEntered()
    {
        ps.Play();
    }

    void PlayerExited()
    {
        ps.Stop();
    }

}
