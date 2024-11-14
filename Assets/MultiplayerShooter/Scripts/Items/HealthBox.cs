using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : Item
{
    [SerializeField] private int _healAmount;
    public override void Pickup(PlayerModel playerModel)
    {
        playerModel.AddHeal(_healAmount);
        Runner.Despawn(Object);
    }
}
