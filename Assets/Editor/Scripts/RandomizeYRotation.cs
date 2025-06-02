using UnityEngine;
using UnityEditor;

public class RandomizeYRotation : MonoBehaviour
{
    [MenuItem("Tools/Randomize Y Rotation")]
    static void RandomizeY()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Randomize Y Rotation");
            Vector3 rot = obj.transform.eulerAngles;
            rot.y = Random.Range(0f, 360f);
            obj.transform.eulerAngles = rot;
        }
    }
}
