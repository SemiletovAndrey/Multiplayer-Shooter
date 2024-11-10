using Fusion;
using System;
using UnityEngine;
using Zenject;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private Transform[] _skins;
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private PlayerDeath _playerDeath;

    [Networked] public string Nickname { get; set; }
    [Networked] public int HP { get; set; }
    [Networked] public int Kills { get; set; }
    [Networked] public int ActiveSkinIndex { get; set; }
    [Networked,OnChangedRender(nameof(OnIsAliveChanged))] public bool IsAlive { get; set; }
    [Networked] public Weapon ActiveWeapon { get; set; }

    public static event Action<PlayerRef> OnPlayerDeath;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            HP = 15;
            Kills = 0;
            int randomIndexCharacter = UnityEngine.Random.Range(0, 3);
            ActiveSkinIndex = randomIndexCharacter;
            IsAlive = true;
        }
        SetActiveSkin(ActiveSkinIndex);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        HP -= damage;
        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TO DO Camera 
        if (!Object.HasStateAuthority) return;

        IsAlive = false;

        OnPlayerDeath?.Invoke(Object.InputAuthority);
    }

    private void OnIsAliveChanged()
    {
        HandleDeath(); 
    }

    private void HandleDeath()
    {
        if (!IsAlive)
        {
            _playerDeath.Death();
            _playerAnimation.PlayDie();
        }
    }

    public void SetActiveSkin(int skinIndex)
    {
        _playerAnimation.Animator = _skins[ActiveSkinIndex].gameObject.GetComponent<Animator>();
        ApplySkin(skinIndex);
    }

    private void ApplySkin(int skinIndex)
    {
        for (int i = 0; i < _skins.Length; i++)
        {
            _skins[i].gameObject.SetActive(i == skinIndex);
        }
    }
}
