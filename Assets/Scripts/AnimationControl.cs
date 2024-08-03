using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnimationControl : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }
    private Animator anim;

    public UnityEvent onStep;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Flip();
    }

    private void Flip()
    {
        Vector2 pointerPosition = (PointerPosition - (Vector2)transform.position).normalized;
        Vector2 scale = transform.localScale;
        if (pointerPosition.x < 0)
        {
            scale.x = -1;
        }
        else
        {
            scale.x = 1;
        }
        transform.localScale = scale;
    }

    public void IsRunning(bool status)
    {
        if(status)
        {
            anim.SetBool("isRunning", true);
        } else
        {
            anim.SetBool("isRunning", false);
        }
    }

    public void Footstep()
    {
        onStep?.Invoke();
    }
}
