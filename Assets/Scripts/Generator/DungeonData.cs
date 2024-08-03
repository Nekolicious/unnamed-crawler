using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData : MonoBehaviour
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public HashSet<Vector2Int> Path { get; set; } = new HashSet<Vector2Int>();

    public GameObject PlayerReference { get; set; }
    public void Reset()
    {
        foreach (Room room in Rooms)
        {
            foreach (var item in room.PropObjectReferences)
            {
                Destroy(item);
            }
            foreach (var item in room.EnemiesInTheRoom.Keys)
            {
                Destroy(item);
            }
            foreach (var item in room.SpecialObject.Keys)
            {
                Destroy(item);
            }
        }
        Rooms = new();
        Path = new();
        //Destroy(PlayerReference);
    }

    public IEnumerator TutorialCoroutine(Action code)
    {
        yield return new WaitForSeconds(1);
        code();
    }
}


/// <summary>
/// Holds all the data about the room
/// </summary>
public class Room
{
    public Vector2 RoomCenterPos { get; set; }
    public float RoomLevel { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new();
    public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new();
    public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new();
    public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new();
    public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new();

    public HashSet<Vector2Int> InnerTiles { get; set; } = new();

    public HashSet<Vector2Int> PropPositions { get; set; } = new();
    public List<GameObject> PropObjectReferences { get; set; } = new List<GameObject>();

    public List<Vector2Int> PositionsAccessibleFromPath { get; set; } = new List<Vector2Int>();

    public Dictionary<GameObject, Vector2> EnemiesInTheRoom { get; set; } = new();

    public Dictionary<GameObject, Vector2> SpecialObject { get; set; } = new();

    public Room(Vector2 roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
    }
}
