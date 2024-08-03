using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    DungeonData dungeonData;
    GameObject target;
    HealthPoint health;

    [Header("Listener")]
    public VoidEventChannelSO onHealthChanged, onScoreChanged, onLevelChanged;

    private VisualElement root;
    private ProgressBar healthBar;
    private Label scoreLabel, levelLabel;

    private int maxHealth;

    private bool targetIsAvailable = false;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q<ProgressBar>(name: "healthBar");
        scoreLabel = root.Q<Label>(name: "scoreLabel");
        levelLabel = root.Q<Label>(name: "levelLabel");
        dungeonData = FindObjectOfType<DungeonData>();
    }

    private void OnEnable()
    {
        onHealthChanged.onRaiseEvent += UpdatePlayerHealth;
        onScoreChanged.onRaiseEvent += updateScore;
        onLevelChanged.onRaiseEvent += updateLevel;
    }

    private void OnDisable()
    {
        onHealthChanged.onRaiseEvent -= UpdatePlayerHealth;
        onScoreChanged.onRaiseEvent -= updateScore;
        onLevelChanged.onRaiseEvent -= updateLevel;
    }

    public void FindPlayer()
    {
        target = dungeonData.PlayerReference;
        targetIsAvailable = target != null;
        if (targetIsAvailable)
        {
            health = target.GetComponent<HealthPoint>();
            UpdatePlayerHealth();
        }
    }
    private void UpdatePlayerHealth()
    {
        if (targetIsAvailable)
        {
            maxHealth = health.maxHealth;
            healthBar.highValue = maxHealth;
            var currentHealth = health.currentHealth;
            healthBar.title = currentHealth+"/"+maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void updateScore()
    {
        scoreLabel.text = GlobalStats.instance.score.ToString();
    }

    private void updateLevel()
    {
        levelLabel.text = "Level : "+GlobalStats.instance.level.ToString()+" - "+GlobalStats.instance.dungeonHeight.ToString()+"x"+GlobalStats.instance.dungeonWidth.ToString();
        updateScore();
    }
}
