using System.Collections;
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

    void Awake()
    {
        StartCoroutine(wait_till_script_refrence_is_ready());
    }

    IEnumerator wait_till_script_refrence_is_ready()
    {
        yield return StartCoroutine(ScriptRefrenceSingleton.Wait_Until_Script_is_Ready());
        playerInput.actions["Interact"].performed += Interact;
    }

    private void Start()
    {
        playerInput = ScriptRefrenceSingleton.instance.gameplayInput.playerInput;
        playerInput.actions["Interact"].performed += Interact;
       // ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += OnDisable;
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