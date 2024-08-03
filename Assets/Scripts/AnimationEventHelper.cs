using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHelper : MonoBehaviour
{
    public UnityEvent OnAnimationEventTrigerred, onAttackPerformed, onAnimationStart;

    public void TriggerEvent()
    {
        OnAnimationEventTrigerred?.Invoke();
    }

    public void TriggerAttack()
    {
        onAttackPerformed?.Invoke();
    }

    public void TriggerAnimationStart()
    {
        onAnimationStart?.Invoke();
    }
}
