using UnityEngine;

public class EnableObject : MonoBehaviour
{
    [SerializeField] GameObject[] Objects_To_enable;

    public void Enable_Objects()
    {
        foreach (GameObject obj in Objects_To_enable)
        {
            obj.SetActive(true);
         }
     }
}
