using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class JointModuleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector3 Axis;
    public float Range;
    public float Speed;

    public Rigidbody Target;

    public bool CreateJoint;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (CreateJoint)
        {
            var joint = new PhysicsJoint()
            {
                BodyBFromJoint = new BodyFrame(new RigidTransform(Quaternion.Euler(Axis*180), transform.position - Target.position)),
                BodyAFromJoint = new BodyFrame(new RigidTransform(Quaternion.Euler(Axis*180), float3.zero)),
                JointType = JointType.Fixed
            };
            
            joint.SetConstraints(new FixedList128Bytes<Constraint>()
            {
                new Constraint(){
                    Min = 0,
                    Max = 0,
                    ConstrainedAxes = new bool3(true,true,true),
                    SpringDamping = 2530.126f,
                    SpringFrequency = 74341.31f,
                    Type = ConstraintType.Linear
                },
                new Constraint(){
                    Min = 0,
                    Max = 0,
                    ConstrainedAxes = new bool3(true,true,true),
                    SpringDamping = 2530.126f,
                    SpringFrequency = 74341.31f,
                    Type = ConstraintType.Angular
                }
            });

            var constrainedBody =
                new PhysicsConstrainedBodyPair(entity, conversionSystem.GetPrimaryEntity(Target.gameObject), true);
            
            var jointEntity = dstManager.CreateEntity();
            dstManager.SetName(jointEntity, "Test joint");

            dstManager.AddComponentData(jointEntity, joint);
            dstManager.AddComponentData(jointEntity, constrainedBody);
            dstManager.AddSharedComponentData(jointEntity, new PhysicsWorldIndex());

            dstManager.AddComponentData(jointEntity, new JointModuleComponent()
            {
                Axis = Axis,
                Range = Range,
                Speed = Speed,
                BaseAxis = joint.BodyBFromJoint.Axis
            });
        }
        else
        {
            var data = new JointModuleInitializeComponent
            {
                Axis = Axis,
                Range = Range,
                Speed = Speed,
                Target = entity
            };
            dstManager.AddComponentData(entity, data);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + Axis);
    }
}

public struct JointModuleInitializeComponent : IComponentData
{
    public Entity Target;
    public float3 Axis;
    public float Range;
    public float Speed;
}

public struct JointModuleComponent : IComponentData
{
    public float3 Axis;
    public float Range;
    public float Speed;
    public float Progress;
    public float3 BaseAxis;
}

public partial class InitJointModuleSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var jointModuleInitializators = GetComponentDataFromEntity<JointModuleInitializeComponent>();
        var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
        
        Dependency = Entities.WithNone<JointModuleComponent>().ForEach((Entity entity, in PhysicsConstrainedBodyPair pair, in PhysicsJoint joint) =>
        {
            if (jointModuleInitializators.HasComponent(pair.EntityA))
            {
                var initialization = jointModuleInitializators[pair.EntityA];
                
                ecb.AddComponent(entity, new JointModuleComponent()
                {
                    Axis = initialization.Axis,
                    Range = initialization.Range,
                    Speed = initialization.Speed,
                    BaseAxis = joint.BodyBFromJoint.Axis
                });
            }
        }).WithReadOnly(jointModuleInitializators).Schedule(Dependency);
        
        _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}

[UpdateAfter(typeof(InitJointModuleSystem))]
public partial class ClearJointInitComponentsSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
        
        Entities.WithAll<JointModuleInitializeComponent>().ForEach((Entity entity) =>
        {
              ecb.RemoveComponent<JointModuleInitializeComponent>(entity);
        }).Schedule(Dependency);
        
        _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}

public partial class JointModuleComponentsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities.ForEach((Entity entity, ref JointModuleComponent jointModule, ref PhysicsJoint joint) =>
        {
            jointModule.Progress += deltaTime * jointModule.Speed;
            
            if (jointModule.Progress >= 1 || jointModule.Progress <= -1)
            {
                jointModule.Speed *= -1;
                jointModule.Progress = Mathf.Clamp(jointModule.Progress, -1, 1);
            }
            
            var angle = jointModule.Progress * jointModule.Range;

            var jointBodyBFromJoint = joint.BodyBFromJoint;
            jointBodyBFromJoint.Axis = jointModule.BaseAxis + angle * jointModule.Axis;
            joint.BodyBFromJoint = jointBodyBFromJoint;


        }).ScheduleParallel();
    }
}


