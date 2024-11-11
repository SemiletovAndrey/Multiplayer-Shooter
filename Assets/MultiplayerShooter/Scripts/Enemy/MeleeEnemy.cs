using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float timeToDealDamage = 1.0f;
    private float timeInRange = 0.0f;

    protected override void Attack()
    {
        base.Attack();

        if (PlayerTransform.gameObject.TryGetComponent<PlayerData>(out PlayerData data))
        {
            Debug.Log($"IsAlive {data.IsAlive}");
            if (data.IsAlive)
            {
                Debug.Log("Attack");
                data.TakeDamage(Damage);
            }
            else
            {
                PlayerTransform = null;
                Debug.Log($"PlayerTransform {PlayerTransform}");
            }
        }

    }

    private void UpdateAttackLogic()
    {
        if (PlayerTransform == null) return;

        if (Vector2.Distance(transform.position, PlayerTransform.position) <= AttackRange)
        {
            timeInRange += Runner.DeltaTime;

            if (timeInRange >= timeToDealDamage && AttackCooldown <= 0)
            {
                Attack();
                IsAttacking = true;
                AttackCooldown = TimeAttackSpeed;
                timeInRange = 0.0f;
            }
        }
        else
        {
            timeInRange = 0.0f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsDead)
        {
            FindClosestPlayer();
            MoveTowardsPlayer();
            if (AttackCooldown > 0)
            {
                AttackCooldown -= Runner.DeltaTime;
            }
            else if (IsStand)
            {
                UpdateAttackLogic();
            }
        }
    }
}
