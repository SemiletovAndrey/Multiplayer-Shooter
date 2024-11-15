using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Zenject;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private LobbyUIManager _lobbyUIManager;
    [SerializeField] private TMP_InputField _inputField;

    [Inject] private PlayerDataConfig _playerDataConfig;

    private void Start()
    {
        string nick = PlayerPrefs.GetString("Nickname");
        int indexSkin = PlayerPrefs.GetInt("IndexSkin");
        _inputField.text = nick;
        _playerDataConfig.NicknamePlayer = nick;
        _playerDataConfig.IndexSkin = indexSkin;
    }
}
