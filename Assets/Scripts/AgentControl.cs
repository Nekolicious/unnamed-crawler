using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentControl : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;
    private Vector2 movementInput, pointerInput;
    private WeaponParent weaponParent;
    private AnimationControl playerAnimation;

    public Vector2 PointerInput { get => pointerInput; set => pointerInput = value; }
    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }

    public void PerformAttack()
    {
        weaponParent.Attack();
    }

    private void Awake()
    {
        weaponParent = GetComponentInChildren<WeaponParent>();
        playerAnimation = GetComponentInChildren<AnimationControl>();
    }

    void Update()
    {
        //pointerInput = GetPointerInput();
        weaponParent.PointerPosition = pointerInput;
        playerAnimation.PointerPosition = pointerInput;
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        rb.velocity = new Vector2(movementInput.x * moveSpeed, movementInput.y * moveSpeed);
        if (rb.velocity == Vector2.zero) {
            playerAnimation.IsRunning(false);
        }
        else
        {
            playerAnimation.IsRunning(true);
        }
    }
}
