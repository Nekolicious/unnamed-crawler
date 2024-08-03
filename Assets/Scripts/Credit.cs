using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Credit : MonoBehaviour
{
    private static VisualElement root;
    private static Button close;
    private static Label text;
    private static UIDocument ui;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();
    }

    public static void ShowCredit()
    {
        root = ui.rootVisualElement;
        text = root.Q<Label>(name: "credit");
        close = root.Q<Button>(name: "creditClose");

        var creditFile = Resources.Load("Credits");

        text.text = creditFile.ToString();

        close.RegisterCallback<ClickEvent>(OnClose);
    }

    private static void OnClose(ClickEvent evt)
    {
        ui.enabled = false;
    }
}
