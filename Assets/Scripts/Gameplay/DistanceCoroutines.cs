using UnityEngine;
using System;
using System.Collections;

public static class DistanceCoroutines
{
    public static IEnumerator DistanceCheck(
        Transform a,
        Transform b,
        float distance,
        Action onEnter,
        Action onExit,
        float interval = 0.2f)
    {
        bool inside = false;
        float distSqr = distance * distance;

        while (true)
        {
            if (a == null || b == null)
                yield break;

            float d = (a.position - b.position).sqrMagnitude;

            if (d <= distSqr)
            {
                if (!inside)
                {
                    inside = true;
                    onEnter?.Invoke();
                }
            }
            else
            {
                if (inside)
                {
                    inside = false;
                    onExit?.Invoke();
                }
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
