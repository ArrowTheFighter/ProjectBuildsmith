using System.Linq;
using UnityEngine;

public class CutscenePlayAudio : MonoBehaviour, ISkippable
{
    public AudioSource audioSource;
    public AudioCollection[] AudioList;
    public void Skip()
    {
        audioSource.Stop();
    }

    public void PlayAudio()
    {
        if(AudioList.Length > 0)
        {
            ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, AudioList);
        }
        else
        {
            audioSource.Play();
        }
    }
}
