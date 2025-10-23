using UnityEngine;

public class EnableObject : MonoBehaviour
{
    [SerializeField] GameObject[] Objects_To_enable;
    [SerializeField] GameObject[] Objects_To_disbale;

    public void Enable_Objects()
    {
        foreach (GameObject obj in Objects_To_enable)
        {
            obj.SetActive(true);
        }
    }

    public void Disbale_Objects()
    {
        foreach (GameObject obj in Objects_To_disbale)
        {
            obj.SetActive(false);
        }
    }

    
}
