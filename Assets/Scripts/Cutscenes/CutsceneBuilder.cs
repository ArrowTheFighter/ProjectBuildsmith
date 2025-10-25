using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CutsceneBuilder : MonoBehaviour
{
    [SerializeField] Transform CameraTransform;
    [SerializeField] Camera cutscene_camera;
    [SerializeField] Transform start_pos;
    [SerializeField] Transform end_pos;
    [SerializeField] Ease cutscene_ease = Ease.InOutSine;
    [SerializeField] float duration;
    [SerializeField] bool hide_dialog = true;
    [SerializeField] bool hide_ui = true;
    [SerializeField] UnityEvent endCutsceneEvent;

    [ContextMenu("StartCutscene")]
    public void StartCutscene()
    {
        if (CameraTransform == null) CameraTransform = cutscene_camera.transform.parent.transform;
        cutscene_camera.gameObject.SetActive(true);
        CameraTransform.DORotate(end_pos.transform.eulerAngles, duration).From(start_pos.transform.eulerAngles).SetEase(cutscene_ease);
        CameraTransform.DOMove(end_pos.position, duration).From(start_pos.position).SetEase(cutscene_ease).OnComplete(Finish_Cutscene);
        cutscene_camera.depth = 5;
        if (hide_ui)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.HideUI();
        }
        if (hide_dialog)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.set_can_use_dialog(false);
            ScriptRefrenceSingleton.instance.dialogManager.Hide_Dialog();
        }
        else
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.Freeze_Player();
        }
    }

    void Finish_Cutscene()
    {
        cutscene_camera.depth = -1;
        cutscene_camera.gameObject.SetActive(true);

        if (hide_ui)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.ShowUI();
        }
        if (hide_dialog)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.set_can_use_dialog(true);
            ScriptRefrenceSingleton.instance.dialogManager.Show_Dialog();
        }
        else
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.Unfreeze_Player();
        }
        endCutsceneEvent.Invoke();

    }

}
