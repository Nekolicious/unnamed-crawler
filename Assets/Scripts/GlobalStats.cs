using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class GlobalStats : MonoBehaviour
{
    public static GlobalStats instance { get; private set; }

    [Header("Dungeon Stats")]
    public int dungeonHeight = 50;
    public int dungeonWidth = 50;
    public int minRoomHeight = 10;
    public int minRoomWidth = 10;

    [Header("Game Stats")]
    public int level = 1;
    public int score = 0;
    public int dungeonChestInMap = 0;
    public float keyDropChance = 0f;
    public bool keyIsDropped = false;

    [Header("Listener")]
    public VoidEventChannelSO onChestDestroyed, onKeyPickedUp, onCoinPickedUp, chestCounter, enemyDefeated;
    public BoolEventChannelSO onKeyDropped;

    [Header("Broadcaster")]
    public VoidEventChannelSO scoreChanged;

    public Vector3 playerSpawnPoint;

    public void Reset()
    {
        dungeonHeight = 50;
        dungeonWidth = 50;
        minRoomHeight = 10;
        minRoomWidth = 10;
        level = 1;
        score = 0;
        dungeonChestInMap = 0;
        keyDropChance = 0f;
        keyIsDropped = false;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        onChestDestroyed.onRaiseEvent += ChestCountUp;
        onKeyDropped.onRaiseEvent += KeyDropped;
        onKeyPickedUp.onRaiseEvent += KeyPickedUp;
        onCoinPickedUp.onRaiseEvent += CoinPickedUp;
        chestCounter.onRaiseEvent += CountChest;
        enemyDefeated.onRaiseEvent += EnemyDefeated;
    }
    private void OnDisable()
    {
        onChestDestroyed.onRaiseEvent -= ChestCountUp;
        onKeyDropped.onRaiseEvent -= KeyDropped;
        onKeyPickedUp.onRaiseEvent -= KeyPickedUp;
        onCoinPickedUp.onRaiseEvent -= CoinPickedUp;
        chestCounter.onRaiseEvent -= CountChest;
        enemyDefeated.onRaiseEvent -= EnemyDefeated;
    }
    public void CountChest()
    {
        dungeonChestInMap = GameObject.FindGameObjectsWithTag("Chest").Length;
    }
    private void ChestCountUp()
    {
        dungeonChestInMap--;
        if (!keyIsDropped)
        {
            if (dungeonChestInMap == 1)
                keyDropChance = 1f;
            else
                keyDropChance += 0.015f;
        }
    }
    private void KeyDropped(bool value)
    {
        keyIsDropped = value;
    }
    private void CoinPickedUp()
    {
        score += 100;
        scoreChanged?.RaiseEvent();
    }
    private void EnemyDefeated()
    {
        score += 500;
        scoreChanged?.RaiseEvent();
    }
    private void KeyPickedUp()
    {
        Debug.Log("key picked up");
    }

    public void CheckHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.SetString("HighLevel", GlobalStats.instance.level.ToString() + " - " + GlobalStats.instance.dungeonHeight.ToString() + "x" + GlobalStats.instance.dungeonWidth.ToString());
        }
    }

    public (int, string) GetHighScore()
    {
        int hScore = PlayerPrefs.GetInt("HighScore", 0);
        string hLevel = PlayerPrefs.GetString("HighLevel", "0 - 0x0");

        return (hScore, hLevel);
    }

    public void ClearHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("HighLevel");
    }
}
