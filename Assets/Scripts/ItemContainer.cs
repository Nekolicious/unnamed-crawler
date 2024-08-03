using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject itemSpawner;
    public void DropItem()
    {
        Debug.Log("item dropped");
    }
}
