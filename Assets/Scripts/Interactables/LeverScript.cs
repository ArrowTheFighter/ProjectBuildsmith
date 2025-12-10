using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class LeverScript : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_Requirements;
    public item_requirement[] required_items => item_Requirements;

    public bool isInteractable = true;
    public bool CanInteract { get => isInteractable; set {isInteractable = value;} }

    bool isOn;

    public UnityEvent LeverPulled;

    public Animator animator;

    public bool Interact(Interactor interactor)
    {
        if(!isInteractable) return false;
        ToggleLever();
        return true;
    }


    void ToggleLever()
    {
        if(!isOn) TurnOn();
        else TurnOff();
    }

    public void TurnOn()
    {
        isOn = true;
        LeverPulled?.Invoke();

        //Play animation here
        animator.Play("TurnOn");
    }

    public void TurnOff()
    {
        isOn =false;

        //Play animation here
        animator.Play("TurnOff");
    }
   
}
