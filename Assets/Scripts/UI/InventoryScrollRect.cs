using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class InventoryScrollRect : MonoBehaviour
{
    ScrollRect scrollRect;

    [SerializeField] PlayerInput playerInput;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput != null)
        {
            float scrollInput = playerInput.actions["Scroll"].ReadValue<Vector2>().y;
            scrollRect.verticalNormalizedPosition += scrollInput * Time.deltaTime;
         }
    }
}
