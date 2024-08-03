using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Setting : MonoBehaviour
{
    private VisualElement root;
    private Button close, clearScore;
    private UIDocument ui;
    private Slider sfxSlider, musicSclider;
    private Toggle fullscreen;
    private DropdownField resolution;

    [SerializeField]
    private AudioMixer mainMixer;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();
    }

    public void ShowSetting()
    {
        root = ui.rootVisualElement;
        close = root.Q<Button>(name: "settingClose");
        sfxSlider = root.Q<Slider>(name: "sfx");
        musicSclider = root.Q<Slider>(name: "music");
        clearScore = root.Q<Button>(name: "clearHighscore");
        fullscreen = root.Q<Toggle>(name: "fullscreen");
        resolution = root.Q<DropdownField>(name: "resolution");

        float value;
        if (mainMixer.GetFloat("SFX", out value))
        {
            sfxSlider.value = value;
        }

        if (mainMixer.GetFloat("Music", out value))
        {
            musicSclider.value = value;
        }

        InitResolution();

        sfxSlider.RegisterValueChangedCallback(SetSFXVolume);
        musicSclider.RegisterValueChangedCallback(SetMusicVolume);
        resolution.RegisterValueChangedCallback(ChangeResolution);
        fullscreen.RegisterValueChangedCallback(ChangeFullscreen);

        clearScore.RegisterCallback<ClickEvent>(ClearHighScore);
        close.RegisterCallback<ClickEvent>(OnClose);
    }

    private void InitResolution()
    {
        List<string> resolutionOptions = new();
        foreach (var resolution in Screen.resolutions)
        {
            resolutionOptions.Add($"{resolution.width} x {resolution.height}");
        }

        resolution.choices = resolutionOptions;

        int currentResolutionIndex = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == Screen.width &&
                Screen.resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        resolution.index = currentResolutionIndex;
        fullscreen.value = Screen.fullScreen;
    }

    private void ChangeResolution(ChangeEvent<string> evt)
    {
        int selected = resolution.index;
        Resolution selectedResolution = Screen.resolutions[selected];

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }
    private void ChangeFullscreen(ChangeEvent<bool> evt)
    {
        Screen.fullScreen = evt.newValue;
    }

    private void ClearHighScore(ClickEvent evt)
    {
        GlobalStats.instance.ClearHighScore();
    }

    private void OnClose(ClickEvent evt)
    {
        ui.enabled = false;
    }

    private void SetSFXVolume(ChangeEvent<float> evt)
    {
        var volume = evt.newValue;
        mainMixer.SetFloat("SFX", volume);
    }

    private void SetMusicVolume(ChangeEvent<float> evt)
    {
        var volume = evt.newValue;
        mainMixer.SetFloat("Music", volume);
    }


}
