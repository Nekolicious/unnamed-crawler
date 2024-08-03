using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    private Transform player;

    [SerializeField]
    private float chaseDistanceThreshold = 3, attackDistanceThreshold = 0.8f;

    [SerializeField]
    private float attackDelay = 1;
    private float passedTime = 1;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);
        if(distance < chaseDistanceThreshold)
        {
            //Debug.Log("Player in chase distance");
            OnPointerInput?.Invoke(player.position);
            if(distance <= attackDistanceThreshold)
            {
                // Attack
                //Debug.Log("Player in attack distance");
                OnMovementInput?.Invoke(Vector2.zero);
                if(passedTime >= attackDelay)
                {
                    passedTime = 0;
                    OnAttack?.Invoke();
                }

            }
            else 
            {
                // Player Chase
                //Debug.Log("Player in movement distance");
                Vector2 direction = player.position - transform.position;
                OnMovementInput?.Invoke(direction.normalized); 
            }
        } 
        else
        {
            OnMovementInput?.Invoke(Vector2.zero);
        }

        if (player == null)
        {
            OnMovementInput?.Invoke(Vector2.zero);
            return;
        }

        if(passedTime<attackDelay)
        {
            passedTime += Time.deltaTime;
        }
    }
}
