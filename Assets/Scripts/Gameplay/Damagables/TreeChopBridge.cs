using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class TreeChopBridge : MonoBehaviour, IDamagable
{
    public List<GameObject> treeStages = new List<GameObject>();
    public int segmentHealth;

    public List<GameObject> finalStage = new List<GameObject>();

    public UnityEvent finishedEvent;

    public ParticleSystem chopParticle;

    public bool PlayerCanStomp { get; set; }

    int currentStage;
    float currentHealth;
    bool finished;

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if (finished) return;
        if (attackTypes.Contains(AttackType.Chop))
        {
            currentHealth -= amount;
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
}
