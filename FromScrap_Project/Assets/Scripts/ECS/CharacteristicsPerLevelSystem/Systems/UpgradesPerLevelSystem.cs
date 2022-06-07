using CharacteristicsPerLevelSystem.Components;
using UpgradesSystem.Components;
using LevelingSystem.Components;
using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using UpgradesSystem.Systems;

namespace CharacteristicsPerLevelSystem.Systems
{
    [UpdateBefore(typeof(ShowUpgradeWindowSystem))]
    public partial class UpgradesPerLevelSystem : SystemBase
    {
        // public List<UpgradesPerLevelComponent> _currentUpgrades = new List<UpgradesPerLevelComponent>();
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
            var upgradesPerLevelBufferFilter = GetBufferFromEntity<UpgradesPerLevelBuffer>();
            
            Entities.ForEach((Entity entity, in UpgradeComponent upgradeInfo,
                in UpgradesPerLevelComponent upgradesPerLevelComponent) =>
            {
                if (upgradesPerLevelBufferFilter.HasComponent(upgradeInfo.Target))
                {
                    upgradesPerLevelBufferFilter[upgradeInfo.Target].Add(new UpgradesPerLevelBuffer()
                    {
                        Characteristics = upgradesPerLevelComponent.Characteristics
                    });
                    
                    ecb.AddComponent<UpgradesPerLevelRecalculateTag>(upgradeInfo.Target);
                }

                ecb.DestroyEntity(entity);
            }).Schedule();
            
            
            Entities.WithAll<UpgradePerLevelListener>().ForEach((Entity entity, DynamicBuffer<NewLevelBuffer> newLevelBuffer) =>
            {
                if (newLevelBuffer.Length > 0)
                    ecb.AddComponent<UpgradesPerLevelRecalculateTag>(entity);
            }).WithoutBurst().Run();

            Entities.WithAll<UpgradesPerLevelRecalculateTag>().ForEach((Entity entity,
                DynamicBuffer<UpgradesPerLevelBuffer> upgradesPerLevelBuffer,
                    DynamicBuffer<CharacteristicModificationsBuffer> characteristicsBuffer,
                    in UpgradePerLevelListener upgradesPerLevelListener, in LevelComponent levelComponent) =>
                {
                    bool found = false;
            
                    for (int i = 0; i < characteristicsBuffer.Length; i++)
                    {
                        var characteristicModificationsBuffer = characteristicsBuffer[i];
                        if (characteristicModificationsBuffer.ModificatorHolder == upgradesPerLevelListener.Target)
                        {
                            found = true;
                            characteristicModificationsBuffer.Value = CalculateCharacteristics(upgradesPerLevelBuffer, levelComponent);
                            characteristicsBuffer[i] = characteristicModificationsBuffer;
                        }
                    }
            
                    if (!found)
                    {
                        characteristicsBuffer.Add(new CharacteristicModificationsBuffer()
                            {ModificatorHolder = upgradesPerLevelListener.Target, Multiply = true, Value = CalculateCharacteristics(upgradesPerLevelBuffer,levelComponent)});
                    }
            
                    ecb.AddComponent<NewCharacteristicsTag>(entity);
                }).Schedule();
        }

        public static Characteristics CalculateCharacteristics(
            DynamicBuffer<UpgradesPerLevelBuffer> upgradesPerLevelBuffer, LevelComponent levelComponent)
        {
            var characteristics = new Characteristics(1f);

            for (int i = 0; i < upgradesPerLevelBuffer.Length; i++)
            {
                for (int levelMultiplier = 0; levelMultiplier < levelComponent.Level + 1; levelMultiplier++)
                    characteristics.Add(upgradesPerLevelBuffer[i].Characteristics);
            }

            return characteristics;
        }
    }
}