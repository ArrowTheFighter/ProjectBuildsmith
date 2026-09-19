using UnityEngine;
using System;
using System.Collections.Generic;

public class FakeMovingPlatformChild : MonoBehaviour, IMoveingPlatform
{
    [Header("Fake Parent")]
    [SerializeField] private Transform fakeParent;

    private Vector3 localPosition;
    private readonly HashSet<IPlatformPassenger> passengers = new();

    public event Action OnBeforePlatformMove;

    private void Awake()
    {
        if (fakeParent == null)
        {
            Debug.LogError($"{name}: Fake Parent is not assigned.", this);
            enabled = false;
            return;
        }

        // Store our position relative to the fake parent.
        localPosition = fakeParent.InverseTransformPoint(transform.position);
    }

    private void FixedUpdate()
    {
        if (fakeParent == null)
            return;

        // Calculate where we should be based on the fake parent's
        // current position AND rotation.
        Vector3 newPosition = fakeParent.TransformPoint(localPosition);

        // Don't send movement events if we haven't actually moved.
        if (newPosition == transform.position)
            return;

        // Tell all passengers BEFORE we move.
        foreach (var passenger in passengers)
        {
            passenger.BeforePlatformMove(this);
        }

        OnBeforePlatformMove?.Invoke();

        // Move to the new position.
        // We intentionally DO NOT change our rotation.
        transform.position = newPosition;
    }

    public Transform getInterfaceTransform()
    {
        return transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            passengers.Add(passenger.GetPassengerScript());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            passengers.Remove(passenger.GetPassengerScript());
        }
    }

    public void SetFakeParent(Transform parent)
    {
        fakeParent = parent;

        if (fakeParent != null)
        {
            localPosition = fakeParent.InverseTransformPoint(transform.position);
        }
    }

    public void ResetLocalPosition()
    {
        if (fakeParent != null)
        {
            localPosition = fakeParent.InverseTransformPoint(transform.position);
        }
    }
}