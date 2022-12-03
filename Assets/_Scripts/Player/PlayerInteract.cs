using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private InputAction _interactAction;
    private PlayerInput _input;

    public Action InteractDelegate;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _interactAction = _input.actions["Interact"];

        InteractDelegate += () => Debug.Log("Interact button Pressed");
    }

    private void OnEnable() => _interactAction.performed += Interact;

    private void OnDisable() => _interactAction.performed -= Interact;

    private void Interact(InputAction.CallbackContext ctx) => InteractDelegate?.Invoke();

}
