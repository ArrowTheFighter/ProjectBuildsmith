using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform interactionPosition;
    public float interactionRadius = 0.5f;
    public LayerMask interactableMask;
    public InteractionPromptUI interactionPromptUI;
    PlayerInput playerInput;

    private readonly Collider[] colliders = new Collider[3];
    public int numberFound;

    private IInteractable interactable;

    private void Start()
    {
        playerInput = GameplayInput.instance.playerInput;
        playerInput.actions["Interact"].performed += Interact;
    }

    private void Update()
    {
        numberFound = Physics.OverlapSphereNonAlloc(interactionPosition.position, interactionRadius, colliders, interactableMask);

        if(numberFound > 0)
        {
            interactable = colliders[0].GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (!interactable.CanInteract) return;
                if (!interactionPromptUI.IsDisplayed) interactionPromptUI.SetUp(interactable.INTERACTION_PROMPT, interactable.required_items);
                if (interactionPromptUI.IsDisplayed) interactionPromptUI.ChangeText(interactable.INTERACTION_PROMPT,interactable.required_items);
                

                //if (playerInput.actions["Interact"].ReadValue<float>() > 0) interactable.Interact(this);
            }
        }
        else
        {
            if(interactable != null) interactable = null;
            if (interactionPromptUI.IsDisplayed) interactionPromptUI.Close();
        }
    }

    void Interact(InputAction.CallbackContext context)
    {
        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPosition.position, interactionRadius);
    }
}