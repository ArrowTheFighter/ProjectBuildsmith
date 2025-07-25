using UnityEngine;
using DG.Tweening;
using UnityEditor.ShaderGraph;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] float vertacleDistance;
    [SerializeField] float duration;
    [SerializeField] float rotationMultiplier;
    Vector3 rotaionVel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOMoveY(transform.position.y + vertacleDistance, duration).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
