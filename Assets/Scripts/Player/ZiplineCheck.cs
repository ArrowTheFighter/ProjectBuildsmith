using System;
using UnityEngine;
using UnityEngine.Splines;
[Obsolete]
public class ZiplineCheck : MonoBehaviour
{
    [SerializeField] float check_radius = 1f;
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,check_radius);
        bool has_zipline = false;
        foreach(Collider col in  colliders)
        {
            if(col.tag == "Zipline")
            {
                SplineContainer spline = col.GetComponent<SplineContainer>();
                transform.parent.GetComponent<PlayerMovement>().StartZipline(col.transform.up,spline);
                has_zipline = true;
            }
        }
        if(!has_zipline)
        {
            transform.parent.GetComponent<PlayerMovement>().StopZipline();
        }
    }
}
