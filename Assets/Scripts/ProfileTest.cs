using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class ProfileTest : MonoBehaviour
{
    [Header("Broadcast Channel")]
    [SerializeField]
    private VoidEventChannelSO startTest;

    [Header("Listen Channel")]
    [SerializeField]
    private VoidEventChannelSO onDungeonFinish, onDungeonRW, onDungeonBSP, onLayoutFinish, onObjectFinish, onAgentFinish;

    [SerializeField]
    public int dungeonWidth = 50, dungeonHeight = 50;

    private float startTime, endTime;
    private long startMemory, endMemory, memoryUsed;

    private void OnEnable()
    {
        onDungeonFinish.onRaiseEvent += Finish;
        onDungeonRW.onRaiseEvent += RandomWalkFinish;
        onDungeonBSP.onRaiseEvent += BSPFinish;
        onLayoutFinish.onRaiseEvent += LayoutFinish;
        onObjectFinish.onRaiseEvent += ObjectFinish;
        onAgentFinish.onRaiseEvent += AgentFinish;
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        onDungeonFinish.onRaiseEvent -= Finish;
        onDungeonRW.onRaiseEvent -= RandomWalkFinish;
        onDungeonBSP.onRaiseEvent -= BSPFinish;
        onLayoutFinish.onRaiseEvent -= LayoutFinish;
        onObjectFinish.onRaiseEvent -= ObjectFinish;
        onAgentFinish.onRaiseEvent -= AgentFinish;
        Application.logMessageReceived -= Log;
    }

    public void Begin()
    {

        GlobalStats.instance.dungeonHeight = dungeonHeight;
        GlobalStats.instance.dungeonWidth = dungeonWidth;

        BeginTest();
    }

    private void BeginTest()
    {
        MeasureMemory.isActive = false;
        UnityEngine.Debug.Log($"Size : {GlobalStats.instance.dungeonHeight} x {GlobalStats.instance.dungeonWidth}");
        startTime = Time.realtimeSinceStartup;

        startTest?.RaiseEvent();
    }

    public void ObjectFinish()
    {
        endTime = Time.realtimeSinceStartup;
        double executionTime = (endTime - startTime) * 1000;
        UnityEngine.Debug.Log($"Object Placer : {executionTime} ms");
    }

    public void AgentFinish()
    {
        endTime = Time.realtimeSinceStartup;
        double executionTime = (endTime - startTime) * 1000;
        UnityEngine.Debug.Log($"Agent Placer : {executionTime} ms");
    }

    public void LayoutFinish()
    {
        endTime = Time.realtimeSinceStartup;
        double executionTime = (endTime - startTime) * 1000;
        UnityEngine.Debug.Log($"Dungeon Layout : {executionTime} ms");
    }

    public void BSPFinish()
    {
        endTime = Time.realtimeSinceStartup;
        double executionTime = (endTime - startTime) * 1000;
        UnityEngine.Debug.Log($"BSP : {executionTime} ms");
    }

    public void RandomWalkFinish()
    {
        endTime = Time.realtimeSinceStartup;
        double executionTime = (endTime - startTime) * 1000;
        UnityEngine.Debug.Log($"Random Walk : {executionTime} ms");
    }

    public void Finish()
    {
        endTime = Time.realtimeSinceStartup;
        double executionTime = (endTime - startTime) * 1000;
        UnityEngine.Debug.Log($"Execution time : {executionTime} ms");
        MeasureMemory.isActive = false;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        TextWriter tw = new StreamWriter(Application.dataPath + "/Logfile.txt", true);
        tw.WriteLine($"[{System.DateTime.Now}] " + logString);
        tw.Close();
    }
}

public static class MeasureMemory
{
    public static bool isActive = false;
    public static void MeasureMethodMemoryUsage(Action method)
    {
        long memoryBefore = GC.GetTotalMemory(false);
        method();
        long memoryAfter = GC.GetTotalMemory(false);
        long memoryUsed = memoryAfter - memoryBefore;
        UnityEngine.Debug.Log($"{method} Memory Used : {FormatBytes(memoryUsed)}");
    }

    static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB" };
        int suffixIndex = 0;
        double size = bytes;

        while (size >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            size /= 1024;
            suffixIndex++;
        }

        return String.Format("{0:0.##} {1}", size, suffixes[suffixIndex]);
    }
}
