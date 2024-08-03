using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthPoint : MonoBehaviour
{
    public int currentHealth, maxHealth;
    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;
    public VoidEventChannelSO updateHealthChannel;
    public VoidEventChannelSO onDestroyedChannel;
    public IntEventChannelSO healChannel;

    [SerializeField]
    private bool isDead = false;

    private void OnEnable()
    {
        if (healChannel!= null)
            healChannel.onRaiseEvent += Heal;
    }

    private void OnDisable()
    {
        if (healChannel != null)
            healChannel.onRaiseEvent -= Heal;
    }

    private void Heal(int value)
    {
        if (currentHealth < maxHealth)
            currentHealth += value;
        updateHealthChannel?.RaiseEvent();
    }
    public void InitializeHealth (int healthValue)
    {
        currentHealth = healthValue;
        maxHealth= healthValue;
        isDead = false;
    }

    public void GetHit(int damage, GameObject sender)
    {
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);
        } else
        {
            OnDeathWithReference?.Invoke(sender);
            onDestroyedChannel?.RaiseEvent();
            isDead = true;
            Destroy(gameObject);
        }
        updateHealthChannel?.RaiseEvent();
    }
}
