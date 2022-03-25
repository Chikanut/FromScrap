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
            Entities.ForEach((ref WheelData wheelData, in Parent parent) =>
            {
                var parentTransform = EntityManager.GetComponentData<LocalToWorld>(parent.Value);
                wheelData.ParentUp = parentTransform.Up;
                wheelData.ParentRight = parentTransform.Right;
            }).WithoutBurst().Run();

            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((ref WheelData wheelData, ref Rotation rotation, ref Translation translation,
                in LocalToWorld localToWorld, in GroundInfoData groundInfoData) =>
            {
                var dist = math.distance(localToWorld.Position, wheelData.PrevPos);
                var targetPos = wheelData.PrevPos = localToWorld.Position;

                var localUp = localToWorld.Up;

                if (groundInfoData.isGrounded)
                {
                    var targetAngle = dist / wheelData.Radius;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                    
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
                    targetPos -= localToWorld.Up * (2 * deltaTime);
                
                    var targetAngle = 35 / wheelData.Radius;
                    
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }

                // var anchor = localToParent.Value.WorldToLocal(wheelData.Anchor);
                // if(math.distance(targetPos, anchor - localUp * (wheelData.SuspensionOffset/2)) > wheelData.SuspensionDistance)
                // {
                //     var dir = math.normalize(math.projectsafe(math.normalize(targetPos - anchor), localUp));
                //     
                //     targetPos = (anchor - localUp * (wheelData.SuspensionOffset/2)) + dir * wheelData.SuspensionDistance;
                // }

                // targetPos.y = math.clamp(targetPos.y, anchor+0.5f, a)
                // translation.Value = targetPos;
                translation.SetTranslationToWorldPosition(localToWorld, targetPos);

                // targetPos = translation.Value;
                
          
                

                // translation.Value = targetPos;

                // localToWorld.Value = targetPos;
                //Vector3.SmoothDamp(localToWorld.Position, targetPos, ref wheelData.Velocity, wheel.SuspensionDamping);

            }).ScheduleParallel();
        }
    }
}