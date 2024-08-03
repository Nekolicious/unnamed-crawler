using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    private protected PlayerInput playerInput;
    private protected GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            //Debug.Log("player is detected");
            playerInput = collision.GetComponent<PlayerInput>();
            playerInput.OnInteract.AddListener(Use);
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("player is leaving");
            playerInput.OnInteract.RemoveListener(Use);
            playerInput = null;
            player = null;
        }
    }

    protected virtual void Use()
    {
        Debug.Log("Object interacted");
    }
}
