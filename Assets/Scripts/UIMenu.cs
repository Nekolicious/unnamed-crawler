using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIMenu : MonoBehaviour
{
    private Button startButton, settingButton, creditButton, exitButton;
    private VisualElement root;
    private UIDocument creditui, settingui;
    private Label highScore;

    [SerializeField]
    private GameObject credit;
    [SerializeField]
    private GameObject setting;

    public VoidEventChannelSO onBeginTriggerChannel;
    private void Awake()
    {
        creditui = credit.GetComponent<UIDocument>();
        settingui = setting.GetComponent<UIDocument>();
        root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>(name: "start");
        settingButton = root.Q<Button>(name: "setting");
        creditButton = root.Q<Button>(name: "credit");
        exitButton = root.Q<Button>(name: "exit");
        highScore = root.Q<Label>(name: "highScore");

        startButton.RegisterCallback<ClickEvent>(onStartClick);
        settingButton.RegisterCallback<ClickEvent>(onSettingClick);
        creditButton.RegisterCallback<ClickEvent>(onCreditClick);
        exitButton.RegisterCallback<ClickEvent>(onExitClick);
        ShowHighScore();
    }

    private void ShowHighScore()
    {
        (int hScore, string hLevel) = GlobalStats.instance.GetHighScore();
        highScore.text = "High Score : " + hScore + " // " + hLevel;
    }

    private void onExitClick(ClickEvent evt)
    {
        Application.Quit();
    }

    private void onSettingClick(ClickEvent evt)
    {
        var settingUI = setting.GetComponent<Setting>();
        settingui.enabled = true;
        settingUI.ShowSetting();
    }

    private void onCreditClick(ClickEvent evt)
    {
        creditui.enabled = true;
        Credit.ShowCredit();
    }

    private void onStartClick(ClickEvent evt)
    {
        LevelManager.Instance.StartGame();
    }
}
