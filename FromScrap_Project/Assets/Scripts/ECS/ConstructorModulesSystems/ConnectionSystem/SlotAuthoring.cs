using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

public class SlotAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public PhysicsBodyAuthoring[] ConnectedBodyes;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        for (int i = 0; i < ConnectedBodyes.Length; i++)
        {
            var jointEntity = dstManager.CreateEntity();
            dstManager.SetName(jointEntity, gameObject.name+" to " +ConnectedBodyes[i].gameObject.name + " connection");
            dstManager.AddComponentData(jointEntity,
                new JointConnection()
                    {EntityA = entity, EntityB = conversionSystem.GetPrimaryEntity(ConnectedBodyes[i].gameObject)});
        }

    }
    
    private void OnDrawGizmosSelected()
    {
        if(ConnectedBodyes == null || ConnectedBodyes.Length <= 0) return;
        
        Gizmos.color = Color.red;

        for (int i = 0; i < ConnectedBodyes.Length; i++)
        {
            Gizmos.DrawLine(transform.position, ConnectedBodyes[i].transform.position);
        }
    }
}

public struct JointConnection : IComponentData
{
    public Entity EntityA;
    public Entity EntityB;
}

public partial class ConnectionSetupSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var localToWorldFilter = GetComponentDataFromEntity<LocalToWorld>(true);

        var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
        Dependency = Entities.WithNone<PhysicsJoint>().ForEach((Entity entity, ref JointConnection jointConnection) =>
        {
            if (jointConnection.EntityA == Entity.Null || jointConnection.EntityB == Entity.Null) return;
            
            if (!localToWorldFilter.HasComponent(jointConnection.EntityA) ||
                !localToWorldFilter.HasComponent(jointConnection.EntityB))
            {
                ecb.DestroyEntity(entity);
                return;
            }
            
            var worldFromA = Math.DecomposeRigidBodyTransform(localToWorldFilter[jointConnection.EntityA].Value);
            var worldFromB = Math.DecomposeRigidBodyTransform(localToWorldFilter[jointConnection.EntityB].Value);
            var bFromA = math.mul(math.inverse(worldFromB), worldFromA);
            var orientationLocal = quaternion.identity;
            var positionLocal = float3.zero;

            var positionInConnectedEntity = math.transform(bFromA, positionLocal);
            var orientationInConnectedEntity = math.mul(bFromA.rot, orientationLocal);

            var joint = PhysicsJoint.CreateFixed(new RigidTransform(orientationLocal, positionLocal),
                new RigidTransform(orientationInConnectedEntity, positionInConnectedEntity));
            var constrainedBody = new PhysicsConstrainedBodyPair(jointConnection.EntityA, jointConnection.EntityB, true);
            
            ecb.AddComponent(entity, joint);
            ecb.AddComponent(entity, constrainedBody);
            ecb.AddSharedComponent(entity, new PhysicsWorldIndex());

        }).WithReadOnly(localToWorldFilter).Schedule(Dependency);
        _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}

public partial class ConnectionCleanupSystem : SystemBase
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
        Dependency = Entities.ForEach((Entity entity,in JointConnection jointConnection) =>
        {
            if (jointConnection.EntityA != Entity.Null && jointConnection.EntityB != Entity.Null) return;
            
            ecb.DestroyEntity(entity);
        }).Schedule(Dependency);
        _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
