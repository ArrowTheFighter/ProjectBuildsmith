using UnityEngine;
using DG.Tweening;

public class WindballTrigger : MonoBehaviour
{
    [SerializeField] private Transform[] windballLocations;
    [SerializeField] private float tweenMoveTime = 3f;
    private int windballStatus = 0;
    [SerializeField] private Collider windballTrigger;
    [SerializeField] private float scaleDownTime = 1f;
    [SerializeField] private string flag;

    [SerializeField] AudioClip windballMovedSoundFX;
    [SerializeField] float windballMovedSoundFXVolume = 1f;
    [SerializeField] float windballMovedSoundFXPitch = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            windballTrigger.enabled = false;
            windballStatus++;
            if (windballLocations[windballStatus - 1] != null)
            {
                ActivateNextWindball();
            }
                
        }
    }

    private void ActivateNextWindball()
    {
        ScriptRefrenceSingleton.instance.soundFXManager.PlaySoundFXClip(windballMovedSoundFX, transform, windballMovedSoundFXVolume, windballMovedSoundFXPitch);

        gameObject.transform.DOMove(windballLocations[windballStatus - 1].position, tweenMoveTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                windballTrigger.enabled = true;
                RemoveWindball();
            });
    } 

    private void RemoveWindball()
    {
        if (windballStatus == windballLocations.Length)
        {
            FlagManager.Set_Flag(flag);
            gameObject.transform.DOScale(Vector3.zero, scaleDownTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}