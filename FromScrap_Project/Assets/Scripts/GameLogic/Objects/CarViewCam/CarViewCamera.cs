using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CarViewCamera : MonoBehaviour
{
    public static CarViewCamera _instance;

    public static CarViewCamera Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load("CarViewCamera") as GameObject).GetComponent<CarViewCamera>();
            }

            return _instance;
        }
    }

    private EntityManager _entityManager;
    private Entity _car;

    public Camera Camera;
    [SerializeField] private float _damping = 0.5f;
    [SerializeField] private float _distanceToTarget = 10;
    [SerializeField] private  Material _carMaterial;
    [SerializeField] private  string _textureName;
    [SerializeField] private float _rotationSpeed = 15;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _cameraVerticalAngle = -15;

    private Vector2 _rotationVelocity;
    
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (_carMaterial.GetTexture(_textureName) == null)
        {
            var texture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);

            _carMaterial.SetTexture(_textureName, texture);
        }

        Camera.clearFlags = CameraClearFlags.Depth;

        var renderTexture = _carMaterial.GetTexture(_textureName) as RenderTexture;
        Camera.targetTexture = renderTexture;
    }

    public void ShowCar(Entity car)
    {
        _car = car;
        gameObject.SetActive(true);
    }

    private Vector3 previousPosition;
    private Vector2 _input;

    public void InitSettings(float cameraDistance, float movementSpeed, Vector2 offset)
    {
        _distanceToTarget = cameraDistance;
        _rotationSpeed = movementSpeed;
        _offset = offset;
    }

    public void Input(Vector2 input)
    {
        _input = input;
    }

    private Vector2 _currentAngle = new Vector2(-15,45);
    private Vector2 _deltaInput;
    
    private void Update()
    {
        if (_car == Entity.Null)
        {
            return;
        }

        _deltaInput = Vector2.SmoothDamp(_deltaInput, _input, ref _rotationVelocity, _damping, Mathf.Infinity, Time.unscaledDeltaTime);
        
        _deltaInput *= _rotationSpeed;
        _deltaInput *= Time.unscaledDeltaTime;

        var carLocalToWorld = _entityManager.GetComponentData<LocalToWorld>(_car);

        var rotationAroundYAxis = -_deltaInput.x * 180; // camera moves horizontally
        var rotationAroundXAxis = _deltaInput.y * 180; // camera moves vertically

        _currentAngle += new Vector2(rotationAroundXAxis, rotationAroundYAxis);

        _currentAngle.x = _cameraVerticalAngle;

        Camera.transform.position = carLocalToWorld.Position + carLocalToWorld.Forward * _distanceToTarget;
        Camera.transform.LookAt(carLocalToWorld.Position, carLocalToWorld.Up);

        Camera.transform.RotateAround(carLocalToWorld.Position, carLocalToWorld.Right, _currentAngle.x);
        Camera.transform.RotateAround(carLocalToWorld.Position, carLocalToWorld.Up, _currentAngle.y);
        
        Camera.transform.position += transform.right * _offset.x;
        Camera.transform.position += transform.up * _offset.y;
        
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
