using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
    private UIDocument ui;
    private VisualElement root;
    private Label score;
    private Button retry, menu;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();
    }

    public void GameOverWindow()
    {
        root = ui.rootVisualElement;
        score = root.Q<Label>(name: "score");
        retry = root.Q<Button>(name: "retryButton");
        menu = root.Q<Button>(name: "menuButton");

        score.text = "Your Score : " + GlobalStats.instance.score.ToString() + " // Level : " + GlobalStats.instance.level.ToString() + " - " + GlobalStats.instance.dungeonHeight.ToString() + "x" + GlobalStats.instance.dungeonWidth.ToString();
        retry.RegisterCallback<ClickEvent>(Retry);
        menu.RegisterCallback<ClickEvent>(Menu);
    }

    private void Retry(ClickEvent evt)
    {
        LevelManager.Instance.StartGame();
        LevelManager.Instance.CloseGameOverWindow();
    }

    private void Menu(ClickEvent evt)
    {
        LevelManager.Instance.Menu();
        LevelManager.Instance.CloseGameOverWindow();
    }
}
