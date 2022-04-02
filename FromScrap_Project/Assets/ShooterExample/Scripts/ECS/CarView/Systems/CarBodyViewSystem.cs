using System;
using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Cars.View.Systems
{
    // [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(EndFramePhysicsSystem))]
    public partial class CarBodyViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ltw = GetComponentDataFromEntity<LocalToWorld>(true);
            var mgid = GetBufferFromEntity<MultyGroundInfoData>(true);

            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((ref Rotation rotation, ref CarBodyData bodyData, in LocalToWorld localToWorld) =>
            {
                if (!ltw.HasComponent(bodyData.Parent) || !mgid.HasComponent(bodyData.Parent)) return;
                
                var parentTransform = ltw[bodyData.Parent];
                var parentGroundInfo = mgid[bodyData.Parent];

                var groundNormal = GetPlaneNormal(parentGroundInfo, parentTransform);
                var groundForward = math.cross(parentTransform.Right,groundNormal);
                var groundRight = math.cross(groundNormal, groundForward);

                // Debug.DrawLine(localToWorld.Position,
                //     localToWorld.Position + groundNormal * 10, Color.red);
                // Debug.DrawLine(localToWorld.Position,
                //     localToWorld.Position + groundForward * 10, Color.green);
                // Debug.DrawLine(localToWorld.Position,
                //     localToWorld.Position + groundRight * 10, Color.blue);
                //     
                // Debug.DrawLine(localToWorld.Position,
                //     localToWorld.Position + parentTransform.Up * 10, Color.red/2);
                // Debug.DrawLine(localToWorld.Position,
                //     localToWorld.Position + parentTransform.Forward * 10, Color.green/2);
                // Debug.DrawLine(localToWorld.Position,
                //     localToWorld.Position + parentTransform.Right * 10, Color.blue/2);
                
                var forward = parentTransform.Value.WorldToLocal(groundForward + parentTransform.Position);
                var up = parentTransform.Value.WorldToLocal(groundNormal + parentTransform.Position);

                bodyData.CurrentForward = ECS_Math_Extensions.SmoothDamp(bodyData.CurrentForward, forward,
                    ref bodyData.ForwardVelocity, bodyData.RotationDamping, float.MaxValue, deltaTime);

                bodyData.CurrentUp = ECS_Math_Extensions.SmoothDamp(bodyData.CurrentUp, up,
                    ref bodyData.UpVelocity, bodyData.RotationDamping, float.MaxValue, deltaTime);

                rotation.Value = quaternion.LookRotation(bodyData.CurrentForward, bodyData.CurrentUp);
                

            }).WithReadOnly(ltw).WithReadOnly(mgid).ScheduleParallel();
        }
        
        private static float3 GetPlaneNormal(DynamicBuffer<MultyGroundInfoData> groundInfo, LocalToWorld localToWorld)
        {
            var LFpos = GetGroundPos(groundInfo[0], localToWorld);
            var RFpos = GetGroundPos(groundInfo[1], localToWorld);
            var RRpos = GetGroundPos(groundInfo[2], localToWorld);
            var LRpos = GetGroundPos(groundInfo[3], localToWorld);

            var a = RRpos - LRpos;
            var b = RFpos - RRpos;
            var c = LFpos - RFpos;
            var d = RRpos - LFpos;

            var crossBA = math.cross(b, a);
            var crossCB = math.cross(c, b);
            var crossDC = math.cross(d, c);
            var crossAD = math.cross(a, d);


            return -math.normalize(crossBA + crossCB + crossDC + crossAD);
        }

        private static float3 GetGroundPos(MultyGroundInfoData groundInfo, LocalToWorld localToWorld)
        {
            return groundInfo.isGrounded
                ? groundInfo.GroundPosition
                : localToWorld.Value.LocalToWorld(groundInfo.AnchorPoints) +
                  math.down() * groundInfo.CheckDistance;
        }
    }
}