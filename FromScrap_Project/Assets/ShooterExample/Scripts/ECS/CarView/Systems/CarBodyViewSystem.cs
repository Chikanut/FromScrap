using Cars.View.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Cars.View.Systems
{
    public partial class CarBodyViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            /*
            Entities.ForEach((ref Rotation rotation,in CarBodyData bodyData, in DynamicBuffer<MultyGroundInfoData> groundInfo) =>
            {
                
            }).ScheduleParallel();
            */
        }
    }
}