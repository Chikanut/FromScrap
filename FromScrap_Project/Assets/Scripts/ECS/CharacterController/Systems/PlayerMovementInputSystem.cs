using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

    public partial class PlayerMovementInputSystem : SystemBase
    {
        protected override void OnCreate()
        {
            InputSystem.onEvent += OnInputSystemEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            InputSystem.onEvent -= OnInputSystemEvent;
        }

        private void OnInputSystemEvent(InputEventPtr inputEventPtr, InputDevice inputDevice) {
            if (!inputEventPtr.IsA<StateEvent>() && !inputEventPtr.IsA<DeltaStateEvent>())
                return;

            var gamepad = inputDevice as Gamepad;
            if (gamepad != null)        
                UpdatePlayerControls(inputEventPtr, inputDevice);
               
            var keyboard =  inputDevice as Keyboard;

            if (keyboard != null)
                UpdatePlayerControls(inputEventPtr, inputDevice);

            //var mouse = inputDevice as Mouse;
            //if (mouse != null)
            //{
            //    _players.FirstOrDefault(i => i.View.IsKeyboard)?.View.SetControls(inputEventPtr, inputDevice);
            //}
        }

        private void UpdatePlayerControls(InputEventPtr inputEventPtr, InputDevice inputDevice)
        {
            Entities.WithAll<PlayerMovementInputComponent>().ForEach((Entity entity, ref CharacterControllerInternalData controller, in LocalToWorld localToWorld) =>
            {
                ProcessMovement(ref controller, inputEventPtr, inputDevice, localToWorld);
            }).WithoutBurst().Run();
        }

        private void ProcessMovement(
            ref CharacterControllerInternalData controller,
            InputEventPtr inputEventPtr, 
            InputDevice inputDevice, LocalToWorld localToWorld)
        {
            float targetSteer = 0f;
            float targetThrottle = 0f;
            float targetBrake = 0f;
            float turrentRotationDir;
            bool boosterUsage = false;
            bool jumpUsage = false;
            
            var keyboard = inputDevice as Keyboard;
		
            if (keyboard != null)
            {

                targetThrottle = keyboard.wKey.ReadValueFromEvent(inputEventPtr) - keyboard.sKey.ReadValueFromEvent(inputEventPtr);;
                targetSteer = keyboard.dKey.ReadValueFromEvent(inputEventPtr) - keyboard.aKey.ReadValueFromEvent(inputEventPtr); ;
			
                turrentRotationDir = keyboard.eKey.ReadValueFromEvent(inputEventPtr) - keyboard.qKey.ReadValueFromEvent(inputEventPtr);
			
                boosterUsage = keyboard.leftShiftKey.ReadValueFromEvent(inputEventPtr) > 0;
			
                jumpUsage = keyboard.spaceKey.ReadValueFromEvent(inputEventPtr) > 0;
            }
            
            var gamepad = inputDevice as Gamepad;
		
            if (gamepad != null)
            {
                //if (gamepad.aButton.ReadValueFromEvent(inputEventPtr) != 0)
                //    PlayerModel.SetPlayerReady();

                //if (!PlayerModel.StartScreenViewController.IsGameReady)
                //    return;

                targetSteer = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).x;
                targetThrottle = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).y;
			
                turrentRotationDir = gamepad.rightStick.ReadValueFromEvent(inputEventPtr).x;

                boosterUsage = gamepad.crossButton.ReadValueFromEvent(inputEventPtr) > 0 ||
                               gamepad.aButton.ReadValueFromEvent(inputEventPtr) > 0;
			
                jumpUsage = gamepad.circleButton.ReadValueFromEvent(inputEventPtr) > 0 ||
                            gamepad.bButton.ReadValueFromEvent(inputEventPtr) > 0;
            }

            var dir = new float3(targetSteer, 0, targetThrottle);
            var forward = localToWorld.Forward;
            var angle = math.clamp(dir.AngleSigned(forward, math.up())/180, -1, 1);

            controller.Input.Movement = new float2(dir.x, dir.z);
            controller.Input.Rotation = -angle;
        }

        protected override void OnUpdate()
        {
            
        }
    }

