using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Item
{
    [SerializeField] private int damageAmount;
    [SerializeField] private float explosionRadius;
    [SerializeField] private GameObject visualShadowBomb;
    [SerializeField] private float ShowBombShadow = 1f;

    public override void Pickup(PlayerModel playerModel)
    {
        Explode(playerModel);
        visualShadowBomb.gameObject.SetActive(true);
        visualShadowBomb.transform.localScale = new Vector3(visualShadowBomb.transform.localScale.x * explosionRadius,
            visualShadowBomb.transform.localScale.y * explosionRadius, 0);
        StartCoroutine(DeleteBomb());
    }

    private void Explode(PlayerModel playerModel)
    {
        var allNetworkObjects = Runner.GetAllNetworkObjects();

        Vector3 explosionPosition = transform.position;

        foreach (var networkObject in allNetworkObjects)
        {
            if (networkObject.TryGetComponent(out Enemy enemy))
            {
                float distance = Vector3.Distance(explosionPosition, networkObject.transform.position);
                if (distance <= explosionRadius)
                {
                    enemy.TakeDamage(damageAmount, playerModel);
                }
            }
        }
    }

    private IEnumerator DeleteBomb()
    {
        yield return new WaitForSeconds(ShowBombShadow);
        Runner.Despawn(Object);
    }
}
