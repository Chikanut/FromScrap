using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace InputManager
{
    public class InputManagerInstaller : Installer<InputManagerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InputSystemManager>().AsSingle();
        }
    }

    interface IInputManager
    {
        public ControlType GetControlType();
        public ConsoleType GetConsoleType();
    }

    public enum ControlType
    {
        PC,
        Mobile,
        Console
    }

    public enum ConsoleType
    {
        none,
        PS4,
        PS5,
        SWITCH,
        XBOX
    }

    public class InputSystemManager : IInputManager, IInitializable
    {
        private ControlType _controlType = ControlType.PC;
        private ConsoleType _consoleType = ConsoleType.none;
        
        [Inject]
        public void Init()
        {

        }

        public void Initialize()
        {

        }
        
        public ControlType GetControlType()
        {
            return _controlType;
        }

        public ConsoleType GetConsoleType()
        {
            return _consoleType;
        }

    }
}
