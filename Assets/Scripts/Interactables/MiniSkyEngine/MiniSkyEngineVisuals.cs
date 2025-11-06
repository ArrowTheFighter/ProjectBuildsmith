using UnityEngine;
using DG.Tweening;

public class MiniSkyEngineVisuals : MonoBehaviour
{
    [Header("Main Animator")]
    public Animator animator;

    [Header("Engine Lid")]
    public Transform EngineLid;
    public float EngineLidOpenDuration = 0.25f;
    public Ease EngineLidOpenEase = Ease.InOutQuad;
    bool finished;

    [Header("Gears")]
    public Transform gearA;
    public Transform gearB;

    [Tooltip("Radius of Gear A (used to compute speed ratio)")]
    public float gearA_Radius = 0.247163f;

    [Tooltip("Radius of Gear B (used to compute speed ratio)")]
    public float gearB_Radius = 0.152913f;

    [Tooltip("Degrees per second for Gear A")]
    public float gearA_Speed = 90f;

    [Header("ItemRequirements")]
    public float smoothSpeed = 2f;
    float currentItemRequirementProgress;
    float smoothedCurrentItemProgress;
    public RepairStructure repairStructure;

    [Header("Dials")]
    public Transform[] ItemRequirementDials;

    // Internal
    private float masterAngle = 0f; // cumulative rotation of Gear A
    private Quaternion initialRotationA; // stores initial rotation of Gear A
    private Quaternion initialRotationB; // stores initial rotation of Gear B
    private Vector3 spinAxisA; // correct spin axis for Gear A
    private Vector3 spinAxisB; // correct spin axis for Gear B

    void Start()
    {
        // Store initial rotations
        if (gearA != null)
            initialRotationA = gearA.localRotation;
        if (gearB != null)
            initialRotationB = gearB.localRotation;

        // Determine spin axis based on mesh orientation
        // For example, if your gear rotates around its local Z axis:
        spinAxisA = initialRotationA * Vector3.forward;
        spinAxisB = initialRotationB * Vector3.forward;

        if (repairStructure != null)
        {
            repairStructure.requiredItemPercentUpdated += RequiredItemsUpdated;
            repairStructure.onFinished += repairFinished;
        }


    }

    void repairFinished()
    {
        SetIsFinished(true);
    }
    
    public void SetIsFinished(bool full)
    {
        finished = full;
        if (!full) return;
        EngineLid.DOLocalRotate(new Vector3(0, 0, 0), EngineLidOpenDuration).SetEase(EngineLidOpenEase);
        animator.SetTrigger("Finished");
    }

    void RequiredItemsUpdated(float newPercent)
    {
        currentItemRequirementProgress = newPercent;
        print(currentItemRequirementProgress);
    }

    void Update()
    {
        smoothedCurrentItemProgress = Mathf.Lerp(smoothedCurrentItemProgress, currentItemRequirementProgress, smoothSpeed * Time.deltaTime);
        foreach (var dial in ItemRequirementDials)
        {
            dial.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -180), smoothedCurrentItemProgress);
        }
        if (gearA == null || gearB == null) return;

        // Increment master angle based on speed
        masterAngle += gearA_Speed * Time.deltaTime;

        // Rotate Gear A around its local spin axis
        gearA.localRotation = initialRotationA * Quaternion.AngleAxis(masterAngle, spinAxisA);

        // Rotate Gear B with radius-based speed ratio, opposite direction
        float gearB_Angle = -masterAngle * (gearA_Radius / gearB_Radius);
        gearB.localRotation = initialRotationB * Quaternion.AngleAxis(gearB_Angle, spinAxisB);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (finished) return;
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
            collider.gameObject.CompareTag("Player"))
        {
            EngineLid.DOLocalRotate(new Vector3(-110, 0, 0), EngineLidOpenDuration)
                     .SetEase(EngineLidOpenEase);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (finished) return;
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
            collider.gameObject.CompareTag("Player"))
        {
            EngineLid.DOLocalRotate(new Vector3(0, 0, 0), EngineLidOpenDuration)
                     .SetEase(EngineLidOpenEase);
        }
    }
}
