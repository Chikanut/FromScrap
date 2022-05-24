using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CarViewImage : MonoBehaviour
{
    [SerializeField] private float _cameraDistance = 8;
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private float _rotationSpeed = 0.8f;
    
    protected void OnEnable()
    {
        InputSystem.onEvent += OnInputSystemEvent;

        CarViewCamera.Instance.InitSettings(_cameraDistance, _rotationSpeed, _offset);
    }

    protected void OnDisable()
    {
        InputSystem.onEvent -= OnInputSystemEvent;
    }

    private void OnInputSystemEvent(InputEventPtr inputEventPtr, InputDevice inputDevice)
    {
        var input = Vector2.zero;

        switch (inputDevice)
        {
            case Keyboard keyboard:
                input.y = keyboard.wKey.ReadValueFromEvent(inputEventPtr) - keyboard.sKey.ReadValueFromEvent(inputEventPtr);
                input.x = -(keyboard.dKey.ReadValueFromEvent(inputEventPtr) - keyboard.aKey.ReadValueFromEvent(inputEventPtr));
                break;
            case Gamepad gamepad:
                input.x = -gamepad.leftStick.ReadValueFromEvent(inputEventPtr).x;
                input.y = gamepad.leftStick.ReadValueFromEvent(inputEventPtr).y;
                break;
        }
        
        CarViewCamera.Instance.Input(input);
    }
}
