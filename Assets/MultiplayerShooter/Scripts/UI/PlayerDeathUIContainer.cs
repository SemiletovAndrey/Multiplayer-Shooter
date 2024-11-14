using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathUIContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nicknamePlayerText;
    [SerializeField] private TextMeshProUGUI _killsPlayerText;
    [SerializeField] private TextMeshProUGUI _damagePlayerText;

    [SerializeField] private Image _statusPlayerLiveImages;
    [SerializeField] private Image _statusPlayerDeadImages;
    [SerializeField] private Image _playerSkinsImage;
    [SerializeField] private List<Sprite> _playerSkinsSprites;

    [SerializeField] private GameObject _youContainer;

    private PlayerModel _playerModel;

    private void InitPlayerDataUI(bool isLocalPlayer)
    {
        _nicknamePlayerText.text = _playerModel.Nickname;
        _killsPlayerText.text = _playerModel.Kills.ToString();
        _damagePlayerText.text = _playerModel.AllDamage.ToString();

        _statusPlayerLiveImages.gameObject.SetActive(_playerModel.IsAlive);
        _statusPlayerDeadImages.gameObject.SetActive(!_playerModel.IsAlive);

        _playerSkinsImage.sprite = _playerSkinsSprites[_playerModel.ActiveSkinIndex];

        _youContainer.SetActive(isLocalPlayer);
    }

    public void InitializePlayerDeathUI(PlayerModel playerModel, bool isLocalPlayer)
    {
        _playerModel = playerModel;
        InitPlayerDataUI(isLocalPlayer);
    }
}
