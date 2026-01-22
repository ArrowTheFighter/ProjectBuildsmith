using UnityEngine;

public interface IPlatformPassenger
{
    public IPlatformPassenger GetPassengerScript();
    public void BeforePlatformMove(IMoveingPlatform moveingPlatform);
}
