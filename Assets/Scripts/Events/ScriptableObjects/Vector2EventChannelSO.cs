using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Vector2 Event Channel")]
public class Vector2EventChannelSO : ScriptableObject
{
    public UnityAction<Vector2> onRaiseEvent;
    public void RaiseEvent(Vector2 value)
    {
        onRaiseEvent?.Invoke(value);
    }
}
