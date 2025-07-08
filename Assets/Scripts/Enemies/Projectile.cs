using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float lifeSpan = 5f;

    private void Awake()
    {
        Invoke("DestroyProjectile", lifeSpan);
    }

    private void Update()
    {
        transform.position += -Vector3.forward * movementSpeed * Time.deltaTime;
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Needs some other code here to damage the player, play a sound, play a "destroy" particle or something
            Destroy(gameObject);
        }

        //Should probably check to see if it hits any other objects to get destroyed too
    }
}
