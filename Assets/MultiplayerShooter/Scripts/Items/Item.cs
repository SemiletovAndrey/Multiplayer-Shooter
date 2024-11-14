using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : NetworkBehaviour
{
    public abstract void Pickup(PlayerModel playerModel);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Collision");
            Pickup(collision.gameObject.GetComponent<PlayerModel>());
        }
    }
}
