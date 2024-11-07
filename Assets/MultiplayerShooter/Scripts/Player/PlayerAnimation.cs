using Fusion;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{
    public Animator Animator;

    private static readonly int RunHash = Animator.StringToHash("IsRunner");
    private static readonly int StandHash = Animator.StringToHash("StandTrigger");
    private static readonly int DieHash = Animator.StringToHash("DieTrigger");
    private static readonly int ResurrectionHash = Animator.StringToHash("ResurrectionTrigger");

    public void SetRunning(bool isRunning) => Animator.SetBool(RunHash, isRunning);
    public void PlayStand() => Animator.SetTrigger(StandHash);
    public void PlayDie() => Animator.SetTrigger(DieHash);
    public void PlayResurrection() => Animator.SetTrigger(ResurrectionHash);
}
