using Fusion;
using System;
using System.Linq;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    [SerializeField] private Transform[] _skins;
    [SerializeField] private Weapon[] _weapons;
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private PlayerDeath _playerDeath;
    [SerializeField] public int MaxHP;

    private int _health;
    private int _kills;
    private bool _isAlive;

    private PlayerDataConfig _playerDataConfig;
    private WeaponPhotonManager _weaponPhotonManager;
    private PlayerAliveManager _aliveManager;

    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnKillCountChanged;
    public event Action OnDeathPlayer;

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
    [Networked, OnChangedRender(nameof(OnSkinIndexChanged))] public int ActiveSkinIndex { get; set; }
    [Networked] public int ActiveWeaponIndex { get; set; }
    [Networked, HideInInspector] public int AllDamage { get; set; }
    [Networked, OnChangedRender(nameof(OnIsAliveChanged)), HideInInspector]
    public bool IsAlive
    {
        get => _isAlive;
        set
        {
            if (_isAlive != value)
            {
                _isAlive = value;
                OnDeathPlayer?.Invoke();
            }
        }
    } 
    [Networked, HideInInspector] public Weapon ActiveWeapon { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            _playerDataConfig = FindObjectOfType<PlayerDataConfigMD>()?.PlayerDataConfig;

            if (_playerDataConfig != null)
            {
                if (Object.HasStateAuthority)
                {
                    InitializePlayerData();
                }
                else
                {
                    RPC_SetNickname(_playerDataConfig.NicknamePlayer, _playerDataConfig.IndexSkin);
                }
            }
        }

        if (Object.HasStateAuthority)
        {
            if (!IsAlive)
            {
                IsAlive = true;
            }

            AssignUniqueWeapon();
        }

        SetActiveSkin(ActiveSkinIndex);
        SetActiveWeapon(ActiveWeaponIndex);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, MaxHP);
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (!Object.HasStateAuthority) return;
        _aliveManager = FindObjectOfType<PlayerAliveManager>();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        IsAlive = false;

        if (_aliveManager != null && Runner.IsServer)
        {
            _aliveManager.OnPlayerDeath(Object.InputAuthority);

            RPC_HandleDeath(Object.InputAuthority);
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_HandleDeath(PlayerRef deadPlayerRef)
    {
        if (Object.InputAuthority == deadPlayerRef)
        {
            var nextAlivePlayer = _aliveManager?.AliveCharacters
                .Where(entry => entry.Key != Object.InputAuthority)
                .Select(entry => entry.Value)
                .FirstOrDefault();

            if (nextAlivePlayer != null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    var cameraFollower = mainCamera.GetComponent<CameraFollower>();
                    if (cameraFollower != null)
                    {
                        cameraFollower.CameraFollow(nextAlivePlayer.transform);
                    }
                }
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetNickname(string nickname, int activeSkinIndex)
    {
        Nickname = nickname;
        ActiveSkinIndex = activeSkinIndex;
        CurrentHP = MaxHP;
        IsAlive = true;
    }

    private void InitializePlayerData()
    {
        Nickname = _playerDataConfig.NicknamePlayer;
        CurrentHP = MaxHP;
        IsAlive = true;
        ActiveSkinIndex = _playerDataConfig.IndexSkin;
    }

    private void OnHealthChangedMethod()
    {
        OnHealthChanged?.Invoke(CurrentHP, MaxHP);
    }

    private void OnKillCountChangedMethod()
    {
        OnKillCountChanged?.Invoke(Kills);
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

    private void AssignUniqueWeapon()
    {
        _weaponPhotonManager = FindObjectOfType<WeaponPhotonManager>();

        if (_weaponPhotonManager != null && Object.HasStateAuthority)
        {
            int assignedWeaponIndex = _weaponPhotonManager.AssignUniqueWeaponIndex(Object.InputAuthority);

            if (assignedWeaponIndex >= 0)
            {
                ActiveWeaponIndex = assignedWeaponIndex;
                SetActiveWeapon(ActiveWeaponIndex);
            }
        }
    }

    private void OnIsAliveChanged()
    {
        HandleDeath();
        OnDeathPlayer?.Invoke();
    }

    private void OnSkinIndexChanged()
    {
        SetActiveSkin(ActiveSkinIndex);
    }

    private void HandleDeath()
    {
        if (!IsAlive)
        {
            _playerDeath.Death();
            _playerAnimation.PlayDie();
        }
    }
}
