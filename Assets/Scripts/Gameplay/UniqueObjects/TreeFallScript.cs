using UnityEngine;

public class TreeFallScript : MonoBehaviour
{

    public void PlayTreeFallAnimation()
    {
        Animation animation = GetComponent<Animation>();
        animation.Play();
     }
}
