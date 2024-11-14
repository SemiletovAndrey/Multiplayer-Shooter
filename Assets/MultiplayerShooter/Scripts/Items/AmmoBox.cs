using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : Item
{
    [SerializeField] private int ammoCount;

    public override void Pickup(PlayerModel playerModel)
    {
        playerModel.AddAmmo(ammoCount);
        Runner.Despawn(Object);
    }
}
