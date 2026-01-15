using System;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable _interactableInRange = null;
    public GameObject interactionIcon;
    
    private void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _interactableInRange?.Interact();
            if (_interactableInRange != null && !_interactableInRange.CanInteract())
            {
                interactionIcon.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            _interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == _interactableInRange)
        {
            _interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
