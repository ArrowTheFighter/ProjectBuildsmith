using System.Collections.Generic;
using UnityEngine;

public class FrogBoss : MonoBehaviour
{
    public List<FrogBossPlatform> bossPlatforms = new List<FrogBossPlatform>();
    List<FrogBossPlatform> availableBossPlatforms = new List<FrogBossPlatform>();

    FrogBossPlatform currentTarget;

    Vector3 startPos;
    Vector3 midPos;
    Vector3 endPos;

    float progress;
    public float speed;
    public float height = 2;
    public float jumpHeight = 15;
    public AnimationCurve animationCurve;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetNewPoint();
    }

    // Update is called once per frame
    void Update()
    {
        progress += speed * Time.deltaTime;
        float curvedProgress = animationCurve.Evaluate(progress);
        
        Vector3 pos1 = Vector3.Lerp(startPos,midPos, curvedProgress);
        Vector3 pos2 = Vector3.Lerp(midPos,endPos, curvedProgress);
        transform.position = Vector3.Lerp(pos1,pos2, curvedProgress);

        Quaternion newRotation = Quaternion.LookRotation((endPos - startPos).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation,newRotation,0.2f);

        if (progress >= 1)
        {
            GetNewPoint();
            progress = 0;
        }
    }

    void GetNewPoint()
    {
        if (currentTarget == null)
            currentTarget = bossPlatforms[Random.Range(0, bossPlatforms.Count)];

        startPos = currentTarget.getTopPos() + Vector3.up * height;

        availableBossPlatforms.Clear();
        availableBossPlatforms.AddRange(bossPlatforms);
        availableBossPlatforms.Remove(currentTarget);

        currentTarget = availableBossPlatforms[Random.Range(0, availableBossPlatforms.Count)];
        endPos = currentTarget.getTopPos() + Vector3.up * height;

        midPos = Vector3.Lerp(startPos,endPos,0.5f) + Vector3.up * jumpHeight;
    }
}
