using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private bool targetIsAvailable = false;

    DungeonData dungeonData;

    public UnityEvent onFinished;

    [SerializeField] private Transform target;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
    }

    public void FindPlayer()
    {
        target = dungeonData.PlayerReference.transform;
        targetIsAvailable = target != null;
    }

    void Update()
    {
        if (target != null)
            TrackPlayer();
    }

    private void TrackPlayer()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
