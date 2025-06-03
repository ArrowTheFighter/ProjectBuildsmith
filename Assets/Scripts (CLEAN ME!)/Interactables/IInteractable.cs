using UnityEngine;

public interface IInteractable
{
    public string INTERACTION_PROMPT { get; }
    public bool Interact(Interactor interactor);
    public item_requirement[] required_items { get; }
}
