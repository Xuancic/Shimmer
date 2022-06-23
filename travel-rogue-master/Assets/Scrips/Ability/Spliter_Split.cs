using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ability
{
    [CreateAssetMenu(menuName = "Travel/Spliter/Split", fileName = "Spliter_Split")]
    public class Spliter_Split : BaseAbilityAsset
    {
        [Tooltip("分裂数量")] 
        public int m_splitCount;
        [Tooltip("分裂的预制体")]
        public GameObject m_splitPrefab;
        
        public override AbilityInstance Instantiate(Unit unit)
        {
            var instance = new Instance();
            instance.Init(this, unit);
            return instance;
        }
        
        public class Instance : AbilityInstance
        {
            public override EAbilityType AbilityType => EAbilityType.Spliter_Split;
            
            private Spliter_Split m_asset;
            private Transform m_root;
            private Unit m_unit;
            public override void Init(BaseAbilityAsset asset, Unit unit)
            {
                base.Init(asset, unit);
                m_asset = asset as Spliter_Split;
                
                m_unit = unit;
                m_root = unit.transform;
                
                BaseState.OnDeadEvent += OnDeadEvent;
            }
            public override void Shutdown()
            {
                base.Shutdown();
                
                BaseState.OnDeadEvent -= OnDeadEvent;
            }
            
            private void OnDeadEvent(Unit self)
            {
                if (self == m_unit)
                {
                    for (int i = 0; i < m_asset.m_splitCount; i++)
                    {
                        Vector3 offset = Random.insideUnitCircle;
                        GameObject.Instantiate(m_asset.m_splitPrefab, m_root.position + offset, m_root.rotation);
                    }
                }
            }
        }
    }
}