using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    public int minRoomWidth = 10, minRoomHeight = 10;
    [SerializeField]
    public int dungeonWidth = 50, dungeonHeight = 50;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 2;
    [SerializeField]
    private bool randomWalkRooms = true;

    [SerializeField]
    private VoidEventChannelSO onBeginTrigger, onRandomWalkFinish, onBSPFinish;

    // PCG Data
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    public UnityEvent OnFinishedRoomGeneration;

    protected DungeonData dungeonData;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
        if (dungeonData == null)
            dungeonData = gameObject.AddComponent<DungeonData>();
    }
    private void OnEnable()
    {
        onBeginTrigger.onRaiseEvent += TriggerEvent;
    }

    private void OnDisable()
    {
        onBeginTrigger.onRaiseEvent -= TriggerEvent;
    }

    private void TriggerEvent()
    {
        tilemapVisualizer.Clear();
        if (GlobalStats.instance.dungeonHeight >= 50 && GlobalStats.instance.dungeonWidth >= 50)
        {
            dungeonWidth = GlobalStats.instance.dungeonWidth;
            dungeonHeight = GlobalStats.instance.dungeonHeight;
        }
        if (MeasureMemory.isActive)
            MeasureMemory.MeasureMethodMemoryUsage(RunProceduralGeneration);
        else
            RunProceduralGeneration();
    }

    protected override void RunProceduralGeneration()
    {
        if (dungeonData != null)
            ClearRoomData();

#if UNITY_EDITOR
        dungeonData = FindObjectOfType<DungeonData>();
        if (dungeonData == null)
            dungeonData = gameObject.AddComponent<DungeonData>();
#endif
        //float startTime = Time.realtimeSinceStartup;
        CreateRooms();
        //float endTime = Time.realtimeSinceStartup;
        //double executionTime = (endTime - startTime) * 1000;
        //UnityEngine.Debug.Log($"RFDG - Total: {executionTime}ms");

        OnFinishedRoomGeneration?.Invoke();
    }

    protected void ClearRoomData()
    {
        dungeonData.Reset();
    }

    protected void CreateRooms()
    {
        //float startTime = Time.realtimeSinceStartup;

        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int
            (dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        //float endTime = Time.realtimeSinceStartup;
        //double executionTime = (endTime - startTime) * 1000;
        //UnityEngine.Debug.Log($"RFDG - BSP : {executionTime}ms");

        HashSet<Vector2Int> floor = new();

        if (randomWalkRooms)
        {
            //startTime = Time.realtimeSinceStartup;

            floor = CreateRoomsRandomly(roomsList);

            //endTime = Time.realtimeSinceStartup;
            //executionTime = (endTime - startTime) * 1000;
            //UnityEngine.Debug.Log($"RFDG - RW: {executionTime}ms");
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        //startTime = Time.realtimeSinceStartup;

        List<Vector2Int> roomCenters = new();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        //endTime = Time.realtimeSinceStartup;
        //executionTime = (endTime - startTime) * 1000;
        //UnityEngine.Debug.Log($"RFDG - Finishing: {executionTime}ms");
    }

    protected HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            HashSet<Vector2Int> tempPos = new();
            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                    tempPos.Add(position);
                }
            }
            dungeonData.Rooms.Add(new Room(roomCenter, tempPos));
        }
        return floor;
    }

    protected HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        dungeonData.Path.UnionWith(corridors);
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    protected HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
            dungeonData.Rooms.Add(new Room(room.center, floor));
        }
        return floor;
    }
}
