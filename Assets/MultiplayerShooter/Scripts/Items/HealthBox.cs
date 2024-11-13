using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : Item
{
    [SerializeField] private int _healAmount;
    public override void Pickup(PlayerData playerData)
    {
        playerData.AddHeal(_healAmount);
        Runner.Despawn(Object);
    }
}
