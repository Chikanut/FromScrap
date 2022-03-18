using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS_Test
{
    public partial class CircleColliderSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var query = GetEntityQuery(typeof(CircleColliderComponent), typeof(Translation), typeof(MovementComponent));

            var compareTranslations = query.ToComponentDataArray<Translation>(Allocator.TempJob);
            var compareCircless = query.ToComponentDataArray<CircleColliderComponent>(Allocator.TempJob);
            var compareMovement = query.ToComponentDataArray<CircleColliderComponent>(Allocator.TempJob);
            var deltaTime = Time.DeltaTime;

            var sphere1 = new Sphere();
            var sphere2 = new Sphere();
            var contact = new Contact();

            Dependency = Entities.ForEach((ref MovementComponent movement, in Translation translation,
                    in CircleColliderComponent collider) =>
                {
                    for (int i = 0; i < compareCircless.Length; i++)
                    {
                        var collider_out = compareCircless[i];
                        var translation_out = compareTranslations[i];
                        var movement_out = compareMovement[i];

                        sphere1.Position = translation.Value;
                        sphere1.Radius = collider.Radius;

                        sphere2.Position = translation_out.Value;
                        sphere2.Radius = collider_out.Radius;

                        if (math.distance(sphere1.Position, sphere2.Position) == 0)
                            continue;

                        if (!(math.distance(sphere1.Position, sphere2.Position) <= (sphere1.Radius + sphere2.Radius)))
                            continue;

                        var midline = (sphere1.Position - sphere2.Position);

                        contact.ContactNormal = (sphere1.Position - sphere2.Position) /
                                                math.length(sphere1.Position - sphere2.Position);
                        contact.ContactPoint = sphere2.Position + (midline / math.length(midline) * sphere2.Radius);
                        contact.DeepestPoint = (sphere1.Position - (midline / math.length(midline) * sphere1.Radius));
                        contact.PenetrationDepth = sphere1.Radius + sphere2.Radius - math.length(midline);

                        var closingVelocity = contact.ContactNormal * contact.PenetrationDepth;

                        closingVelocity.y = 0;

                        movement.Velocity += closingVelocity;
                    }
                })
                .WithDisposeOnCompletion(compareMovement)
                .WithDisposeOnCompletion(compareTranslations)
                .WithDisposeOnCompletion(compareCircless).Schedule(Dependency);
        }

        protected struct Sphere
        {
            public float3 Position;
            public float Radius;
        }

        protected struct Contact
        {
            public float3 ContactNormal;
            public float3 ContactPoint;
            public float3 DeepestPoint;
            public float PenetrationDepth;
        }
    }
}

// public class CircleColliderSystemJob : JobComponentSystem
