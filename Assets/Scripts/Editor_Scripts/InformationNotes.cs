using UnityEngine;

[AddComponentMenu("Miscellaneous/README Info Note")]
public class InformationNotes : MonoBehaviour
{
    // Do NOT make any changes to this script or it may delete all of our notes!!!
    [TextArea(10, 1000)] public string Comment = "Information Here.";
}