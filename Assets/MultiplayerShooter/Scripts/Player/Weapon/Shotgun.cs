using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private float spreadAngleOffset = 5f;

    protected override float GetSpreadAngle(int bulletIndex)
    {
        return bulletIndex == 0 ? -spreadAngleOffset : spreadAngleOffset;
    }
}
