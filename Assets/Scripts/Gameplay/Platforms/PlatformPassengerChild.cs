using UnityEngine;

public class PlatformPassengerChild : MonoBehaviour, IPlatformPassenger
{
    public Transform MainPlatformPassengerTransform;
    public IPlatformPassenger mainPlatformPassenger;

    void Start()
    {
        mainPlatformPassenger = MainPlatformPassengerTransform.GetComponent<IPlatformPassenger>();    
    }

    public void BeforePlatformMove(IMoveingPlatform moveingPlatform)
    {
        mainPlatformPassenger.BeforePlatformMove(moveingPlatform);
    }

    public IPlatformPassenger GetPassengerScript()
    {
        return mainPlatformPassenger;
    }
}
