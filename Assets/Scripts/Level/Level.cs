using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "LudumDare-51/Level", order = 0)]
public class Level : ScriptableObject
{
    public List<LevelBlock> phases;
}

// 10 seconds worth of interactions
[Serializable] public struct LevelBlock {
    public Conversation conversation;
    [SerializeField] private List<SpawnObject> spawns;

    public List<SpawnObject> GetSpawns() {
        return new List<SpawnObject>(spawns);
    }
}

[Serializable] public struct SpawnObject {
    public float time;
    public Vector3 position;
    public GameObject obj;
}