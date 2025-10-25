using UnityEngine;

public class SoundFXManager : MonoBehaviour
{

    [SerializeField] private AudioSource soundFXObject;


    public void PlayRandomSoundCollection(Transform _transform, params AudioCollection[] audioCollection)
    {
        AudioCollection randomCollection = audioCollection[Random.Range(0, audioCollection.Length)];
        PlayAllSoundCollection(_transform, randomCollection);    
    }

    public void PlayAllSoundCollection(Transform _transform, params AudioCollection[] audioCollection)
    {
        foreach (var collection in audioCollection)
        {
            PlaySoundFXClip(collection.audioClip, _transform, collection.audioClipVolume,
            UnityEngine.Random.Range(collection.audioClipPitch * 0.9f, collection.audioClipPitch * 1.1f));
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
