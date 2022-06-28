using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class RotationModuleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Speed;
    public Vector3 Axis;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RotationModuleComponent
        {
            Speed = Speed,
            Axis = Axis
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Axis.normalized));
    }
}

public struct RotationModuleComponent : IComponentData
{
    public float Speed;
    public float3 Axis;
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(BuildPhysicsWorld)),
 UpdateBefore(typeof(StepPhysicsWorld))]
public partial class RotationModuleSystem : SystemBase
{
    BuildPhysicsWorld _createPhysicsWorldSystem;

    protected override void OnCreate()
    {
        _createPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        this.RegisterPhysicsRuntimeSystemReadWrite();
    }
    
    protected override void OnUpdate()
    {
         var deltaTime = Time.DeltaTime;
         var world = _createPhysicsWorldSystem.PhysicsWorld;
         Entities.ForEach((Entity entity, ref RotationModuleComponent rotationModule, in LocalToWorld localToWorld) =>
             {
                 var rgID = world.GetRigidBodyIndex(entity);
                 if(rgID == -1)
                     return;
                 
                 var velocity = localToWorld.WorldToLocalDirection(rotationModule.Axis) * rotationModule.Speed * deltaTime;
                 world.SetAngularVelocity(rgID,  velocity);
             })
            .Schedule();
    }
}
