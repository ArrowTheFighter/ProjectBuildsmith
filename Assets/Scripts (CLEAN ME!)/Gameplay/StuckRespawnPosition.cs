using UnityEngine;

public class StuckRespawnPosition : MonoBehaviour
{
    bool is_enabled;
    public void Enable_Respawn_Position()
    {
        if (is_enabled) return;
        print("Enabling respawn point");
        GameplayUtils.instance.add_respawn_point(transform);
        is_enabled = true;
    }
}
