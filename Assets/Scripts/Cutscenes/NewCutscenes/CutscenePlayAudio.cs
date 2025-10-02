using UnityEngine;

public class CutscenePlayAudio : MonoBehaviour, ISkippable
{
    public AudioSource audioSource;
    public void Skip()
    {
        audioSource.Stop();
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }
}
