using System;
using UnityEngine;
using UnityEngine.Events;

public class SkyEssencePickup : MonoBehaviour, ISaveable
{
    public SkyEngineScript SkyEngine;
    public ParticleSystem touchedVFX;
    public bool FlyTowardsEngine;
    public Vector3 SkyEngineOffset;
    public float maxSpeed;
    float speed;
    public UnityEvent onTouched;

    [Header("Audio")]
    [SerializeField] AudioClip essencePickupSoundFX;
    [SerializeField] float essencePickupSoundFXVolume = 1f;
    [SerializeField] float essencePickupSoundFXPitch = 1f;

    public AudioSource SkyEssenceAmbiantSource;

    public int unique_id;
    bool collected;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => collected;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FlyTowardsEngine) return;
            touchedVFX.Play();

            if (essencePickupSoundFX != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(essencePickupSoundFX, transform, essencePickupSoundFXVolume, essencePickupSoundFXPitch);
            }

            FlyTowardsEngine = true;
            collected = true;
            if (GetComponent<FloatingItem>() != null)
            {
                GetComponent<FloatingItem>().StopAnimating();
            }
            onTouched?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (FlyTowardsEngine)
        {
            Vector3 dir = SkyEngine.transform.position + SkyEngineOffset - transform.position;
            transform.position += dir.normalized * speed * Time.fixedDeltaTime;
            speed = Mathf.Lerp(speed, maxSpeed, 0.1f);
            if (Vector3.Distance(transform.position, SkyEngine.transform.position + SkyEngineOffset) < 2.5f)
            {
                SkyEngine.CollectEssence();
                gameObject.SetActive(false);
            }
         }
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        collected = true;
        SkyEngine.LoadCollectedEssence();
        gameObject.SetActive(false);
    }
}