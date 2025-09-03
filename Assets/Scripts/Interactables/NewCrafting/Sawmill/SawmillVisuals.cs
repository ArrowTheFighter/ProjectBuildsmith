using UnityEngine;

public class SawmillVisuals : MonoBehaviour
{
    CraftingProcessor craftingProcessor;
    public Transform BladeTransform;
    float bladeSpeed;
    public float bladeMaxSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        craftingProcessor = GetComponent<CraftingProcessor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (craftingProcessor.isCrafting)
        {
            bladeSpeed = Mathf.Lerp(bladeSpeed, 1, 0.1f);
        }
        else
        {
            bladeSpeed = Mathf.Lerp(bladeSpeed, 0, 0.1f);
        }

        if (BladeTransform != null)
        {
            BladeTransform.Rotate(0, 0, bladeMaxSpeed * bladeSpeed * Time.deltaTime, Space.Self);
         }
    }
}
