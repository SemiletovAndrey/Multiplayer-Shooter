using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _weaponText;
    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private WaveManager _waveManager;

    [SerializeField] private PlayerData _playerData;

    private void OnEnable()
    {
        UpdateHealthUI(_playerData.CurrentHP, _playerData.MaxHP);
        UpdateKillCountUI(_playerData.Kills);
    }

    public void InitializePlayerUI(PlayerData playerData)
    {
        _playerData = playerData;
        _playerData.OnHealthChanged += UpdateHealthUI;
        _playerData.OnKillCountChanged += UpdateKillCountUI;
        //_playerData.ActiveWeapon.OnAmmoChanged += UpdateAmmoUI;
    }

    public void UpdateHealthUI(int currentHP, int maxHP)
    {
        _healthText.text = $"HP {currentHP}/{maxHP}";
    }
    
    public void UpdateKillCountUI(int kills)
    {
        _killsText.text = $"Kills: {kills}";
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        _weaponText.text = $"Ammo: {currentAmmo}/{maxAmmo}";
    }
}
