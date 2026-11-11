using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerCheckpointPosition : MonoBehaviour
{
    public Vector3 pos {get; private set; }
    public bool UseCheckpoints = true;

    void Start()
    {
        pos = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint" && UseCheckpoints)
        {
            SetCheckpointPosition(other.transform.position + Vector3.up);
        }
    }

    public void SetCheckpointPosition(Vector3 _pos)
    {
        pos = _pos;
    }

    public void SetPlayerToCheckpointPosition()
    {
        transform.position = pos;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        StartCoroutine(DelayPlayerFreeze());
    }

    public void MovePlayerToCheckpointSmoothly(float duration = 1)
    {
        transform.DOMove(pos, duration).SetEase(Ease.InOutQuad);
    }

    IEnumerator DelayPlayerFreeze()
    {
        yield return null;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}
