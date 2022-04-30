using System.Globalization;
using BovineLabs.Event.Systems;
using Unity.Mathematics;
using UnityEngine;

namespace DamageSystem.Systems
{
    public struct DamagePointsEvent
    {
        public float Damage;
        public float3 Position;
    }

    public partial class DamagePointsVisualSystem : ConsumeSingleEventSystemBase<DamagePointsEvent>
    {
        private (int maxDamage, string textPrefabName)[] _textsInfo = new[]
        {
            (25, "SmallDamageText"), (50, "DefaultDamageText"), (100, "MediumDamageText"), (250, "BigDamageText"),
            (500, "LargeDamageText"), (1000, "EpicDamageText"), (5000, "ColossalDamageText"), (10000, "BFDamageText")
        };
        
        protected override void OnEvent(DamagePointsEvent e)
        {
            var _defaultDamageText = _textsInfo[0];

            for (int i = 0; i < _textsInfo.Length; i++)
            {
                if (_textsInfo[i].maxDamage <= e.Damage)
                {
                    _defaultDamageText = _textsInfo[i];
                }
                else
                {
                    break;
                }
            }

            var text = ObjectsPool.Instance.GetObjectOfType<DamageTextPoolObject>(_defaultDamageText.textPrefabName);
            text.transform.position = e.Position;
            text.SetText(e.Damage.ToString(CultureInfo.InvariantCulture));
            
            Debug.LogError(_defaultDamageText.textPrefabName);
        }
    }
}

