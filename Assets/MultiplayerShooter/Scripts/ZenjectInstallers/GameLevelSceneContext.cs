using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class GameLevelSceneContext : MonoInstaller
{
    [SerializeField] private WeaponManager _weaponManager;
    public override void InstallBindings()
    {
        Container.Bind<WeaponManager>().FromInstance(_weaponManager).AsSingle();
    }
}
