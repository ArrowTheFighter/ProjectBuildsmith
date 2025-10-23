using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class TreeChopBridge : MonoBehaviour, IDamagable,ISaveable
{
    public List<GameObject> treeStages = new List<GameObject>();
    public int segmentHealth;

    public List<GameObject> finalStage = new List<GameObject>();

    public UnityEvent finishedEvent;

    public ParticleSystem chopParticle;

    public bool PlayerCanStomp { get; set; }

    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => finished;

    public AudioCollection[] TreeHitAudioCollection;
    public AudioCollection[] TreeChoppedAudioCollection;

    int currentStage;
    float currentHealth;
    bool finished;

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if (finished) return;
        if (attackTypes.Contains(AttackType.Chop))
        {
            currentHealth -= amount;
            SoundFXManager.instance.PlayRandomSoundCollection(transform, TreeHitAudioCollection);
            if (currentHealth <= 0)
            {
                if (currentStage < treeStages.Count - 1)
                {
                    treeStages[currentStage].SetActive(false);
                    currentStage++;
                    treeStages[currentStage].SetActive(true);
                    currentHealth = segmentHealth;
                }
                else
                {
                    treeStages[currentStage].SetActive(false);
                    foreach (GameObject obj in finalStage)
                    {
                        obj.SetActive(true);
                    }
                    finishedEvent?.Invoke();
                    finished = true;
                    print("finished");

                    SoundFXManager.instance.PlayRandomSoundCollection(transform, TreeChoppedAudioCollection);
                }
            }
            
            if (chopParticle != null)
            {
                chopParticle.Play();
            }
        }


    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = 0;
        TakeDamage(amount, attackTypes, source);
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        TakeDamage(amount, attackTypes, source);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = segmentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        foreach (var stage in treeStages)
        {
            stage.gameObject.SetActive(false);
        }
        foreach (GameObject obj in finalStage)
        {
            obj.SetActive(true);
        }
        finished = true;
        if (finishedEvent != null)
        {
            int count = finishedEvent.GetPersistentEventCount();
            for (int i = 0; i < count; i++)
            {
                var target = finishedEvent.GetPersistentTarget(i) as ISkippable;
                if (target != null)
                {
                    target.Skip();
                }
            }
        }
        InvokeNonSkippableListeners(finishedEvent);
    }

    void InvokeNonSkippableListeners(UnityEvent evt)
    {
        if (evt == null) return;

        int count = evt.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            var target = evt.GetPersistentTarget(i);
            var methodName = evt.GetPersistentMethodName(i);

            // Skip ISkippable objects
            if (target is ISkippable)
                continue;

            // Invoke this specific listener
            if (target != null && !string.IsNullOrEmpty(methodName))
            {
                var method = target.GetType().GetMethod(methodName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    // For base UnityEvent, all methods should be parameterless
                    method.Invoke(target, null);
                }
            }
        }
    }
}
