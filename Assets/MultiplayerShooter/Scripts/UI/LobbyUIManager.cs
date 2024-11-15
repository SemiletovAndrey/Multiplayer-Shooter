using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LobbyUIManager : MonoBehaviour
{
    [Inject] private PlayerDataConfig _playerDataConfig;

    [SerializeField] private GameObject _startWindow;
    [SerializeField] private GameObject _changeSkinWindow;
    [SerializeField] private TMP_InputField _inputField;

    private int _skinIndex;

    private void Start()
    {
        if (_inputField != null)
        {
            _inputField.onEndEdit.AddListener(OnInputFieldChanged);
        }
    }

    private void OnDestroy()
    {
        if (_inputField != null)
        {
            _inputField.onEndEdit.RemoveListener(OnInputFieldChanged);
        }
    }

    public void ChangeSkinWindow()
    {
        _startWindow.SetActive(false);
        _changeSkinWindow.SetActive(true);
    }

    public void BackStartMenu()
    {
        _startWindow.SetActive(true);
        _changeSkinWindow.SetActive(false);
    }

    public void ChoseSkin()
    {
        _playerDataConfig.IndexSkin = _skinIndex;
        PlayerPrefs.SetInt("Nickname", _skinIndex);
    }

    public void SetSkin(int skinIndex)
    {
        _skinIndex = skinIndex;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnInputFieldChanged(string newText)
    {
        PlayerPrefs.SetString("Nickname", newText);
        _playerDataConfig.NicknamePlayer = newText;
    }
}
