using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayInput : MonoBehaviour
{

    public InputActionAsset inputAsset;

    Dictionary<string, bool> actionUsed = new Dictionary<string, bool>();

    [HideInInspector] public PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

    }




    void Start()
    {
        foreach (var map in inputAsset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                string key = $"{map.name}/{action.name}";
                actionUsed[key] = false;

                action.performed += ctx => MarkActionUsed(map.name, action.name);
            }
        }
    }

   

    void MarkActionUsed(string mapName, string actionName)
    {
        string key = $"{mapName}/{actionName}";
        if (!actionUsed[key])
        {
            actionUsed[key] = true;
        }
    }

    public bool HasUsedInput(string mapName, string actionName)
    {
        string key = $"{mapName}/{actionName}";
        return actionUsed.ContainsKey(key) && actionUsed[key];
    }

    public void SwitchToUI()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SwitchToGameplay()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

}
