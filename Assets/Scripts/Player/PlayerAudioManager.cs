using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public List<AudioCollection> audioSystems = new List<AudioCollection>();

    public AudioCollection GetAudioClipByID(string audioClip)
    {
        foreach (AudioCollection collection in audioSystems)
        {
            if (collection.AudioID == audioClip)
            {
                return collection;
            }
        }

        return null;
    }
}

[Serializable]
public class AudioCollection
{
    public string AudioID;
    public AudioClip audioClip;
    public float audioClipVolume;
    public float audioClipPitch;
}
