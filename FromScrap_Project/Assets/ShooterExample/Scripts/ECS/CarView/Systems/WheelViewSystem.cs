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
                var targetPos = translation.Value;
                var moveDir = math.normalize(localToWorld.Position - wheelData.PrevPos);
                wheelData.PrevPos = localToWorld.Position;
                var localUp = wheelData.ParentUp;

                var spinAxis = new float3(1,0,0);
                if (wheelData.isGuide)
                {
                    wheelData._testFloat += deltaTime;
                    var input = math.cos(wheelData._testFloat + math.PI);
                    
                    wheelData.TurnDirection = math.lerp(math.forward(), math.right(), wheelData.TurnRange * input);
                    
                    rotation.Value =  quaternion.LookRotationSafe(wheelData.TurnDirection, math.up());
    
                    spinAxis = math.cross(wheelData.TurnDirection, math.up());
                }
                if (groundInfoData.isGrounded)
                {
                    var targetAngle = dist / wheelData.Radius;
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
                    targetPos -= localUp * (2 * deltaTime);
                
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
                
                var anchor = wheelData.LocalAnchor;

                if(math.distance(targetPos, anchor - wheelData.ParentUp * (wheelData.SuspensionOffset/2)) > wheelData.SuspensionDistance)
                {
                    var dir = math.normalize(math.project(math.normalize(targetPos - anchor), wheelData.ParentUp));
                    
                    targetPos = (anchor - wheelData.ParentUp * (wheelData.SuspensionOffset/2)) + dir * wheelData.SuspensionDistance;
                }
                
                translation.Value = targetPos;
            }).ScheduleParallel();
        }
    }
}