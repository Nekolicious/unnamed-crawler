using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAudioManager : MonoBehaviour
{
    public List<AudioClip> attackSound;

    private AudioSource weaponSource;

    private void Awake()
    {
        weaponSource = GetComponent<AudioSource>();
    }

    public void playWeaponSound()
    {
        weaponSource.clip = attackSound[Random.Range(0, attackSound.Count)];
        weaponSource.Play();
    }
}
