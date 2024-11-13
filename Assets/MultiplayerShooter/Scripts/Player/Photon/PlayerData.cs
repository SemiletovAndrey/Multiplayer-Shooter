using Fusion;
using System;
using UnityEngine;
using Zenject;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private Transform[] _skins;
    [SerializeField] private Weapon[] _weapons;
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private PlayerDeath _playerDeath;
    [SerializeField] public int MaxHP;

    private int _health;
    private int _kills;

    private WeaponPhotonManager _weaponPhotonManager;

    [Networked] public string Nickname { get; set; }
    [Networked, OnChangedRender(nameof(OnHealthChangedMethod))]
    public int CurrentHP
    {
        get => _health;
        set
        {
            if (_health != value)
            {
                _health = value;
                OnHealthChanged?.Invoke(_health, MaxHP);
            }
        }
    }
    [Networked, OnChangedRender(nameof(OnKillCountChangedMethod))]
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
    [Networked] public int ActiveWeaponIndex { get; set; }
    [Networked] public int AllDamage { get; set; }
    [Networked, OnChangedRender(nameof(OnIsAliveChanged))] public bool IsAlive { get; private set; }
    [Networked] public Weapon ActiveWeapon { get; set; }

    public static event Action<PlayerRef> OnPlayerDeath;

    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnKillCountChanged;


    public override void Spawned()
    {
        base.Spawned();
        _weaponPhotonManager = FindObjectOfType<WeaponPhotonManager>();
        if (Object.HasStateAuthority)
        {
            CurrentHP = MaxHP;
            Kills = 0;
            int randomIndexCharacter = UnityEngine.Random.Range(0, _skins.Length);
            int randomIndexWeapon = _weaponPhotonManager.AssignUniqueWeaponIndex(Object.InputAuthority);
            ActiveSkinIndex = randomIndexCharacter;
            ActiveWeaponIndex = randomIndexWeapon;
            IsAlive = true;
        }
        SetActiveSkin(ActiveSkinIndex);
        SetActiveWeapon(ActiveWeaponIndex);
        OnPlayerDeath += HandlePlayerDeath;
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
            if (i == skinIndex)
            {
                _skins[i].gameObject.SetActive(true);
                _skins[i].gameObject.GetComponent<Animator>().enabled = true;
                _skins[i].gameObject.GetComponent<NetworkMecanimAnimator>().enabled = true;
            }
            else
            {
                _skins[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetActiveWeapon(int indexWeapon)
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (i == indexWeapon)
            {
                _weapons[i].gameObject.SetActive(true);
                ActiveWeapon = _weapons[i];
                gameObject.GetComponent<ShotCharacterController>().SetCurrentWeapon(ActiveWeapon);
            }
            else
            {
                _weapons[i].gameObject.SetActive(false);
            }
        }
    }

    public void AddHeal(int healthCount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + healthCount, 0, MaxHP);
    }

    public void AddAmmo(int ammoCount)
    {
        ActiveWeapon.AddAmmo(ammoCount);
    }

    private void OnHealthChangedMethod()
    {
        OnHealthChanged?.Invoke(CurrentHP, MaxHP);
    }

    private void OnKillCountChangedMethod()
    {
        OnKillCountChanged?.Invoke(Kills);
    }

    private void HandlePlayerDeath(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            var aliveManager = FindObjectOfType<PlayerAliveManager>();
            if (aliveManager != null)
            {
                aliveManager.DiePlayerRpc();
            }
        }
    }
}
