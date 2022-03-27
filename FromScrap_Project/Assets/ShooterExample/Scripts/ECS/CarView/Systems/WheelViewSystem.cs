using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Cars.View.Systems
{
    public partial class WheelViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref WheelData wheelData, in LocalToWorld localToWorld, in Parent parent) =>
            {
                var parentTransform = EntityManager.GetComponentData<LocalToWorld>(parent.Value);
                wheelData.ParentUp = parentTransform.Up;
                wheelData.ParentForward = parentTransform.Forward;
                wheelData.ParentRight = parentTransform.Right;
            }).WithoutBurst().Run();

            var deltaTime = Time.DeltaTime;

            Entities.ForEach((ref WheelData wheelData, ref Rotation rotation, ref Translation translation,
                in LocalToWorld localToWorld, in GroundInfoData groundInfoData) =>
            {
                var dist = math.distance(localToWorld.Position, wheelData.PrevPos);
                var moveDir = math.normalize(localToWorld.Position - wheelData.PrevPos);
                
                UpdateSteering(ref wheelData, ref rotation, moveDir);
                UpdateRotation(ref wheelData, ref rotation, localToWorld, groundInfoData, dist, moveDir);
                UpdateSuspension(ref translation, wheelData, localToWorld, groundInfoData, deltaTime);
                
                wheelData.PrevPos = localToWorld.Position;
            }).ScheduleParallel();
        }

        private static void UpdateSteering(ref WheelData wheelData, ref Rotation rotation, float3 moveDir)
        {
            if (!wheelData.isGuide) return;

            moveDir.y = 0;
            moveDir = math.normalize(moveDir);
            
            var input = math.dot(wheelData.ParentRight, moveDir);
            
            if (!float.IsNaN(input))
            {
                wheelData.TurnDirection = math.lerp(math.forward(), math.right() * math.sign(input),
                    wheelData.TurnRange * math.abs(input));
            }

            rotation.Value = quaternion.LookRotationSafe(wheelData.TurnDirection, math.up() * (wheelData.isLeft ? -1 : 1));
        }

        private static void UpdateRotation(ref WheelData wheelData, ref Rotation rotation, LocalToWorld localToWorld,
            GroundInfoData groundInfoData, float moveDist, float3 moveDir)
        {
            if (groundInfoData.isGrounded)
            {
                var targetAngle = moveDist / wheelData.Radius;

                targetAngle *= math.sign(math.dot(localToWorld.Right, wheelData.ParentRight));
                targetAngle *= math.sign(math.dot(moveDir, wheelData.ParentForward));

                if (wheelData.isGuide)
                {
                    wheelData.CurrentAngle += targetAngle;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(wheelData.CurrentAngle));
                }
                else
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }
            }
            else
            {
                var targetAngle = 0.01f / wheelData.Radius;
                targetAngle *= math.sign(math.dot(localToWorld.Right, wheelData.ParentRight));

                if (wheelData.isGuide)
                {
                    wheelData.CurrentAngle += targetAngle;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(wheelData.CurrentAngle));
                }
                else
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }
            }
        }

        private static void UpdateSuspension(ref Translation translation, WheelData wheelData, LocalToWorld localToWorld,
            GroundInfoData groundInfoData, float deltaTime)
        {
            var targetPos = translation.Value;
            var localUp = wheelData.ParentUp;
            
            if (groundInfoData.isGrounded)
            {
                var alpha = groundInfoData.GroundNormal.Angle(localUp);
                var distance = math.distance(localToWorld.Position, groundInfoData.GroundPosition);
                if (distance > 0)
                {
                    var cTarget = wheelData.Radius;
                    var cComp = cTarget / distance;
                    var a = distance * math.sin(alpha);
                    var b = math.sqrt(math.pow(distance, 2) - math.pow(a, 2));
                    var bTarget = b * cComp;
                    bTarget -= b;
                    targetPos += localUp * bTarget;
                }
            }
            else
            {
                targetPos -= localUp * (9.8f * deltaTime);
            }

            var anchor = wheelData.LocalAnchor;

            if (math.distance(targetPos, anchor - wheelData.ParentUp * (wheelData.SuspensionOffset / 2)) >
                wheelData.SuspensionDistance)
            {
                var dir = math.normalize(math.project(math.normalize(targetPos - anchor), wheelData.ParentUp));

                targetPos = (anchor - wheelData.ParentUp * (wheelData.SuspensionOffset / 2)) +
                            dir * wheelData.SuspensionDistance;
            }

            translation.Value = targetPos;
        }
    }
}