using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AgentPlacer : MonoBehaviour
{
    private GameObject[] enemyPrefabs;
    private GameObject[] specialObjects;

    private bool hasEntrance = false;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private int playerRoomIndex;

    [SerializeField]
    private int enemiesPerRoom;

    [SerializeField]
    private bool randomizeEnemiesCountPerRoom = true;

    DungeonData dungeonData;

    public UnityEvent onFinished;

    [SerializeField]
    private bool showGizmo = false;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
        enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        specialObjects = Resources.LoadAll<GameObject>("Prefabs/Objects");
    }

    public void PlaceAgents()
    {
        if (MeasureMemory.isActive)
            MeasureMemory.MeasureMethodMemoryUsage(StartPlace);
        else
            StartPlace();
    }

    private void StartPlace()
    {
        //float startTime = Time.realtimeSinceStartup;
        if (dungeonData == null)
            return;

        //Reading room levels
        List<float> roomLevels = new List<float>();
        foreach (Room room in dungeonData.Rooms)
        {
            roomLevels.Add(room.RoomLevel);
        }

        var roomCount = dungeonData.Rooms.Count;
        //Loop for each room
        for (int i = 0; i < roomCount; i++)
        {
            //TO place eneies we need to analyze the room tiles to find those accesible from the path
            Room room = dungeonData.Rooms[i];
            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);

            //Find the Path inside this specific room
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
            //Find the tiles belonging to both the path and the room
            roomFloor.IntersectWith(dungeonData.Path);

            //Run the BFS to find all the tiles in the room accessible from the path
            Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);

            //Positions that we can reach + path == positions where we can place enemies
            room.PositionsAccessibleFromPath = roomMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            //spawn enemies per room
            if (i != playerRoomIndex)
            {
                if (enemiesPerRoom == 0)
                    break;
                if (randomizeEnemiesCountPerRoom)
                {
                    PlaceEnemies(room, UnityEngine.Random.Range(1, enemiesPerRoom), roomLevels);
                }
                else
                {
                    PlaceEnemies(room, enemiesPerRoom, roomLevels);
                }
            }

            //Place destructible and key objects

            PlaceSpecialObjects(room, roomLevels);

            //Place the player
            if (i == playerRoomIndex)
            {
                //GameObject player = Instantiate(playerPrefab);
                GameObject player = SpawnPlayer();
                player.transform.localPosition = dungeonData.Rooms[i].RoomCenterPos + Vector2.one * 0.5f;
                GlobalStats.instance.playerSpawnPoint = dungeonData.Rooms[i].RoomCenterPos + Vector2.one * 0.5f;
                dungeonData.PlayerReference = player;
            }
        }

        //float endTime = Time.realtimeSinceStartup;
        //double executionTime = (endTime - startTime) * 1000;
        //UnityEngine.Debug.Log($"AgentPlacer - Total: {executionTime}ms");

        onFinished?.Invoke();
    }

    private void PlaceSpecialObjects(Room room, List<float> roomLevels)
    {
        //Find the highest level
        float highestLevel = roomLevels.Max();
        List<Vector2Int> positionAvailable = room.PositionsAccessibleFromPath
            .Select(v => new Vector2Int(v.x, v.y))
            .Except(room.EnemiesInTheRoom.Values
            .Select(v => new Vector2Int((int)v.x, (int)v.y)))
            .ToList();
        foreach (GameObject go in specialObjects)
        {
            //check if current component IS an entrance
            if (go.GetComponent<Entrance>() != null)
            {
                //Skip if entrance has been placed or current level is not the highest level
                if (hasEntrance || room.RoomLevel != highestLevel)
                    continue;
                hasEntrance = true;
            }
            
            var position = positionAvailable[UnityEngine.Random.Range(0, positionAvailable.Count)];
            
            GameObject obj = Instantiate(go);
            obj.transform.localPosition = (Vector2)position + Vector2.one * 0.5f;
            room.SpecialObject.Add(obj, position);
            positionAvailable.Remove(position);
        }
    }

    private GameObject SpawnPlayer()
    {
        GameObject playerExist = (GameObject.FindWithTag("Player"));
        if (playerExist == null)
        {
            playerExist = Instantiate(playerPrefab);
        }
        return playerExist;
    }

    /// <summary>
    /// Places enemies in the positions accessible from the path
    /// </summary>
    /// <param name="room"></param>
    /// <param name="enemysCount"></param>
    private void PlaceEnemies(Room room, int enemysCount, List<float> roomLevels)
    {
        //GameObject enemyPrefab = enemyPrefabs.ElementAt(UnityEngine.Random.Range(0, enemyPrefabs.Length));
        List<GameObject> enemyPrefab = new();
        List<GameObject> enemyElitePrefab = new();
        foreach (GameObject go in enemyPrefabs)
        {
            if (go.name.Contains("Elite"))
            {
                enemyElitePrefab.Add(go);
            } else
            {
                enemyPrefab.Add(go);
            }
        }

        float highestLevel = roomLevels.Max();
        for (int k = 0; k < enemysCount; k++)
        {
            if (room.PositionsAccessibleFromPath.Count <= k)
            {
                return;
            }

            if (room.RoomLevel <= highestLevel && k == enemysCount-1)
            {
                GameObject enemy = Instantiate(enemyElitePrefab.ElementAt(UnityEngine.Random.Range(0, enemyElitePrefab.Count())));
                enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[k] + Vector2.one * 0.5f;
                room.EnemiesInTheRoom.Add(enemy, enemy.transform.localPosition);
            } else
            {
                GameObject enemy = Instantiate(enemyPrefab.ElementAt(UnityEngine.Random.Range(0, enemyPrefab.Count())));
                enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[k] + Vector2.one * 0.5f;
                room.EnemiesInTheRoom.Add(enemy, enemy.transform.localPosition);
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (dungeonData == null || showGizmo == false)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            Color color = Color.green;
            color.a = 0.3f;
            Gizmos.color = color;

            foreach (Vector2Int pos in room.PositionsAccessibleFromPath)
            {
                Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}

public class RoomGraph
{
    public static List<Vector2Int> fourDirections = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

    public RoomGraph(HashSet<Vector2Int> roomFloor)
    {
        foreach (Vector2Int pos in roomFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in fourDirections)
            {
                Vector2Int newPos = pos + direction;
                if (roomFloor.Contains(newPos))
                {
                    neighbours.Add(newPos);
                }
            }
            graph.Add(pos, neighbours);
        }
    }

    /// <summary>
    /// Creates a map of reachable tiles in our dungeon.
    /// </summary>
    /// <param name="startPos">Door position or tile position on the path between rooms inside this room</param>
    /// <param name="occupiedNodes"></param>
    /// <returns></returns>
    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        //BFS related variuables
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        visitedNodes.Add(startPos);

        //The dictionary that we will return 
        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>();
        map.Add(startPos, startPos);

        while (nodesToVisit.Count > 0)
        {
            //get the data about specific position
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            //loop through each neighbour position
            foreach (Vector2Int neighbourPosition in neighbours)
            {
                //add the neighbour position to our map if it is valid
                if (visitedNodes.Contains(neighbourPosition) == false &&
                    occupiedNodes.Contains(neighbourPosition) == false)
                {
                    nodesToVisit.Enqueue(neighbourPosition);
                    visitedNodes.Add(neighbourPosition);
                    map[neighbourPosition] = node;
                }
            }
        }

        return map;
    }
}