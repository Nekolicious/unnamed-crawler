using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationWindow : MonoBehaviour
{

    private UIDocument ui;
    private VisualElement root;
    private Button yes, no;
    private string route;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();
    }

    public void Verify(String _case)
    {
        switch ( _case )
        {
            case "Menu":
                route = _case;
                VerifyWindow();
                break;
            default:
                break;
        }
    }

    private void VerifyWindow()
    {
        root = ui.rootVisualElement;
        yes = root.Q<Button>(name: "yesButton");
        no = root.Q<Button>(name: "noButton");

        no.RegisterCallback<ClickEvent>(No);
        yes.RegisterCallback<ClickEvent>(Yes);
    }

    private void Yes(ClickEvent evt)
    {
        switch (route)
        {
            case "Menu":
                LevelManager.Instance.Menu();
                LevelManager.Instance.CloseConfirmWindow();
                break;
            default:
                break;
        }
    }

    private void No(ClickEvent evt)
    {
        switch (route)
        {
            case "Menu":
                LevelManager.Instance.CloseConfirmWindow();
                break;
            default:
                break;
        }
    }

}
