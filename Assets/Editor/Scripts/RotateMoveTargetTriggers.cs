using UnityEngine;
using UnityEditor;

public class RotateMoveTargetTriggers : MonoBehaviour
{
    [MenuItem("Tools/Auto Rotate MoveTargetTrigs")]
    public static void AutoRotateMoveTargetTriggers()
    {
        int undoGroup = Undo.GetCurrentGroup();

        Undo.SetCurrentGroupName("Auto Rotate Move Target Triggers");

        MoveObjectTrigger[] moveObjectTriggers = FindObjectsByType<MoveObjectTrigger>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        foreach(var trigger in moveObjectTriggers)
        {
            if(trigger.NewPos != null)
            {
                Undo.RecordObject(trigger.transform, "Auto Rotate Move Target Triggers");

                Vector3 dirToNewPos = trigger.NewPos.position - trigger.transform.position;
                dirToNewPos.y = 0;
                if(dirToNewPos.sqrMagnitude > 0.001f)
                {
                    trigger.transform.forward = dirToNewPos.normalized;

                    EditorUtility.SetDirty(trigger.transform);
                }
            }
        }
        Undo.CollapseUndoOperations(undoGroup);
    }
}
