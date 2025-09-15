using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void Open(string url)
    {
        Application.OpenURL(url);
    }
}
