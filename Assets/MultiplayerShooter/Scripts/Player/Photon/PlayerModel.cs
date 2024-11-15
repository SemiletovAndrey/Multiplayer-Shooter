using Fusion;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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

    private WeaponPhotonManager _weaponPhotonManager;
    private PlayerDataConfig _playerDataConfig;
    private PlayerAliveManager _aliveManager;
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
    [Networked] private int ActiveWeaponIndex { get; set; }
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

    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnKillCountChanged;
    public event Action OnDeathPlayer;


    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            CurrentHP = MaxHP;
            Kills = 0;

            IsAlive = true;
        }
    }

    public void Init(PlayerDataConfig playerDataConfig, WeaponPhotonManager weaponPhotonManager, PlayerAliveManager playerAliveManager)
    {
        _playerDataConfig = playerDataConfig;
        _weaponPhotonManager = weaponPhotonManager;
        _aliveManager = playerAliveManager;
        if (Object.HasStateAuthority)
        {
            Nickname = _playerDataConfig.NicknamePlayer;
            int randomIndexCharacter = _playerDataConfig.IndexSkin;
            ActiveSkinIndex = randomIndexCharacter;

            int randomIndexWeapon = _weaponPhotonManager.AssignUniqueWeaponIndex(Object.InputAuthority);
            ActiveWeaponIndex = randomIndexWeapon;

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
                    else
                    {
                        Debug.LogWarning("CameraFollower component not found on the main camera.");
                    }
                }
                else
                {
                    Debug.LogWarning("Main camera not found.");
                }
            }
            else
            {
                Debug.Log("No other live players available to switch the camera.");
            }
        }
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

    private void OnIsAliveChanged()
    {
        HandleDeath();
        OnDeathPlayer?.Invoke();
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
