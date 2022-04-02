using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Cars.View.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class CarBodyViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ltw = GetComponentDataFromEntity<LocalToWorld>(true);
            var mgid = GetBufferFromEntity<MultyGroundInfoData>(true);

            var deltaTime = Time.fixedDeltaTime;

            Entities.WithAll<Parent>().ForEach(
                (ref Rotation rotation, ref CarBodyData bodyData, in LocalToWorld localToWorld) =>
                {
                    if (!ltw.HasComponent(bodyData.Parent) || !mgid.HasComponent(bodyData.Parent)) return;

                    var parentTransform = ltw[bodyData.Parent];
                    var parentGroundInfo = mgid[bodyData.Parent];

                    var groundNormal = GetPlaneNormal(parentGroundInfo, parentTransform);
                    ApplyMovementNoise(ref groundNormal, ref bodyData, localToWorld, deltaTime);
                    var groundForward = math.cross(parentTransform.Right, groundNormal);

                    //DebugTrajectories(localToWorld, parentTransform, groundNormal, groundForward);

                    var forward = parentTransform.Value.WorldToLocal(groundForward + parentTransform.Position);
                    var up = parentTransform.Value.WorldToLocal(groundNormal + parentTransform.Position);

                    bodyData.CurrentForward = ECS_Math_Extensions.SmoothDamp(bodyData.CurrentForward, forward,
                        ref bodyData.ForwardVelocity, bodyData.RotationDamping, float.MaxValue, deltaTime);

                    bodyData.CurrentUp = ECS_Math_Extensions.SmoothDamp(bodyData.CurrentUp, up,
                        ref bodyData.UpVelocity, bodyData.RotationDamping, float.MaxValue, deltaTime);

                    rotation.Value = quaternion.LookRotation(bodyData.CurrentForward, bodyData.CurrentUp);
                }).WithReadOnly(ltw).WithReadOnly(mgid).ScheduleParallel();
        }

        private static void ApplyMovementNoise(ref float3 up, ref CarBodyData bodyData, LocalToWorld transform, float deltaTime)
        {
            var dist = Vector3.Distance(transform.Position, bodyData.PrevPos);
            float3 moveDir = Vector3.Normalize(transform.Position - bodyData.PrevPos);
                
            var speed = dist / deltaTime;
            var acceleration = (speed - bodyData.PrevSpeed) / deltaTime;

            // Debug.Log("acceleration : " + acceleration + " deltaTime : " + deltaTime);

            bodyData.CurrentSuspension = ECS_Math_Extensions.SmoothDamp(bodyData.CurrentSuspension,
                moveDir * math.clamp(acceleration, -1, 1) * bodyData.SuspensionRange, ref bodyData.SuspensionVelocity,
                bodyData.SuspensionDamping, float.MaxValue, deltaTime);
            bodyData.CurrentSuspension.y = 0;
                
            if(float.IsNaN(bodyData.CurrentSuspension.x))
                bodyData.CurrentSuspension = float3.zero;
                
            up -= bodyData.CurrentSuspension;
                
            bodyData.PrevSpeed = speed;
            bodyData.PrevPos = transform.Position;
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

        private static void DebugTrajectories(LocalToWorld localToWorld, LocalToWorld parentTransform, float3 groundNormal, float3 groundForward)
        {
            var groundRight = math.cross(groundNormal, groundForward);
            
            Debug.DrawLine(localToWorld.Position,
                localToWorld.Position + groundNormal * 10, Color.red);
            Debug.DrawLine(localToWorld.Position,
                localToWorld.Position + groundForward * 10, Color.green);
            Debug.DrawLine(localToWorld.Position,
                localToWorld.Position + groundRight * 10, Color.blue);
                
            Debug.DrawLine(localToWorld.Position,
                localToWorld.Position + parentTransform.Up * 10, Color.red/2);
            Debug.DrawLine(localToWorld.Position,
                localToWorld.Position + parentTransform.Forward * 10, Color.green/2);
            Debug.DrawLine(localToWorld.Position,
                localToWorld.Position + parentTransform.Right * 10, Color.blue/2);
        }
    }
}