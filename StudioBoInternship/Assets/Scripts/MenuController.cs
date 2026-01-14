using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    private void Start()
    {
        menuCanvas.SetActive(false);
    }

    public void Menu(InputAction.CallbackContext context)
    {
        if (context.performed) menuCanvas.SetActive(!menuCanvas.activeSelf);
    }
}
