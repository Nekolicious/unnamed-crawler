using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Pickable Item")]
public class PickableItemSO : ScriptableObject
{
    public Sprite sprite;
    [Header("Item Picked Up")]
    public AudioClip pickUpClip;
    public VoidEventChannelSO pickUpChannel;
    public IntEventChannelSO intPickUpChannel;

    public ItemRarity rarity;

    [Header("Item Type")]
    public Type itemType;

    [Header("Prefab")]
    [Tooltip("If TRUE 'gameObject' :" +
        "\n- 'Item' will be placed on ItemHolder" +
        "\n- 'Weapon' will be placed in WeaponHolder")]
    public bool hasGameObject;
    public GameObject gameObjectPrefab;
}

public enum Type
{
    Item,
    Weapon,
    Score,
    Heal
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Key,
}
