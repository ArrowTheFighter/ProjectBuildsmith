using UnityEngine;
using UnityEngine.Events;

public class ActivateArea : MonoBehaviour, IInteractable
{
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] required_items => items_requried;
    public item_requirement[] items_requried;

    public bool IsInteractable;
    public bool CanInteract { get => IsInteractable; set {IsInteractable = value;} }

    public UnityEvent ActivatedEvent;

    public bool Interact(Interactor interactor)
    {
        ActivatedEvent?.Invoke();
        return true;
    }

    
}
