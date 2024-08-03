using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }

    private AgentControl agentControl;

    private void Awake()
    {
        agentControl = GetComponentInParent<AgentControl>();
    }

    private void Update()
    {
        PointerPosition = agentControl.PointerInput;
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.up = direction;
    }
}
