using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    //public List<GameObject> items;

    [SerializeField]
    private GameObject itemPrefab;

    private Rigidbody2D rb;

    public float forceSpeed = 1f;

    [Header("Items to be dropped")]
    [SerializedDictionary("Pickable Item", "Max Quantity")]
    public SerializedDictionary<PickableItemSO, int> itemLists;

    [Header("Item Drop Chance")]
    [Range(0f, 1f)]
    public float commonDrop = 0.8f;
    [Range(0f, 1f)]
    public float rareDrop = 0.4f;
    [Range(0f, 1f)]
    public float epicDrop = 0.1f;

    [Header("Key Dropped Broadcaster")]
    public BoolEventChannelSO onKeyDroppedChannel;

    public void ThrowItems()
    {
        foreach(KeyValuePair<PickableItemSO, int> item in itemLists)
        {
            var itemData = item.Key;
            var itemQuantity = item.Value;
            float dropChance;
            switch (itemData.rarity)
            {
                case ItemRarity.Common:
                    dropChance = commonDrop;
                    break;

                case ItemRarity.Rare:
                    dropChance = rareDrop;
                    break;

                case ItemRarity.Epic:
                    dropChance = epicDrop;
                    break;

                //Key item has its own drop chance defined in GlobalStats
                case ItemRarity.Key:
                    dropChance = GlobalStats.instance.keyDropChance;
                    break;

                default:
                    dropChance = 1f;
                    break;
            }

            for (int i = 0; i < itemQuantity; i++)
            {
                // Spawn item if random range has value below or equal drop chance
                if (!(UnityEngine.Random.Range(0f, 1f) <= dropChance))
                    continue;
                // Skip if current item is key and already dropped
                if (itemData.rarity == ItemRarity.Key && GlobalStats.instance.keyIsDropped)
                    continue;
                GameObject droppedItem = Instantiate(itemPrefab, transform.position, transform.rotation);

                // Add sprite
                SpriteRenderer itemSpriteRenderer = droppedItem.GetComponentInChildren<SpriteRenderer>();
                itemSpriteRenderer.sprite = itemData.sprite;

                // Add collider based on sprite
                itemSpriteRenderer.gameObject.AddComponent<PolygonCollider2D>();

                var pickableData = droppedItem.GetComponent<PickableItem>();
                // Set audio when item picked up
                pickableData.PickUpClip = itemData.pickUpClip;
                pickableData.ItemType = itemData.itemType.ToString();

                // Set event trigger channel when picked up
                if (itemData.pickUpChannel != null)
                {
                    pickableData.PickUpChannel = itemData.pickUpChannel;
                }

                if (itemData.intPickUpChannel != null)
                {
                    pickableData.IntPickupChannel = itemData.intPickUpChannel;
                }

                // Set which prefab added to player when picked up
                if (itemData.hasGameObject)
                {
                    pickableData.ItemPrefab = itemData.gameObjectPrefab;
                }

                // Raise event if the dropped item is a Key
                if (itemData.rarity == ItemRarity.Key)
                    onKeyDroppedChannel?.RaiseEvent(true);

                // Add force to spawned item
                rb = droppedItem.GetComponent<Rigidbody2D>();
                rb.AddForce(UnityEngine.Random.onUnitSphere * forceSpeed, ForceMode2D.Impulse);
            }
        }
    }
}
