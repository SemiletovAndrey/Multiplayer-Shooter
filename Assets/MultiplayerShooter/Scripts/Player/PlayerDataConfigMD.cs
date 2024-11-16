using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerDataConfigMD : MonoBehaviour
{
    [Inject] public PlayerDataConfig _playerDataConfig;

    public PlayerDataConfig PlayerDataConfig { get { return _playerDataConfig; } }
}
