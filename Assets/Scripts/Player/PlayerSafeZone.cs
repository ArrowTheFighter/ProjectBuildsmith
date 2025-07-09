using UnityEngine;

public class PlayerSafeZone : MonoBehaviour
{
    CharacterMovement characterMovement;
    public Vector3 safePos;

    void Start()
    {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    void Update()
    {
        LayerMask layerMask = ~characterMovement.IgnoreGroundLayerMask;
        if (!Physics.Raycast(transform.position, Vector3.down, 1.25f, layerMask)) return;
        if (!Physics.Raycast(transform.position + transform.forward, Vector3.down, 1.25f, layerMask)) return;
        if (!Physics.Raycast(transform.position - transform.forward, Vector3.down, 1.25f, layerMask)) return;

        if (!Physics.Raycast(transform.position + transform.right, Vector3.down, 1.25f, layerMask)) return;
        if (!Physics.Raycast(transform.position - transform.right, Vector3.down, 1.25f, layerMask)) return;
        //print("on a safe spot");
        safePos = transform.position;
    }
}
