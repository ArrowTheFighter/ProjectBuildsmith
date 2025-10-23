using UnityEngine;

public class ObjectDestroy : MonoBehaviour, ISkippable
{
    public GameObject objectToDestroy;
    public ParticleSystem destroyParticles;
    public void DestroyObject()
    {
        if (destroyParticles != null)
        {
            destroyParticles.Play();
        }
        if (objectToDestroy != null)
        {
            objectToDestroy.SetActive(false);
        }
    }

    public void Skip()
    {
        if (objectToDestroy != null)
        {
            objectToDestroy.SetActive(false);
        }
    }
}
