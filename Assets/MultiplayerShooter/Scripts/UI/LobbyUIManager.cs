using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LobbyUIManager : MonoBehaviour  
{
    [Inject] private PlayerDataConfig _playerDataConfig;

    [SerializeField] private GameObject _startWindow;
    [SerializeField] private GameObject _changeSkinWindow;
    [SerializeField] private GameObject _loadingWindow;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private TMP_InputField _inputField;

    private int _skinIndex;
    private NetworkRunner _runner;

    private void Start()
    {
        if (_inputField != null)
        {
            _inputField.onValueChanged.AddListener(OnInputFieldChanged);
        }
    }

    private void OnDestroy()
    {
        if (_inputField != null)
        {
            _inputField.onValueChanged.RemoveListener(OnInputFieldChanged);
        }
    }

    public void ChangeSkinWindow()
    {
        _startWindow.SetActive(false);
        _changeSkinWindow.SetActive(true);
    }

    public void CreateRoom()
    {
        _startWindow.SetActive(false);
        _playerDataConfig.gameMode = GameMode.Host;
        StartGameHost();
    }
    
    public void JoinRoom()
    {
        _startWindow.SetActive(false);
        _playerDataConfig.gameMode = GameMode.Client;
        StartGameHost();
    }

    public void BackStartMenu()
    {
        _startWindow.SetActive(true);
        _changeSkinWindow.SetActive(false);
    }

    public void ChoseSkin()
    {
        _playerDataConfig.IndexSkin = _skinIndex;
        _startWindow.SetActive(true);
        _changeSkinWindow.SetActive(false);
        PlayerPrefs.SetInt("IndexSkin", _skinIndex);
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
        _playerDataConfig.NicknamePlayer = newText;
        PlayerPrefs.SetString("Nickname", newText);

    }

    private void StartGameHost()
    {
        _loadingWindow.SetActive(true);
        SceneManager.LoadSceneAsync("GameLevel");
    }
}
