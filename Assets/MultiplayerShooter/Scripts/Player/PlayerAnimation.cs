using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private static readonly int RunHash = Animator.StringToHash("IsRunner");
    private static readonly int StandHash = Animator.StringToHash("StandTrigger");
    private static readonly int DieHash = Animator.StringToHash("DieTrigger");
    private static readonly int ResurrectionHash = Animator.StringToHash("ResurrectionTrigger");

    public void SetRunning(bool isRunning) => _animator.SetBool(RunHash, isRunning);
    public void PlayStand() => _animator.SetTrigger(StandHash);
    public void PlayDie() => _animator.SetTrigger(DieHash);
    public void PlayResurrection() => _animator.SetTrigger(ResurrectionHash);
}
