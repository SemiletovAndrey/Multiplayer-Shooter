using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _weaponText;
    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private TextMeshProUGUI _timeWaveText;
    [SerializeField] private TextMeshProUGUI _deathText;
    [SerializeField] private TextMeshProUGUI _statusWaveText;

    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private PlayerModel _playerModel;

    private void OnEnable()
    {
        UpdateHealthUI(_playerModel.CurrentHP, _playerModel.MaxHP);
        UpdateKillCountUI(_playerModel.Kills);
        UpdateAmmoUI(_playerModel.ActiveWeapon.Ammo, _playerModel.ActiveWeapon.MaxAmmo);
        UpdateTimerWaveUI(_waveManager.TimeWave);
        UpdateStatusWave(_waveManager.WaveStatus);

        _deathText.gameObject.SetActive(false);
    }

    public void InitializePlayerUI(PlayerModel playerModel)
    {
        _playerModel = playerModel;
        _playerModel.OnHealthChanged += UpdateHealthUI;
        _playerModel.OnKillCountChanged += UpdateKillCountUI;
        _playerModel.ActiveWeapon.OnAmmoChanged += UpdateAmmoUI;
        _playerModel.OnDeathPlayer += UpdateIsAlivePlayer;
        _waveManager.OnTimeChanged += UpdateTimerWaveUI;
        _waveManager.OnStatusChanged += UpdateStatusWave;
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
    
    public void UpdateStatusWave(string waveStatus)
    {
        _statusWaveText.text = $"{waveStatus}";
    }
    
    public void UpdateTimerWaveUI(float timeWave)
    {
        int minutes = Mathf.FloorToInt(timeWave / 60);  
        int seconds = Mathf.FloorToInt(timeWave % 60);

        _timeWaveText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void UpdateIsAlivePlayer()
    {
        _deathText.gameObject.SetActive(true);
        _healthText .gameObject.SetActive(false);
    }
}
