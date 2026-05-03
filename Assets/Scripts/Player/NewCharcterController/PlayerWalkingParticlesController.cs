using UnityEngine;

public class PlayerWalkingParticlesController : MonoBehaviour
{
    [SerializeField] CharacterMovement characterMovement;
    [SerializeField] ParticleSystem walkingParticles;
    float lastTimeParticleSpawned;
    [SerializeField] float timeBetweenSpawningParticle;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 HorVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);
        float speed_blend = Mathf.Clamp01(Mathf.InverseLerp(0, characterMovement.maxSpeed, HorVel.magnitude));
        if (characterMovement.grounded && speed_blend > 0.5f)
        {
            if(lastTimeParticleSpawned + timeBetweenSpawningParticle < Time.time)
            {
                walkingParticles.Play();
                lastTimeParticleSpawned = Time.time;
            }
        }
    }
}
