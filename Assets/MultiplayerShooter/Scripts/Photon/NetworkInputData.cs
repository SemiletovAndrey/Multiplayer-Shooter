using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction; 
    public Vector2 aimDirection;
    public bool isShooting;
}
