using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class JointModuleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector3 Axis;
    public float Range;
    public float Speed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
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


