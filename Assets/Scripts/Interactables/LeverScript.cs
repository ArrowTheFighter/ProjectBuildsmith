using UnityEngine;
using UnityEngine.Events;

public class LeverScript : MonoBehaviour, IInteractable, ISaveable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_Requirements;
    public item_requirement[] required_items => item_Requirements;

    public bool isInteractable = true;
    public bool CanInteract { get => isInteractable; set {isInteractable = value;} }


    bool isOn;
    public bool ActivateWhenOnAndOff;
    public bool onlyOnce;

    public UnityEvent LeverPulled;

    public Animator animator;
    float cooldown;


    public int unique_id;
    public int Get_Unique_ID { get => unique_id; set => unique_id = value; }

    public bool Get_Should_Save => onlyOnce && isOn;

    public bool Interact(Interactor interactor)
    {
        if(cooldown > Time.time) return false;
        if(!isInteractable) return false;
        ToggleLever();
        cooldown = Time.time + 0.2f;
        if(onlyOnce) isInteractable = false;
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
        if(ActivateWhenOnAndOff)
            LeverPulled?.Invoke();
        animator.Play("TurnOff");
    }

    public void SetInteractable(bool canInteract)
    {
        isInteractable = canInteract;
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        isInteractable = false;
        animator.Play("TurnOn");
    }
}
