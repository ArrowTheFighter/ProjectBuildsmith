using UnityEngine;
using UnityEngine.Events;

public class ActivateAfterXAmountOfTimes : MonoBehaviour
{
    public int RequiredAmount;
    int currentAmount;

    public UnityEvent Activate;
    bool activated;


    public void InscreaseAmount()
    {
        currentAmount++;
        if (currentAmount >= RequiredAmount && !activated)
        {
            Activate?.Invoke();
            activated = true;
        }
    }
}
