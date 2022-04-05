using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CarView : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public Transform Transform;
        public float Radius;
        public float SuspensionDistance;
        public float SuspensionOffset;
        public float SuspensionDamping;

        [HideInInspector]
        public Vector3 Anchor;
        [HideInInspector]
        public Vector3 PrevPos;
        [HideInInspector]
        public Vector3 Velocity;
        
        [FormerlySerializedAs("isForward")] public bool isGuide;
        [Range(0, 1)]
        public float TurnRange;
        [HideInInspector]
        public Vector3 TurnDirection;
        public float TurnDamping;
        [HideInInspector]
        public Vector3 TurnVelocity;
    }

    [Header("Components")]
    // [SerializeField] private Rigidbody _realBody;
    // [SerializeField] private SphereCollider _collider;
    [SerializeField] List<Wheel> _wheels = new List<Wheel>();

    [Header("Body Settings")]
    [SerializeField] private float _movementDamping;
    [SerializeField] private float _rotationDamping;

    [SerializeField] private bool _updateWheels;
    [SerializeField] private bool _updateBody;

    [Header("Wheels Settings")]
    [SerializeField] private float _wheelsGravity = 3;
    
    // (bool isGround, Ray ray, RaycastHit hit) isOnGround()
    // {
    //     var ray = new Ray(transform.position, Vector3.down);
    //     
    //     if (Physics.Raycast(ray, out var groundHit))
    //         ray = new Ray(transform.position, -groundHit.normal);
    //
    //     return (Physics.Raycast(ray, out var hit,_collider.radius + 0.1f), ray, hit);
    // }
    
    // private Vector3 _up;
    // private Vector3 _upVel;
    // private Vector3 _moveVel;

    void Start()
    {
        for (int i = 0; i < _wheels.Count; i++)
        {
            _wheels[i].Anchor = _wheels[i].Transform.parent.InverseTransformPoint(_wheels[i].Transform.position);
            _wheels[i].PrevPos = _wheels[i].Transform.position;
        }
    }

    void Update()
    {
       var input = GetInput();

       // if(_updateBody)
       //  UpdateView();

       if (_updateWheels)
       {
           for (int i = 0; i < _wheels.Count; i++)
           {
               UpdateWheel(_wheels[i], input);
           }
       }
    }
    
    // void UpdateView()
    // {
    //     var groundInfo = isOnGround();
    //     transform.position = Vector3.SmoothDamp(transform.position, _realBody.position, ref _moveVel, _movementDamping);
    //     if (groundInfo.isGround)
    //     {
    //         _up = Vector3.SmoothDamp(_up,groundInfo.hit.normal,ref _upVel,_rotationDamping);
    //         
    //         transform.LookAt(_realBody.velocity.normalized+_realBody.position, _up);
    //     }
    //     else
    //     {
    //         _up = Vector3.SmoothDamp(_up,Vector3.up,ref _upVel,_rotationDamping);
    //         transform.LookAt(_realBody.velocity.normalized+_realBody.position, _up);
    //     }
    // }

    void UpdateWheel(Wheel wheel, float input)
    {
        var dist = Vector3.Distance(wheel.Transform.position, wheel.PrevPos);
        var targetPos = wheel.PrevPos = wheel.Transform.position;
        var anchor = wheel.Transform.parent.TransformPoint(wheel.Anchor);
        var localUp = wheel.Transform.parent.up;
        
        var ray = new Ray(anchor, -localUp);
        
        if (Physics.Raycast(ray ,out var hit, wheel.Radius + Vector3.Distance(anchor, wheel.Transform.position)))
        {
            //Very complicated calculation of hit triangle form
            var alpha = Vector3.Angle(hit.normal, localUp);
            var distance = Vector3.Distance(wheel.Transform.position, hit.point);
            var cTarget = wheel.Radius;
            var c = distance;
            var cComp = cTarget / c;
            var a = c * Mathf.Sin(alpha);
            var b = Mathf.Sqrt(Mathf.Pow(c, 2) - Mathf.Pow(a, 2));
            
            //And we get the distance of wheel suspension movement
            var bTarget = b * cComp;
            bTarget -= b;
            
            targetPos += localUp * bTarget;

            var targetAngle = dist / wheel.Radius;
            targetAngle *= Mathf.Rad2Deg;

            wheel.Transform.RotateAround(wheel.Transform.position,wheel.Transform.right, targetAngle);
        }
        else
        {
            targetPos += Vector3.Project(Vector3.up, localUp) * (-_wheelsGravity * Time.deltaTime);
            
            var targetAngle = 35 / wheel.Radius;
            targetAngle *= Mathf.Rad2Deg;
            wheel.Transform.RotateAround(wheel.Transform.position,wheel.Transform.right, targetAngle);
        }


        if(Vector3.Distance(targetPos, anchor - localUp * (wheel.SuspensionOffset/2)) > wheel.SuspensionDistance)
        {
            var dir = Vector3.Project((targetPos-anchor).normalized,  localUp).normalized;

            targetPos = (anchor - localUp * (wheel.SuspensionOffset/2)) + dir * wheel.SuspensionDistance;
        }

        wheel.Transform.position = Vector3.SmoothDamp(wheel.Transform.position, targetPos, ref wheel.Velocity,
            wheel.SuspensionDamping);

        if (!wheel.isGuide) return;
        
        wheel.TurnDirection = Vector3.SmoothDamp(wheel.TurnDirection,
            wheel.Transform.parent.forward + (wheel.Transform.parent.right * wheel.TurnRange) * input,
            ref wheel.TurnVelocity, wheel.TurnDamping).normalized;

        var angle = Vector3.Angle(wheel.TurnDirection, Vector3.Cross(wheel.Transform.right, localUp));

        var cross = Vector3.Cross(wheel.TurnDirection, Vector3.Cross(localUp,wheel.Transform.right));
        if (cross.y < 0) angle = -angle;

        wheel.Transform.RotateAround(wheel.Transform.position,wheel.Transform.parent.up, angle);
    }
    
    float GetInput()
    {
        return 0;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var wheel in _wheels)
        {
            if(wheel.Transform == null) continue;
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(wheel.Transform.position, wheel.Transform.right, wheel.Radius);
            var startPoint = wheel.Transform.position + wheel.Transform.parent.up * wheel.SuspensionOffset;
            Gizmos.DrawLine(startPoint,startPoint-Vector3.down*wheel.SuspensionDistance);
        }
    }
    #endif
}