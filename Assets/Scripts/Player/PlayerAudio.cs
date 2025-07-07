using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;

    public void PlayClip(int clip_id,float volume_adjustment = 1f,float pitchBase = 1f)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.pitch = Random.Range(pitchBase - 0.1f,pitchBase + 0.1f);
        if (clip_id > audioClips.Length - 1) return;
        audioSource.PlayOneShot(audioClips[clip_id],AudioListener.volume * volume_adjustment);
    }
}
