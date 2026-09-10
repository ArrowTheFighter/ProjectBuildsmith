using TMPro;
using UnityEngine;

public class FrogBossBullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifetime = 10;
    [SerializeField] Renderer beamRenderer;
    Material mat;
    bool hitPlayer = false;
    float spawnTime;

    void Awake()
    {
        spawnTime = Time.time;
    }

    void Start()
    {
        mat = beamRenderer.material;
    }
    // Update is called once per frame
    void Update()
    {   
        mat.SetFloat("_fade",Mathf.Min((Time.time - spawnTime) * 2,1));
        transform.position += transform.forward * speed * Time.deltaTime;
        if(Time.time - lifetime > spawnTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hitPlayer) return;
        if(other.tag == "Player")
        {
            print($"Hitting Player! {other.transform.parent.name}");
            
            other.GetComponentInParent<IDamagable>().TakeDamage(1, new[] {AttackType.Simple},gameObject,1);
            hitPlayer = true;
        }
    }
}
