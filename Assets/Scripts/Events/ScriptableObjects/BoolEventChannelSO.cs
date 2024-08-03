using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Bool Event Channel")]
public class BoolEventChannelSO : ScriptableObject
{
    public UnityAction<bool> onRaiseEvent;

    public void RaiseEvent(bool value)
    {
        onRaiseEvent?.Invoke(value);
    }
}
