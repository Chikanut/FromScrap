using BovineLabs.Event.Systems;
using ECS.SignalSystems.Systems;
using Kits.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kits.Systems
{
    public partial class KitInstalationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        private EventSystem _eventSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _eventSystem = this.World.GetOrCreateSystem<EventSystem>();
            
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();

            var kpc = GetComponentDataFromEntity<KitPlatformComponent>(true);
            var kpcb = GetBufferFromEntity<KitPlatformConnectionBuffer>(true);
            var kpkb = GetBufferFromEntity<KitPlatformKitsBuffer>();

            var installEvent = _eventSystem.CreateEventWriter<KitInstalledSignal>();
            
            Entities.WithNone<Parent>().ForEach(
                (Entity entity, ref KitComponent kitComponent, in KitIDComponent kitID, in LocalToWorld localToWorld, in KitInstalatorComponent instalationData,
                    in KitInstalatorTargetComponent intalationTarget) =>
                {
                    if (!kpcb.HasComponent(intalationTarget.TargetEntity) ||
                        !kpc.HasComponent(intalationTarget.TargetEntity))
                    {
                        Debug.LogError(
                            "Cant connect kit to entity! Entity dont have kit connection buffer or kit platform component!");
                        ecb.DestroyEntity(entity);
                        return;
                    }


                    var connectionInfo = kpcb[intalationTarget.TargetEntity];

                    var canInstall = false;

                    for (var i = 0; i < connectionInfo.Length; i++)
                    {
                        if (connectionInfo[i].ConnectionType != kitComponent.Type) continue;
                        
                        canInstall = true;
                        break;
                    }

                    if (!canInstall)
                    {
                        Debug.LogError("Cant connect kit to entity! Platform does not have connection type!");
                        ecb.DestroyEntity(entity);
                        return;
                    }
                    
                    var platformInfo = kpc[intalationTarget.TargetEntity];

                    if (!kitComponent.IsStacked)
                    {
                        if (kpkb.HasComponent(intalationTarget.TargetEntity))
                        {
                            for (int i = 0; i < kpkb[intalationTarget.TargetEntity].Length; i++)
                            {
                                if (GetComponent<KitIDComponent>(kpkb[intalationTarget.TargetEntity][i].ConnectedKit)
                                        .ID != kitID.ID) continue;
                                
                                ecb.DestroyEntity(kpkb[intalationTarget.TargetEntity][i].ConnectedKit);
                                kpkb[intalationTarget.TargetEntity].RemoveAt(i);
                                platformInfo.IsFree = true;
                            }
                        }
                    }
                    
                    if (!platformInfo.IsFree)
                    {
                        Debug.LogError("Cant connect kit to entity! Platform is occupied!");
                        ecb.DestroyEntity(entity);
                        return;
                    }

                    if (kpkb.HasComponent(intalationTarget.TargetEntity))
                    {
                        kpkb[intalationTarget.TargetEntity].Add(new KitPlatformKitsBuffer() {ConnectedKit = entity});
                    }

                    kitComponent.Platform = intalationTarget.TargetEntity;
                    
                    ecb.AddComponent(entity, new Parent() {Value = intalationTarget.TargetEntity});
                    ecb.AddComponent(entity, new LocalToParent());
                    ecb.SetComponent(entity, new Translation() {Value = instalationData.Offset});
                    installEvent.Write(new KitInstalledSignal(){Car = platformInfo.Scheme});
                    
                    if (platformInfo.CanOccupy)
                    {
                        ecb.SetComponent(intalationTarget.TargetEntity, new KitPlatformComponent()
                        {
                            CanOccupy = true,
                            IsFree = false,
                            Scheme = platformInfo.Scheme
                        });
                    }

                    if (!instalationData.LocalUp)
                    {
                        var forward = localToWorld.Value.WorldToLocal(localToWorld.Position + math.forward());
                        var up = localToWorld.Value.WorldToLocal(localToWorld.Position + math.up());
                        ecb.SetComponent(entity, new Rotation() {Value = quaternion.LookRotation(forward, up)});
                    }
                    
                    ecb.RemoveComponent<KitInstalatorComponent>(entity);
                }).WithReadOnly(kpc).WithReadOnly(kpcb).Run();
            
            _eventSystem.AddJobHandleForProducer<KitInstalledSignal>(Dependency);
        }
    }
}
