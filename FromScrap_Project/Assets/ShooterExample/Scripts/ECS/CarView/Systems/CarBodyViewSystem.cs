using Cars.View.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Cars.View.Systems
{
    public partial class CarBodyViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref CarBodyData bodyData, in LocalToWorld localToWorld, in Parent parent) =>
            {
                var parentTransform = EntityManager.GetComponentData<LocalToWorld>(parent.Value);
                bodyData.ParentForward = parentTransform.Forward;
                bodyData.ParentRight = parentTransform.Right;
                bodyData.ParentUp = parentTransform.Up;
            }).WithoutBurst().Run();

            Entities.WithAll<Parent>().ForEach(
                (ref Rotation rotation, in Translation translation, in LocalToWorld localToWorld,
                    in CarBodyData bodyData, in DynamicBuffer<MultyGroundInfoData> groundInfo) =>
                {
                    //FROM NORMAL
                    
                    // var midNormal = float3.zero;
                    // for (int i = 0; i < groundInfo.Length; i++)
                    // {
                    //     if (groundInfo[i].isGrounded)
                    //     {
                    //         midNormal += groundInfo[i].GroundNormal;
                    //         Debug.DrawLine(groundInfo[i].GroundPosition,
                    //             groundInfo[i].GroundPosition + groundInfo[i].GroundNormal, Color.red);
                    //     }
                    //     else
                    //     {
                    //         midNormal += math.up();
                    //     }
                    // }
                    //
                    // midNormal /= groundInfo.Length;
                    //
                    // Debug.DrawLine(localToWorld.Position,
                    //     localToWorld.Position + midNormal * 10, Color.red);
                    // var forward = math.cross(bodyData.ParentRight,midNormal);
                    //
                    // rotation.Value = quaternion.LookRotationSafe(forward, midNormal);
                    
                    //FROM NORMAL

        
                     var midNormal = float3.zero;
                     for (var i = 0; i < groundInfo.Length; i++)
                     {
                         if (!groundInfo[i].isGrounded) continue;
                         
                         var point1 = groundInfo[i].GroundPosition;
                         var tempNormal = float3.zero;
                         
                         for (int j = 0; j < groundInfo.Length; j++)
                         {
                             if (i == j) continue;

                             float3 point2;
                             if (groundInfo[j].isGrounded)
                             {
                                 point2 = groundInfo[j].GroundPosition;
                             }
                             else
                             {
                                 point2 =  localToWorld.Value.LocalToWorld(groundInfo[i].AnchorPoints)- math.up() * groundInfo[i].CheckDistance;
                             }

                             var dir = math.normalize(point1 - point2);
                             
                             tempNormal += math.cross( math.cross(dir,bodyData.ParentUp),bodyData.ParentRight);
                         }

                         tempNormal = math.normalize(tempNormal);
                         midNormal += tempNormal;
                     }
                     
                     midNormal = math.normalize(midNormal);
                     
                     Debug.DrawLine(localToWorld.Position,
                         localToWorld.Position + midNormal * 10, Color.red);

                     // if (!float.IsNaN(midNormal.x))
                     // {
                     //     var forward = math.cross(localToWorld.Right, midNormal);
                     //
                     //     rotation.Value = quaternion.LookRotation(forward, midNormal);
                     // }


                     //FROM POSITION
                    
                    // var midPoint = float3.zero;
                    // for (int i = 0; i < groundInfo.Length; i++)
                    // {
                    //     var startPoint = localToWorld.Value.LocalToWorld(groundInfo[i].AnchorPoints);
                    //     
                    //     if (groundInfo[i].isGrounded)
                    //     {
                    //         midPoint += groundInfo[i].GroundPosition;
                    //     }
                    //     else
                    //     {
                    //         midPoint += startPoint - math.up() * groundInfo[i].CheckDistance;
                    //     }
                    // }
                    //
                    // midPoint /= groundInfo.Length;
                    //
                    // var midNormal = math.normalize(midPoint - localToWorld.Position);
                    //
                    // Debug.DrawLine(localToWorld.Position,
                    // localToWorld.Position + midNormal * 10, Color.red);
                    //
                    // var forward = math.cross(bodyData.ParentRight,midNormal);
                    //
                    // rotation.Value = quaternion.LookRotationSafe(forward, midNormal);
                }).ScheduleParallel();
        }
    }
}