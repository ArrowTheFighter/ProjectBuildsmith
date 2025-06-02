using UnityEngine;
using UnityEditor;

public class RandomizeScale : MonoBehaviour
{
   [MenuItem("Tools/Randomize Scale")]
    static void RandomizeObjectsScale()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Randomize Object Scale");
            Vector3 scale = Vector3.one * Random.Range(2f, 3f);
            obj.transform.localScale = scale;
        }
    }
}
