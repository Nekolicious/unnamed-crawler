using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource aSource1, aSource2;

    [Header("Footsteps")]
    public List<AudioClip> footstepClips;

    [Header("Hit")]
    public List<AudioClip> attackedHitClips;

    [Header("Death")]
    public List<AudioClip> deathClips;

    void Awake()
    {
        aSource1 = GetComponents<AudioSource>()[0] ?? GetComponent<AudioSource>();
        aSource2 = GetComponents<AudioSource>()[1] ?? GetComponent<AudioSource>();
    }

    public void PlayHitsound()
    {
        aSource2.clip = attackedHitClips[Random.Range(0, attackedHitClips.Count)];
        aSource2.Play();
    }

    public void PlayFootstep()
    {
        aSource1.clip = footstepClips[Random.Range(0, footstepClips.Count)];
        aSource1.Play();
    }

    public void PlayDeathsound()
    {
        AudioSource.PlayClipAtPoint(deathClips[Random.Range(0, deathClips.Count)], gameObject.transform.position);
    }
}
