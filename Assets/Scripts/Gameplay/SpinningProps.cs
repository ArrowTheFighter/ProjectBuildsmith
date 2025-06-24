using System;
using UnityEngine;

[Obsolete("This is an outdated script, use 'RotatingObject' instead")]
public class SpinningProps : MonoBehaviour
{
    public float rotateSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
