using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySpawner : MonoBehaviour
{
    private NetworkRunner _runner;

    private bool _isHost;
    private bool _isGameStarted;

    public async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        SceneRef nextScene = SceneRef.FromIndex(1);
        if (!nextScene.IsValid)
        {
            Debug.LogError("—цена игры недоступна!");
            return;
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = nextScene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }

    public void StartGameForAllPlayers()
    {
        if (!_isHost || _isGameStarted) return;

        _isGameStarted = true;

        _runner.SessionInfo.IsOpen = false;
        _runner.SessionInfo.IsVisible = false;


        if (_runner.IsSceneAuthority)
        {
            _runner.UnloadScene(SceneRef.FromIndex(0));
            _runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Additive);
        }

    }
}
