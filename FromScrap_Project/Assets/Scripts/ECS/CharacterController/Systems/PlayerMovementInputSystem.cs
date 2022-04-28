using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Vehicles.Components;

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
        }

        private void UpdatePlayerControls(InputEventPtr inputEventPtr, InputDevice inputDevice)
        {
            Entities.WithAll<PlayerMovementInputComponent>().ForEach((Entity entity, ref CharacterControllerInternalData controller, in LocalToWorld localToWorld) =>
            {
                ProcessCharacterMovement(ref controller, inputEventPtr, inputDevice, localToWorld);
            }).WithoutBurst().Run();
            
            Entities.WithAll<PlayerMovementInputComponent>().ForEach((Entity entity, ref VehicleInputComponent vehicleComponent, in LocalToWorld localToWorld) =>
            {
                ProcessVehicleMovement(ref vehicleComponent, inputEventPtr, inputDevice, localToWorld);
            }).WithoutBurst().Run();
        }

        private void ProcessCharacterMovement(
            ref CharacterControllerInternalData controller,
            InputEventPtr inputEventPtr, 
            InputDevice inputDevice, LocalToWorld localToWorld)
        {
            var targetSteer = 0f;
            var targetThrottle = 0f;

            switch (inputDevice)
            {
                case Keyboard keyboard:
                    targetThrottle = keyboard.wKey.ReadValueFromEvent(inputEventPtr) - keyboard.sKey.ReadValueFromEvent(inputEventPtr);;
                    targetSteer = keyboard.dKey.ReadValueFromEvent(inputEventPtr) - keyboard.aKey.ReadValueFromEvent(inputEventPtr); ;
                    break;
                case Gamepad gamepad:
                    targetSteer = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).x;
                    targetThrottle = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).y;
                    break;
            }

            var dir = new float3(targetSteer, 0, targetThrottle);
            var forward = localToWorld.Forward;
            var angle = math.clamp(dir.AngleSigned(forward, math.up())/180, -1, 1);

            controller.Input.Movement = new float2(dir.x, dir.z);
            controller.Input.Rotation = -angle;
        }

        private void ProcessVehicleMovement(ref VehicleInputComponent vehicleComponent,
            InputEventPtr inputEventPtr,
            InputDevice inputDevice, LocalToWorld localToWorld)
        {
            var targetSteer = 0f;
            var targetThrottle = 0f;

            switch (inputDevice)
            {
                case Keyboard keyboard:
                    targetThrottle = keyboard.wKey.ReadValueFromEvent(inputEventPtr) - keyboard.sKey.ReadValueFromEvent(inputEventPtr);;
                    targetSteer = keyboard.dKey.ReadValueFromEvent(inputEventPtr) - keyboard.aKey.ReadValueFromEvent(inputEventPtr); ;
                    break;
                case Gamepad gamepad:
                    targetSteer = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).x;
                    targetThrottle = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).y;
                    break;
            }

            var dir = new float3(targetSteer, 0, targetThrottle);
            vehicleComponent.MoveDir = dir;
            // ECS_Math_Extensions.SmoothDamp(vehicleComponent.MoveDir, dir,
            //     ref vehicleComponent.MoveDirVelocity, 1f, float.MaxValue, Time.DeltaTime);
        }

        protected override void OnUpdate()
        {
            
        }
    }

