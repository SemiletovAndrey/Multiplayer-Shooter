using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveConfig
{
    [Space(10)]
    public float WaveDuration = 30f;
    public float TimeBetweenWaves = 10f;

    [Space(10)]
    public float SpawnIntervalEnemy = 5f;

    [Space(10)]
    public float MinIntervalSpawn = 5f;
    public float MaxIntervalSpawn = 10f;

    [Space(10)]
    public List<Enemy> EnemyTypes;
    public List<Item> ItemType;
}
