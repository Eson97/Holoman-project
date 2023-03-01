using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private InputAction _interactAction;
    private PlayerInput _input;

    public static Queue<Action> ActionQueue = new Queue<Action>();

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _interactAction = _input.actions["Interact"];
    }

    private void OnEnable() => _interactAction.performed += Interact;
    private void OnDisable() => _interactAction.performed -= Interact;

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Default) return;
        Debug.Log("Interact button Pressed");
        if (ActionQueue.Count <= 0) return;

        var current = ActionQueue.Dequeue();
        current?.Invoke();
    }
}
