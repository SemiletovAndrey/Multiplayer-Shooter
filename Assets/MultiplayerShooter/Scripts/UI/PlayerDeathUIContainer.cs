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

    private PlayerModel _playerModel;

    private void OnEnable()
    {
        InitPlayerDataUI();
    }

    private void InitPlayerDataUI()
    {
        _nicknamePlayerText.text = _playerModel.Nickname;
        _killsPlayerText.text = _playerModel.Kills.ToString();
        _damagePlayerText.text = _playerModel.AllDamage.ToString();
        if (_playerModel.IsAlive)
        {
            _statusPlayerLiveImages.gameObject.SetActive(true);
            _statusPlayerDeadImages.gameObject.SetActive(false);
        }
        else
        {
            _statusPlayerDeadImages.gameObject.SetActive(true);
            _statusPlayerLiveImages.gameObject.SetActive(false);
        }
        _playerSkinsImage.GetComponent<SpriteRenderer>().sprite = _playerSkinsSprites[_playerModel.ActiveSkinIndex];
    }

    public void InitializePlayerDeathUI(PlayerModel playerModel)
    {
        _playerModel = playerModel;
    }
}
