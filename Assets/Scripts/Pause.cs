using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pause : MonoBehaviour
{
    private static VisualElement root;
    private static Button resume, stuck, menu;
    private static UIDocument ui;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();
    }

    public static void ShowPause()
    {
        root = ui.rootVisualElement;
        resume = root.Q<Button>(name: "resumeButton");
        stuck = root.Q<Button>(name: "stuckButton");
        menu = root.Q<Button>(name: "menuButton");

        resume.RegisterCallback<ClickEvent>(Resume);
        stuck.RegisterCallback<ClickEvent>(Unstuck);
        menu.RegisterCallback<ClickEvent>(Menu);
    }

    private static void Menu(ClickEvent evt)
    {
        LevelManager.Instance.BackToMenu();
    }

    private static void Resume(ClickEvent evt)
    {
        LevelManager.Instance.ResumeGame();
    }

    private static void Unstuck(ClickEvent evt)
    {
        LevelManager.Instance.UnstuckPlayer();
    }
}
