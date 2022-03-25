using Unity.Entities;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

    public class PlayerMovementInputSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            InputSystem.onEvent += OnInputSystemEvent;
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
            Entities.WithAll<PlayerMovementInputComponent>().ForEach((
                Entity entity,
                ref GameCharacterMovementComponent controller) =>
            {
                ProcessMovement(ref controller, inputEventPtr, inputDevice);
            });
        }

        private void ProcessMovement(
            ref GameCharacterMovementComponent controller,
            InputEventPtr inputEventPtr, 
            InputDevice inputDevice)
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
                //IsKeyboard = true;

                //if (keyboard.spaceKey.ReadValueFromEvent(inputEventPtr) != 0)
                //    PlayerModel.SetPlayerReady();

                //if (!PlayerModel.StartScreenViewController.IsGameReady)
                //    return;

                targetThrottle = keyboard.wKey.ReadValueFromEvent(inputEventPtr);
                targetSteer = keyboard.dKey.ReadValueFromEvent(inputEventPtr) - keyboard.aKey.ReadValueFromEvent(inputEventPtr); ;
                targetBrake = keyboard.sKey.ReadValueFromEvent(inputEventPtr);
			
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
                targetThrottle = gamepad.leftTrigger.ReadValueFromEvent(inputEventPtr);
                targetBrake = gamepad.leftShoulder.ReadValueFromEvent(inputEventPtr);
			
                turrentRotationDir = gamepad.rightStick.ReadValueFromEvent(inputEventPtr).x;
                //turrentShoot = gamepad.rightTrigger.ReadValueFromEvent(inputEventPtr) > 0.3f;
			
                //rocketShoot = gamepad.rightShoulder.ReadValueFromEvent(inputEventPtr) > 0f;

                boosterUsage = gamepad.crossButton.ReadValueFromEvent(inputEventPtr) > 0 ||
                               gamepad.aButton.ReadValueFromEvent(inputEventPtr) > 0;
			
                jumpUsage = gamepad.circleButton.ReadValueFromEvent(inputEventPtr) > 0 ||
                            gamepad.bButton.ReadValueFromEvent(inputEventPtr) > 0;
            }
            
            float movementX = targetSteer;
            float movementZ = targetThrottle - targetBrake;
            float boost = boosterUsage ? 1.5f : 1.0f;
            
            controller.HorizontalAxis = movementX;
            controller.VerticalAxis = movementZ * boost;
            controller.SpaceKey = jumpUsage;
        }

        protected override void OnUpdate()
        {
            
        }
    }

