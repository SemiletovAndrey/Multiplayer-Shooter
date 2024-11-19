using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public abstract class Enemy : NetworkBehaviour
{
    [SerializeField] protected float TimeShowDeathAnimation = 10f;
    [SerializeField] protected float Hp;
    [SerializeField] protected int Damage;
    [SerializeField] protected float Speed;
    [SerializeField] protected float TimeAttackSpeed;
    [SerializeField] protected float AttackRange;
    [SerializeField] private Transform _visualObject;
    [SerializeField] protected EnemyAnimation EnemyAnimation;
    [SerializeField] private float detectionRadius = 10f;

    protected Transform PlayerTransform;

    protected Rigidbody2D Rigidbody2D;

    [Networked, OnChangedRender(nameof(UpdateDirection))] protected bool IsFacingLeft { get; set; }
    [Networked, OnChangedRender(nameof(OnIsRunningChanged))] protected bool IsRunning { get; set; }
    [Networked, OnChangedRender(nameof(OnIsStandChanged))] protected bool IsStand { get; set; }
    [Networked, OnChangedRender(nameof(OnIsAlive))] public bool IsDead { get; set; }
    [Networked, OnChangedRender(nameof(OnAttackChanged))] public bool IsAttackingAnimation { get; set; }

    [Networked] protected float AttackCooldown { get; set; }

    private PhysicsScene2D _physicsScene;

    private bool prevIsFaceLeft;

    public virtual void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            _physicsScene = Runner.GetPhysicsScene2D();
            prevIsFaceLeft = true;
            AttackCooldown = TimeAttackSpeed;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsDead)
        {
            FindClosestPlayer();
            MoveTowardsPlayer();
            UpdateFacingDirection();
            if (AttackCooldown > 0)
            {
                AttackCooldown -= Runner.DeltaTime;
            }
            else if (IsStand)
            {
                CheckAttack();
            }
        }
    }

    protected void FindClosestPlayer()
    {
        if (PlayerTransform != null) return;

        int playerLayer = 1 << LayerMask.NameToLayer("Player");
        Collider2D hitColliders = _physicsScene.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (hitColliders == null)
        {
            return;
        }

        Transform closestPlayer = null;
        if (hitColliders.GetComponent<NetworkCharacterController2D>().enabled == true)
        {
            closestPlayer = hitColliders.transform;
        }
        else
        {
            PlayerTransform = null;
        }

        if (closestPlayer != null)
        {
            PlayerTransform = closestPlayer;
        }
    }


    public void TakeDamage(int damage, PlayerModel playerModel)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Die();
            playerModel.IncreaseKillCount();
        }
    }

    protected void MoveTowardsPlayer()
    {
        if (PlayerTransform == null)
        {
            IsStand = false;
            StopMoving();
            return;
        }

        Vector2 direction = (PlayerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);


        RpcSetFacingDirection(direction);


        if (distanceToPlayer > AttackRange)
        {
            Rigidbody2D.velocity = direction * Speed;
            IsRunning = true;
            IsStand = false;
        }
        else
        {
            StopMoving();
            IsRunning = false;
            IsStand = true;
        }
    }

    protected void StopMoving()
    {
        EnemyAnimation.SetStand(IsStand);
        Rigidbody2D.velocity = Vector2.zero;
    }

    protected void CheckAttack()
    {
        if (PlayerTransform == null) return;

        if (Vector2.Distance(transform.position, PlayerTransform.position) <= AttackRange && AttackCooldown <= 0)
        {
            IsAttackingAnimation = true;
            Attack();
            AttackCooldown = TimeAttackSpeed;
        }
    }

    protected virtual void Attack()
    {
        if (IsAttackingAnimation)
        {
            EnemyAnimation.SetHit(IsAttackingAnimation);
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        EnemyAnimation.SetDie(IsDead);
        GetComponent<Rigidbody2D>().simulated = false;

        StartCoroutine(DieCoroutine());
    }

    protected void UpdateFacingDirection()
    {
        if (PlayerTransform != null)
        {
            IsFacingLeft = PlayerTransform.position.x < transform.position.x;
            UpdateDirection();
        }
    }

    private void UpdateDirection()
    {
        _visualObject.localScale = new Vector3(IsFacingLeft ? -1 : 1, 1, 1);
    }

    private void OnIsRunningChanged()
    {
        EnemyAnimation.SetRunning(IsRunning);
    }

    private void OnIsStandChanged()
    {
        EnemyAnimation.SetStand(IsStand);
    }
    private void OnIsAlive()
    {
        EnemyAnimation.SetDie(IsDead);
    }

    private bool DirectionEnemy(Vector2 direction)
    {
        if (direction.x < 0)
        {
            return true;
        }
        else if (direction.x > 0)
        {
            return false;
        }
        return false;
    }

    private void OnAttackChanged()
    {
        EnemyAnimation.SetHit(IsAttackingAnimation);
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(TimeShowDeathAnimation);
        if (Runner != null && Object != null)
        {
            Runner.Despawn(Object);
        }
        else
        {
            Debug.LogWarning("Runner or Object is null in DieCoroutine.");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcSetFacingDirection(Vector2 direction)
    {
        IsFacingLeft = DirectionEnemy(direction);
        prevIsFaceLeft = IsFacingLeft;
        UpdateDirection();
    }
}
