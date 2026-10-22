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
        Debug.Log($"New amount is {currentAmount}");
        if (currentAmount >= RequiredAmount && !activated)
        {
            Activate?.Invoke();
            activated = true;
        }
    }

    public void DecreaseAmount()
    {
        currentAmount--;
     }
}
