using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectContextInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IInputService>().To<MobileInputService>().AsSingle();
    }
    private void BindInputServices()
    {
        //if (Application.isMobilePlatform)
        //{
        //    Container.Bind<IInputService>().To<MobileInputService>().AsSingle();
        //}
        //else
        //{
        //    Container.Bind<IInputService>().To<PCInputSystem>().AsSingle();
        //}
    }

}
