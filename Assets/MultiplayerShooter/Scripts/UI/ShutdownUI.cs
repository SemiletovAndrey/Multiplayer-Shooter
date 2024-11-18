using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShutdownUI : MonoBehaviour
{
    public void ShutdownLogic()
    {
        SceneManager.LoadSceneAsync("Lobby");
    }
}
