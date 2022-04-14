using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Cars.View.Systems
{
    public partial class WheelViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ltw = GetComponentDataFromEntity<LocalToWorld>(true);
            var deltaTime = Time.DeltaTime;

            Entities.WithAll<Parent>().ForEach((ref WheelData wheelData, ref Rotation rotation,
                ref Translation translation,
                in LocalToWorld localToWorld, in GroundInfoData groundInfoData) =>
            {
                if (!ltw.HasComponent(wheelData.Parent)) return;

                var parentTransform = ltw[wheelData.Parent];

                var dist = math.distance(localToWorld.Position, wheelData.PrevPos);
                var moveDir = localToWorld.Position - wheelData.PrevPos;
                wheelData.PrevPos = localToWorld.Position;

                if (wheelData.isGuide)
                    UpdateSteering(ref wheelData, ref rotation, moveDir, parentTransform, deltaTime);
                UpdateRotation(ref wheelData, ref rotation, localToWorld, parentTransform, groundInfoData, dist,
                    moveDir);
                UpdateSuspension(ref translation, wheelData, localToWorld, parentTransform, groundInfoData, deltaTime);
            }).WithReadOnly(ltw).ScheduleParallel();
        }

        private static void UpdateSteering(ref WheelData wheelData, ref Rotation rotation, float3 moveDir,
            LocalToWorld parentTransform, float deltaTime)
        {
            moveDir.y = 0;

            var power = moveDir.Magnitude();

            if (power > 0.05f)
            {
                moveDir = math.normalize(moveDir);

                var input = math.dot(parentTransform.Right, moveDir);

                if (!float.IsNaN(input))
                {
                    var targetDir = math.lerp(math.forward(), math.right() * math.sign(input),
                        wheelData.TurnRange * math.abs(input));

                    wheelData.TurnDirection = ECS_Math_Extensions.SmoothDamp(wheelData.TurnDirection, targetDir,
                        ref wheelData.TurnVelocity, wheelData.TurnDamping, float.MaxValue, deltaTime);
                }
            }

            rotation.Value =
                quaternion.LookRotationSafe(wheelData.TurnDirection, math.up() * (wheelData.isLeft ? -1 : 1));
        }

        private static void UpdateRotation(ref WheelData wheelData, ref Rotation rotation, LocalToWorld localToWorld,
            LocalToWorld parentTransform,
            GroundInfoData groundInfoData, float moveDist, float3 moveDir)
        {
            if (groundInfoData.isGrounded)
            {
                var targetAngle = moveDist / wheelData.Radius;

                targetAngle *= math.sign(math.dot(localToWorld.Right, parentTransform.Right));
                targetAngle *= math.sign(math.dot(moveDir, parentTransform.Forward));

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
                targetAngle *= math.sign(math.dot(localToWorld.Right, parentTransform.Right));

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

        private static void UpdateSuspension(ref Translation translation, WheelData wheelData,
            LocalToWorld localToWorld, LocalToWorld parentTransform,
            GroundInfoData groundInfoData, float deltaTime)
        {
            var targetPos = wheelData.LocalAnchor;
            targetPos.y = translation.Value.y;
            var localUp = parentTransform.Value.WorldToLocal(parentTransform.Up + parentTransform.Position);

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

            if (math.distance(targetPos, anchor - localUp * (wheelData.SuspensionOffset / 2)) >
                wheelData.SuspensionDistance)
            {
                var dir = math.normalize(math.project(math.normalize(targetPos - anchor), localUp));

                targetPos = (anchor - localUp * (wheelData.SuspensionOffset / 2)) +
                            dir * wheelData.SuspensionDistance;
            }

            translation.Value = ECS_Math_Extensions.SmoothDamp(translation.Value, targetPos,
                ref wheelData.SuspensionVelocity,
                wheelData.SuspensionDamping, float.MaxValue, deltaTime);
        }
    }
}