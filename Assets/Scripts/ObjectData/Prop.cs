using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Prop : ScriptableObject
{
    [Header("Prop data:")]
    public Sprite PropSprite;
    /// <summary>
    /// Affects the collider size of the prop
    /// </summary>
    public Vector2Int PropSize = Vector2Int.one;

    [Header("Collider")]
    public bool HasCollider = true;

    [Header("Layer Sorting")]
    public LayerList layerType;

    [Header("Light")]
    public bool EmitLight = false;
    public float LightOuterRadius = 1f;
    public float LightInnerRadius = 0.6f;
    public float LightIntensity = 1f;
    public Color color = new(243, 176, 53, 255);

    [Space, Header("Placement type:")]
    public bool Corner = true;
    public bool NearWallUP = true;
    public bool NearWallDown = true;
    public bool NearWallRight = true;
    public bool NearWallLeft = true;
    public bool Inner = true;
    [Min(0)]
    public int PlacementQuantityMin = 1;
    [Min(1)]
    public int PlacementQuantityMax = 1;

    [Space, Header("Group placement:")]
    public bool PlaceAsGroup = false;
    [Min(1)]
    public int GroupMinCount = 1;
    [Min(1)]
    public int GroupMaxCount = 1;

    public enum LayerList
    {
        Foreground,
        Floor
    }

}