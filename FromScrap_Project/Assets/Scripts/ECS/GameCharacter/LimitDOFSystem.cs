using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using System;
    public partial class LimitDOFSystem : SystemBase
    {

        public static PhysicsJoint CreateLimitDOFJoint(bool3 linearLocks, bool3 angularLocks)
        {
            var constraints = new FixedList128Bytes<Constraint>();
            if (math.any(linearLocks))
            {
                constraints.Add(new Constraint
                {
                    ConstrainedAxes = linearLocks,
                    Type = ConstraintType.Linear,
                    Min = 0,
                    Max = 0,
                    SpringFrequency = Constraint.DefaultSpringFrequency,
                    SpringDamping = Constraint.DefaultSpringDamping
                });
            }
            if (math.any(angularLocks))
            {
                constraints.Add(new Constraint
                {
                    ConstrainedAxes = angularLocks,
                    Type = ConstraintType.Angular,
                    Min = 0,
                    Max = 0,
                    SpringFrequency = Constraint.DefaultSpringFrequency,
                    SpringDamping = Constraint.DefaultSpringDamping
                });
            }

            var joint = new PhysicsJoint
            {
                BodyAFromJoint = BodyFrame.Identity,
            };
            joint.SetConstraints(constraints);
            return joint;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var entityManager = World.EntityManager;
            Entities.WithStructuralChanges().ForEach((Entity e, in LimitDOFJoint limit) =>
            {
                var archetype = World.EntityManager.CreateArchetype(typeof(PhysicsJoint));
                Entity entity = World.EntityManager.CreateEntity(archetype);

                PhysicsJoint joint = CreateLimitDOFJoint(limit.LockLinearAxes, limit.LockAngularAxes);

                var en = CreateJoint(joint, entity, e, true);
                
                entityManager.AddComponentData(e, joint);
                
            }).Run();
        }

        protected override void OnUpdate()
        {
        }
        
        public Entity CreateJoint(PhysicsJoint joint, Entity entityA, Entity entityB, bool enableCollision = false)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            ComponentType[] componentTypes =
            {
                typeof(PhysicsConstrainedBodyPair),
                typeof(PhysicsJoint)
            };
            Entity jointEntity = entityManager.CreateEntity(componentTypes);
 
            entityManager.SetComponentData(jointEntity, new PhysicsConstrainedBodyPair(entityA, entityB, enableCollision));
            entityManager.SetComponentData(jointEntity, joint);
 
            return jointEntity;
        }
    }
