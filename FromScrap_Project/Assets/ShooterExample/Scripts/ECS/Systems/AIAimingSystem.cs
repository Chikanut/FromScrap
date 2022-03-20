using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class AIAimingSystem: SystemBase
{
    protected override void OnUpdate()
    {
        
        var query = GetEntityQuery(typeof(PlayerTag), typeof(Translation));
        var compareTranslations = query.ToComponentDataArray<Translation>(Allocator.TempJob);
        
        if(compareTranslations.Length == 0)
            return;
        
        var deltaTime = Time.DeltaTime;
        Dependency = Entities.WithAll<EnemyTag>().ForEach((ref RotateTowards rotateTowards, in Translation translation, in Rotation rotation) =>
        {
            var currentDist = float.MaxValue;
            rotateTowards.Target = math.forward(rotation.Value) + translation.Value;
            
            for (int i = 0; i < compareTranslations.Length; i++)
            {
                var dist = math.distance(translation.Value, compareTranslations[i].Value);

                if (dist < currentDist)
                    rotateTowards.Target = compareTranslations[i].Value;
            }
        }).WithDisposeOnCompletion(compareTranslations).Schedule(Dependency);
        //searching
    }
}