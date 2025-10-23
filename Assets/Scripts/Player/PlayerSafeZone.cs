using UnityEngine;

public class PlayerSafeZone : MonoBehaviour
{
    public CharacterMovement characterMovement;
    public LayerMask safeLayers;
    public Vector3 safePos;
    public Vector3 checkOffset;

    void Start()
    {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    void Update()
    {
        LayerMask layerMask = safeLayers;
        if (!Physics.Raycast(transform.position + checkOffset, Vector3.down, 1.25f, layerMask)) return;
        if (!Physics.Raycast(transform.position + checkOffset + transform.forward, Vector3.down, 1.25f, layerMask)) return;
        if (!Physics.Raycast(transform.position + checkOffset - transform.forward, Vector3.down, 1.25f, layerMask)) return;

        if (!Physics.Raycast(transform.position + checkOffset + transform.right, Vector3.down, 1.25f, layerMask)) return;
        if (!Physics.Raycast(transform.position + checkOffset - transform.right, Vector3.down, 1.25f, layerMask)) return;
        //print("on a safe spot");
        safePos = transform.position;
    }
}
