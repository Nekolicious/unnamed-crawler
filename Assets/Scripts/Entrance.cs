using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : Interactable
{
    private Key key;

    protected override void Use()
    {
        key = player.GetComponentInChildren<Key>();
        if (key != null)
        {
            LevelManager.Instance.NextStage();
            Destroy(key);
        }
        else
        {
            Debug.Log("No key");
        }
    }
}
