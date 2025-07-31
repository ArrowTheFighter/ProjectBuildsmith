using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
    public GameObject objectToDestroy;
    public ParticleSystem destroyParticles;
    public void DestroyObject()
    {
        if (destroyParticles != null)
        {
            destroyParticles.Play();
        }
        Destroy(objectToDestroy);
    }
}
