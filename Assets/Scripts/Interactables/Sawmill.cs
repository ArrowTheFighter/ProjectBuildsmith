using UnityEngine;
using DG.Tweening;

public class Sawmill : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT 
    {
        get
        {
            bool has_items = true;
            foreach (item_requirement item in required_items)
            {
                if (GameplayUtils.instance.get_item_holding_amount(item.item_id) < item.item_amount) has_items = false;
            }
            if (!has_items)
            {
                return outOfWoodString;
            }
            else
            {
                return hasWoodString;
            }

        }
    }

    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    bool CInteract;
    public bool CanInteract { get => CInteract; set { CInteract = value; } }

    private bool sawmillRunning;

    public GameObject log;
    public Transform startPos;
    public Transform endPos;
    public Transform Blade;
    [SerializeField] Transform output_transform;
    [SerializeField] float output_force = 10;
    public Vector3 BladeSpeed;
    float blade_speed_lerp;
    [Header("Items")]
    public GameObject finished_product;
    public float logTime;
    Tween log_spin_tween;
    [SerializeField] Ease LogEasing = Ease.InOutQuad;
    [SerializeField] AudioClip finished_sound;
    [SerializeField] float finished_sound_volume = 1;
    private bool resetPos;

    public string hasWoodString;
    public string outOfWoodString;

    void Start()
    {
        log.transform.position = startPos.position;
    }

    public bool Interact(Interactor interactor)
    {
        foreach (item_requirement item in required_items)
        {
            if (GameplayUtils.instance.get_item_holding_amount(item.item_id) < item.item_amount) return false;
         }
        if (!sawmillRunning)
        {
             foreach (item_requirement item in required_items)
            {
                GameplayUtils.instance.remove_items_from_inventory(item.item_id,item.item_amount);
            }
            sawmillRunning = true;
            log.gameObject.SetActive(true);
            log.transform.DOMove(endPos.position, logTime - 0.1f).From(startPos.position, false).SetEase(LogEasing).OnComplete(FinishedSawing);
            if (log_spin_tween != null && log_spin_tween.IsPlaying())
            {
                log_spin_tween.Kill();
            }
            log_spin_tween = DOVirtual.Float(0, 1, logTime * 0.2f, (context) =>
            {
                blade_speed_lerp = context;
            });
            //Invoke("ResetSawmill", logTime);
        }
        return true;
    }

    public void Update()
    {
        if (Blade != null)
        {
            Blade.Rotate(Mathf.Lerp(0, BladeSpeed.x * Time.deltaTime,blade_speed_lerp),
            Mathf.Lerp(0, BladeSpeed.y * Time.deltaTime,blade_speed_lerp),
            Mathf.Lerp(0, BladeSpeed.z * Time.deltaTime,blade_speed_lerp),Space.Self);    
        }
    }

    void FinishedSawing()
    {
        log_spin_tween = DOVirtual.Float(1, 0, logTime * 0.4f, (context) =>
        {
            blade_speed_lerp = context;
        });
        log.gameObject.SetActive(false);
        log.transform.position = startPos.position;
        sawmillRunning = false;
        if (finished_product != null)
        {
            Transform _output_transform = endPos;
            if (output_transform != null) _output_transform = output_transform;

            GameObject spawned_item = Instantiate(finished_product, _output_transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Rigidbody spawned_item_rigidbody = spawned_item.GetComponent<Rigidbody>();
            spawned_item_rigidbody.useGravity = true;
            Vector3 direction = _output_transform.forward;
            direction = new Vector3(direction.x + Random.Range(-0.25f, 0.25f), 1f, direction.z + Random.Range(-0.25f, 0.25f));
            float spawn_force = output_force;
            spawned_item_rigidbody.linearVelocity = direction.normalized * spawn_force;
            if (GetComponent<AudioSource>() != null)
            {
                GetComponent<AudioSource>().PlayOneShot(finished_sound, finished_sound_volume);
             }
        }
    }

}
