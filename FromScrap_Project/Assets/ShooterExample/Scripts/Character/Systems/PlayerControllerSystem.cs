using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace VertexFragment
{
    /// <summary>
    /// Main control system for player input.
    /// </summary>
    public class PlayerControllerSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            Debug.Log("Init Input System");
            InputSystem.onEvent += OnInputSystemEvent;
        }
        
        private void OnInputSystemEvent(InputEventPtr inputEventPtr, InputDevice inputDevice) {
            // Ignore anything that isn't a state event.
            if (!inputEventPtr.IsA<StateEvent>() && !inputEventPtr.IsA<DeltaStateEvent>())
                return;

            var gamepad = inputDevice as Gamepad;
            if (gamepad != null)        
                UpdatePlayerControlls(inputEventPtr, inputDevice);
               
            var keyboard =  inputDevice as Keyboard;

            if (keyboard != null)
                UpdatePlayerControlls(inputEventPtr, inputDevice);

            //var mouse = inputDevice as Mouse;
            //if (mouse != null)
            //{
            //    _players.FirstOrDefault(i => i.View.IsKeyboard)?.View.SetControls(inputEventPtr, inputDevice);
            //}
        }

        private void UpdatePlayerControlls(InputEventPtr inputEventPtr, InputDevice inputDevice)
        {
            Entities.WithAll<PlayerControllerComponent>().ForEach((
                Entity entity,
                ref CameraFollowComponent camera,
                ref CharacterControllerComponent controller) =>
            {
                ProcessMovement(ref controller, ref camera, inputEventPtr, inputDevice);
            });
        }
        
        protected override void OnUpdate()
        {
            /*
            Entities.WithAll<PlayerControllerComponent>().ForEach((
                Entity entity,
                ref CameraFollowComponent camera,
                ref CharacterControllerComponent controller) =>
            {
                ProcessMovement(ref controller, ref camera);
            });
            */
        }

        /// <summary>
        /// Processes the horizontal movement input from the player to move the entity along the xz plane.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="camera"></param>
        private void ProcessMovement(ref CharacterControllerComponent controller, ref CameraFollowComponent camera, InputEventPtr inputEventPtr, InputDevice inputDevice)
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
            
            //float movementX = (Input.GetAxis("Move Right") > 0.0f ? 1.0f : 0.0f) + (Input.GetAxis("Move Left") > 0.0f ? -1.0f : 0.0f);
            //float movementZ = (Input.GetAxis("Move Forward") > 0.0f ? 1.0f : 0.0f) + (Input.GetAxis("Move Backward") > 0.0f ? -1.0f : 0.0f);

            Vector3 forward = new Vector3(camera.Forward.x, 0.0f, camera.Forward.z).normalized;
            Vector3 right = new Vector3(camera.Right.x, 0.0f, camera.Right.z).normalized;

            if (!MathUtils.IsZero(movementX) || !MathUtils.IsZero(movementZ))
            {
                controller.CurrentDirection = ((forward * movementZ) + (right * movementX)).normalized;
                controller.CurrentMagnitude = boosterUsage ? 1.5f : 1.0f;
            }
            else
            {
                controller.CurrentMagnitude = 0.0f;
            }

            //controller.Jump = Input.GetAxis("Jump") > 0.0f;
            controller.Jump = jumpUsage;
            
            Debug.Log(movementX);
            Debug.Log(movementZ);
            Debug.Log(boosterUsage);
            Debug.Log(jumpUsage);
        }
    }
}
