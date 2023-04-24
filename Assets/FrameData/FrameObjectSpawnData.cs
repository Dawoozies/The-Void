using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameObjectSpawnData : ScriptableObject
{
    public AnimationClip animationClip;
    public List<ObjectSpawnData> dataListObjectSpawnData;

    public List<GameObject> ObjectsToSpawnAtFrame(int frame)
    {
        return dataListObjectSpawnData[frame].objectsToSpawn;
    }
    public List<Vector3> SpawnPositionsAtFrame(int frame)
    {
        return dataListObjectSpawnData[frame].spawnPositions;
    }
}
[System.Serializable]
public class ObjectSpawnData
{
    public List<GameObject> objectsToSpawn;
    public List<Vector3> spawnPositions;
}
