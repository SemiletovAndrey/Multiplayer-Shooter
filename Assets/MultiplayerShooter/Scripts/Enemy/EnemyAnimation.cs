using Fusion;
using UnityEngine;

public class EnemyAnimation : NetworkBehaviour
{
    public Animator Animator;

    private static readonly int RunHash = Animator.StringToHash("IsRunner");
    private static readonly int HitHash = Animator.StringToHash("HitTrigger");
    private static readonly int StandHash = Animator.StringToHash("IsStand");
    private static readonly int DieHash = Animator.StringToHash("IsDead");

    public void SetRunning(bool isRunning) => Animator.SetBool(RunHash, isRunning);
    public void SetStand(bool isStand) => Animator.SetBool(StandHash, isStand);
    public void SetDie(bool isDead) => Animator.SetBool(DieHash, isDead);
    public void PlayHit() => Animator.SetTrigger(HitHash);
}
