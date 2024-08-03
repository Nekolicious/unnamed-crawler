using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProfileTest), true)]
public class ProfileTestEditor : Editor
{
    ProfileTest generator;

    private void Awake()
    {
        generator = (ProfileTest)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Begin Test"))
        {
            generator.Begin();
        }
    }
}

