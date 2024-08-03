using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SocialPlatforms.Impl;

public class PickableItem : MonoBehaviour
{

    private Rigidbody2D rb;
    private PolygonCollider2D itemCollider;
    private Transform visual = null;
    private GameObject holder;
    private GameObject playerReference;
    private bool hasTarget;
    private Vector3 targetPosition;

    public float magnetSpeed = 5f;
    public AudioMixerGroup mixerGroup;

    public VoidEventChannelSO PickUpChannel { get; set; }
    public IntEventChannelSO IntPickupChannel { get; set; }
    public string ItemType { get; set; }
    public GameObject ItemPrefab { get; set; }
    public AudioClip PickUpClip { get; set; }

    void Start()
    {
        AddAttribute();
        StartCoroutine(PickUpFloatingAnimation());
    }

    private void AddAttribute()
    {
        rb = GetComponent<Rigidbody2D>();
        itemCollider = gameObject.GetComponentInChildren<PolygonCollider2D>();
        visual = gameObject.transform.GetChild(0);
    }

    private void FixedUpdate()
    {
        if (IsMoving())
        {
            itemCollider.isTrigger = true;
        }

        if (hasTarget)
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized;
            rb.velocity = new Vector2(targetDirection.x, targetDirection.y) * magnetSpeed;
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    private bool IsMoving()
    {
        if (rb.velocity == Vector2.zero)
            return false;
        else
            return true;
    }

    private void Collected()
    {
        PlayClipAtPointWithMixer(PickUpClip, gameObject.transform.position, group: mixerGroup);
        //AudioSource.PlayClipAtPoint(PickUpClip, gameObject.transform.position);
        TriggerItemEvent();
        Destroy(gameObject);
    }

    private void TriggerItemEvent()
    {
        switch (ItemType)
        {
            case "Item":
                holder = playerReference.GetComponentInChildren<ItemHolder>().gameObject;
                break;
            case "Weapon":
                holder = playerReference.GetComponentInChildren<WeaponHolder>().gameObject;
                break;
            case "Score":
                PickUpChannel?.RaiseEvent();
                break;
            case "Heal":
                IntPickupChannel?.RaiseEvent(1);
                break;
            default:
                holder = null;
                break;
        }

        if (holder != null)
        {
            if (ItemPrefab != null)
            {
                var item = Instantiate(ItemPrefab);
                item.transform.SetParent(holder.transform, false);
            }
        }
    }

    private IEnumerator PickUpFloatingAnimation()
    {
        while (true)
        {
            visual.Rotate(Vector2.up, 60 * Time.deltaTime, Space.World);
            visual.localPosition = new Vector3(0, Mathf.Sin(Time.time) * 0.2f, 0);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerReference = collision.gameObject;
            Collected();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerReference = collision.gameObject;
            Collected();
        }
    }

    public static void PlayClipAtPointWithMixer(AudioClip clip, Vector3 position, float volume = 1.0f, AudioMixerGroup group = null)
    {
        if (clip == null) return;
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        if (group != null)
            audioSource.outputAudioMixerGroup = group;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
}
