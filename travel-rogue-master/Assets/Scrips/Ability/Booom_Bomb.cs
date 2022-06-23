using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Ability
{
    [CreateAssetMenu(menuName = "Travel/Booom/Bomb", fileName = "Booom_Bomb")]
    public class Booom_Bomb : BaseAbilityAsset
    {
        [Tooltip("伤害")]
        public float m_damage = 1.5f;
        [Tooltip("检测距离")]
        public float m_checkRadius = 0.5f;
        [Tooltip("伤害范围")]
        public float m_damageRadius = 0.5f;
        [Tooltip("爆炸延迟")]
        public float m_bombDelay = 1f;
        [Tooltip("爆炸特效")]
        public GameObject m_damageEffectPrefab;
        
        public override AbilityInstance Instantiate(Unit unit)
        {
            var instance = new Instance();
            instance.Init(this, unit);
            return instance;
        }
        
        public class Instance : AbilityInstance
        {
            public override EAbilityType AbilityType => EAbilityType.Booom_Bomb;
            
            private static readonly Collider2D[] m_hits = new Collider2D[10];

            private Booom_Bomb m_asset;
            private Unit m_unit;
            private Transform m_root;
            private EnemyController m_controller;
            public override void Init(BaseAbilityAsset asset, Unit unit)
            {
                base.Init(asset, unit);
                m_asset = asset as Booom_Bomb;
                
                m_controller = unit.GetComponent<EnemyController>();
                m_unit = unit;
                m_root = unit.transform;
                
                BaseState.OnBeHitEvent += OnBeHitEvent;
            }
            public override void Shutdown()
            {
                base.Shutdown();
                BaseState.OnBeHitEvent -= OnBeHitEvent;
            }
            protected override bool OnBegin()
            {
                if (!base.OnBegin()) return false;
                m_controller.Controlable = false;
                return true;
            }
            protected override void OnEnd()
            {
                base.OnEnd();
                m_controller.Controlable = true;
            }
            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (!m_isSpelling)
                {
                    var target = m_controller.Target;
                    if (target != null)
                    {
                        var disSq = (target.transform.position - m_root.position).sqrMagnitude;
                        if (disSq < m_asset.m_checkRadius * m_asset.m_checkRadius)
                        {
                            OnBegin();
                        }
                    }
                }
                else
                {
                    if (m_timer >= m_asset.m_bombDelay)
                    {
                        var origin = m_root.position;
                        var count = Physics2D.OverlapCircleNonAlloc(origin, m_asset.m_damageRadius, m_hits, Layers.UNIT_MASK);
                        for (int i = 0; i < count; i++)
                        {
                            var hit = m_hits[i];
                            if (hit.transform == m_root) continue;
                            var state = hit.GetComponent<BaseState>();
                            if (state.IsAlive)
                            {
                                if (hit.CompareTag(Tags.PLAYER))
                                {
                                    state.ApplyDamage(m_asset.m_damage, EDamageBy.Booom_Bomb, m_unit);
                                }
                                else if (hit.CompareTag(Tags.ENEMY))
                                {
                                    state.ApplyDamage(m_asset.m_damage * 0.5f, EDamageBy.Booom_Bomb, m_unit);
                                }
                            }
                        }
                        if (m_asset.m_damageEffectPrefab != null) Object.Instantiate(m_asset.m_damageEffectPrefab, origin, Quaternion.identity);
                        OnEnd();
                        m_state.ApplyDamage(9999f, EDamageBy.Booom_Bomb, m_unit);
                    }
                }
            }
            
            private void OnBeHitEvent(Unit self, float damage, EDamageBy damageBy, Unit srcUnit)
            {
                if (self == m_unit && m_unit != srcUnit && damageBy == EDamageBy.Booom_Bomb)
                {
                    OnBegin();
                }
            }
        }
    }
}