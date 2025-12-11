
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform interactionPosition;
    public float interactionRadius = 0.5f;
    public LayerMask interactableMask;
    public InteractionPromptUI interactionPromptUI;

    private readonly Collider[] colliders = new Collider[3];
    public int numberFound;

    private IInteractable interactable;

    public event Action InteractorLostAllInteractions;

    void Awake()
    {
        ScriptRefrenceSingleton.OnScriptLoaded += BindInputs;
    }


    void BindInputs()
    {
        ScriptRefrenceSingleton.OnScriptLoaded -= BindInputs;

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Interact"].performed += Interact;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnBindInputs;
    }
    
    void UnBindInputs()
    {

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Interact"].performed -= Interact;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnBindInputs;
    }



    private void Update()
    {
        numberFound = Physics.OverlapSphereNonAlloc(interactionPosition.position, interactionRadius, colliders, interactableMask);

        if(numberFound > 0)
        {
            for (int i = 0; i < numberFound; i++)
            {
                interactable = colliders[i].GetComponent<IInteractable>();
                if(interactable != null && !interactable.CanInteract) continue;
                break;
            }

            if (interactable != null)
            {
                //if (!interactable.CanInteract) return;
                if (!interactionPromptUI.IsDisplayed) interactionPromptUI.SetUp(interactable.INTERACTION_PROMPT, interactable.required_items);
                if (interactionPromptUI.IsDisplayed) interactionPromptUI.ChangeText(interactable.INTERACTION_PROMPT,interactable.required_items);
                

                //if (playerInput.actions["Interact"].ReadValue<float>() > 0) interactable.Interact(this);
            }
        }
        else
        {
            if(interactable != null)
            {
                InteractorLostAllInteractions?.Invoke();
                interactable = null;
            } 
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