using System;
using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using UnityEngine;
using Zenject;

namespace ECS.SignalSystems.Systems
{
 

    public struct EnemyDamageSignal : ISignal
    {
        public int Damage;
    }
    
    public struct EnemyKillSignal : ISignal { }


    
    public class EnemyDamageSignalSystem : ConsumeSingleEventSystemBase<EnemyDamageSignal>
    {
        protected override void Create()
        {
            base.Create();
        
            ProjectContext.Instance.Container.Inject(this);
        }
        
        private ISignalService _signalService;
        
        [Inject]
        public void Init(ISignalService signalService)
        {
            _signalService = signalService;
        }
        
        protected override void OnEvent(EnemyDamageSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
    
    public class EnemyKillSignalSystem : ConsumeSingleEventSystemBase<EnemyKillSignal>
    {
        protected override void Create()
        {
            base.Create();
        
            ProjectContext.Instance.Container.Inject(this);
        }
        
        private ISignalService _signalService;
        
        [Inject]
        public void Init(ISignalService signalService)
        {
            _signalService = signalService;
        }
        
        protected override void OnEvent(EnemyKillSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
}