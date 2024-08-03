using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public UnityAction<int> onRaiseEvent;

    public void RaiseEvent(int value)
    {
        onRaiseEvent?.Invoke(value);
    }
}
