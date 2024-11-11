using Fusion;
using System;
using UnityEngine;
using Zenject;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private Transform[] _skins;
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private PlayerDeath _playerDeath;
    [SerializeField] public int MaxHP;

    private int _health;
    private int _kills;

    [Networked] public string Nickname { get; set; }
    [Networked, OnChangedRender(nameof(OnHealthChangedMethod))]
    public int CurrentHP
    {
        get => _health; set
        {
            if (_health != value)
            {
                _health = value;
                OnHealthChanged?.Invoke(_health, MaxHP);
            }
        }
    }
    [Networked, OnChangedRender(nameof (OnKillCountChangedMethod))]
    public int Kills
    {
        get => _kills;
        set
        {
            if (_kills != value)
            {
                _kills = value;
                OnKillCountChanged?.Invoke(_kills);
            }
        }
    }
    [Networked] public int ActiveSkinIndex { get; set; }
    [Networked] public int AllDamage { get; set; }
    [Networked, OnChangedRender(nameof(OnIsAliveChanged))] public bool IsAlive { get; private set; }
    [Networked] public Weapon ActiveWeapon { get; set; }

    public static event Action<PlayerRef> OnPlayerDeath;

    public event Action<int,int> OnHealthChanged;
    public event Action<int> OnKillCountChanged;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            CurrentHP = MaxHP;
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

        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // TO DO Camera 
        if (!Object.HasStateAuthority) return;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
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

    public void IncreaseKillCount()
    {
        if (Object.HasStateAuthority)
        {
            Kills++;
        }
    }

    public void AddAllDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            AllDamage += damage;
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

    private  void OnHealthChangedMethod()
    {
        OnHealthChanged?.Invoke(CurrentHP, MaxHP);
    }

    private void OnKillCountChangedMethod()
    {
        OnKillCountChanged?.Invoke(Kills);
    }
}
