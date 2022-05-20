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
    [SerializeField] private float _distanceToTarget = 10;
    [SerializeField] private  Material _carMaterial;
    [SerializeField] private  string _textureName;
    [SerializeField] private float _rotationSpeed = 15;
    
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
    
    public void Input(Vector2 input)
    {
        _input = input;
    }

    private Vector2 _currentAngle;

    private void Update()
    {
        if (_car == Entity.Null)
        {
            return;
        }

        _input = Vector2.right * _rotationSpeed;

        _input *= Time.unscaledDeltaTime;

        var carLocalToWorld = _entityManager.GetComponentData<LocalToWorld>(_car);

        var rotationAroundYAxis = -_input.x * 180; // camera moves horizontally
        var rotationAroundXAxis = _input.y * 180; // camera moves vertically

        _currentAngle += new Vector2(rotationAroundXAxis, rotationAroundYAxis);

        _currentAngle.x = -15;

        Camera.transform.position = carLocalToWorld.Position + carLocalToWorld.Forward * _distanceToTarget;
        Camera.transform.LookAt(carLocalToWorld.Position, carLocalToWorld.Up);

        Camera.transform.RotateAround(carLocalToWorld.Position, carLocalToWorld.Right, _currentAngle.x);
        Camera.transform.RotateAround(carLocalToWorld.Position, carLocalToWorld.Up, _currentAngle.y);
        
        _input = Vector2.zero;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
