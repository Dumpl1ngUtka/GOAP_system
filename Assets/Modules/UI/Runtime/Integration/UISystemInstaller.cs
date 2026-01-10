using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class UISystemInstaller : MonoInstaller
{
    [FormerlySerializedAs("_windowsConfig")] 
    [SerializeField] private UIConfig uiConfig;
    [SerializeField] private UIRoot _uiRoot;
    [SerializeField] private Camera _camera;
    
    public override void InstallBindings()
    {
        Container
            .BindInstance(_uiRoot)
            .AsSingle();//Не должно быть для всех
        
        Container
            .BindInstance(_camera)
            .AsSingle();//Не должно быть для всех
        
        Container
            .Bind<UISystem>()
            .AsSingle();

        Container
            .Bind<UIFactory>()
            .AsSingle()
            .WithArguments(uiConfig, _uiRoot);
    }
}