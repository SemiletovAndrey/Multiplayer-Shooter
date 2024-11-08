using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public abstract class Enemy : NetworkBehaviour
{
    [SerializeField] protected float Hp;
    [SerializeField] protected int Damage;
    [SerializeField] protected float Speed;
    [SerializeField] protected float TimeAttackSpeed;
    [SerializeField] protected float AttackRange;
    [SerializeField] private Transform _visualObject;
    [SerializeField] protected EnemyAnimation EnemyAnimation;

    protected Transform PlayerTransform;

    protected Rigidbody2D Rigidbody2D;

    [Networked, OnChangedRender(nameof(UpdateDirection))] protected bool IsFacingLeft { get; set; }
    [Networked, OnChangedRender(nameof(OnIsRunningChanged))] protected bool IsRunning { get; set; }
    [Networked, OnChangedRender(nameof(OnIsStandChanged))] protected bool IsStand { get; set; }
    [Networked, OnChangedRender(nameof(OnIsAlive))] protected bool IsDead { get; set; }

    [Networked] protected float AttackCooldown { get; set; }

    public virtual void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsDead)
        {
            MoveTowardsPlayer();

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

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Die();
        }
    }

    protected void MoveTowardsPlayer()
    {
        //TO DO 

        PlayerTransform = FindObjectOfType<PlayerFacade>()?.transform;

        if (PlayerTransform == null)
        {
            IsStand = false;
            StopMoving();
            return;
        }

        Vector2 direction = (PlayerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerTransform.position);
        DirectionEnemy(direction);
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
            Attack();
            AttackCooldown = TimeAttackSpeed;
        }
    }

    protected virtual void Attack()
    {
        EnemyAnimation.PlayHit();
    }

    protected virtual void Die()
    {
        IsDead = true;
        EnemyAnimation.SetDie(IsDead);
        GetComponent<Rigidbody2D>().simulated = false;
        StartCoroutine(DieCoroutine());
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

    private void DirectionEnemy(Vector2 direction)
    {
        if (direction.x < 0 && !IsFacingLeft)
        {
            IsFacingLeft = true;
        }
        else if (direction.x > 0 && IsFacingLeft)
        {
            IsFacingLeft = false;
        }
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(10f);
        Runner.Despawn(Object);
    }

}
