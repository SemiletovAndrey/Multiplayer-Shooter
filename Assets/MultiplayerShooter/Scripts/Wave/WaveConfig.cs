using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveConfig
{
    public float WaveDuration = 30f;
    public float SpawnInterval = 5f;
    public float TimeBetweenWaves = 10f;
    public List<GameObject> EnemyTypes;
}
