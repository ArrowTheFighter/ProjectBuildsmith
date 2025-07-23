using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float pitch)
    {
        //Spawn in gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign the audioClip
        audioSource.clip = audioClip;

        //Assign volume
        audioSource.volume = volume;

        //Assign pitch
        audioSource.pitch = pitch;

        //Play sound
        audioSource.Play();

        //Get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //Destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume, float pitch)
    {
        //Assign a random index
        int rand = Random.Range(0, audioClip.Length);

        //Spawn in gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign the audioClip
        audioSource.clip = audioClip[rand];

        //Assign volume
        audioSource.volume = volume;

        //Assign pitch
        audioSource.pitch = pitch;

        //Play sound
        audioSource.Play();

        //Get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //Destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
