using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace Ability
{
    [CreateAssetMenu(menuName = "Travel/BossSlime/Duang", fileName = "BossSlime_Duang")]
    public class BossSlime_Duang : BaseAbilityAsset
    {
        [Tooltip("上升延迟")]
        public float m_upDelay = 0.3f;
        [Tooltip("上升曲线")]
        public AnimationCurve m_upCurve;
        [Tooltip("滞空延迟")]
        public float m_airDelay = 1.5f;
        [Tooltip("下降延迟")]
        public float m_downDelay = 0.3f;
        [Tooltip("下降曲线")]
        public AnimationCurve m_downCurve;
        [Tooltip("伤害")] 
        public float m_damage = 1.5f;
        [Tooltip("伤害范围")] 
        public float m_damageRadius = 5f;
        [Tooltip("冷却")] 
        public float m_cooldown;
        
        public override AbilityInstance Instantiate(Unit unit)
        {
            var instance = new Instance();
            instance.Init(this, unit);
            return instance;
        }
        
        public class Instance : AbilityInstance
        {
            public override EAbilityType AbilityType => EAbilityType.Captain_Rash;

            private static readonly Collider2D[] m_hits = new Collider2D[10];

            private bool m_collide;
            private bool m_setPosition;
            private Vector3 m_targetPosition;
            private float m_cooldownTimer;
            
            private BossSlime_Duang m_asset;
            private Unit m_unit;
            private Transform m_root;
            private Transform m_modelRoot;
            private Collider2D m_collider;
            private EnemyController m_controller;
            public override void Init(BaseAbilityAsset asset, Unit unit)
            {
                base.Init(asset, unit);
                m_asset = asset as BossSlime_Duang;
                
                m_unit = unit;
                m_root = unit.transform;
                m_modelRoot = m_root.Find("Model");
                m_collider = m_root.GetComponent<Collider2D>();
                m_controller = m_root.GetComponent<EnemyController>();
            }
            
            protected override bool OnBegin()
            {
                if (!base.OnBegin()) return false;
                m_collide = false;
                m_setPosition = false;
                m_controller.Controlable = false;
                m_targetPosition = m_controller.Target.transform.position;
                m_cooldownTimer = m_asset.m_cooldown;
                return true;
            }
            protected override void OnEnd()
            {
                base.OnEnd();
                m_collider.enabled = true;
                m_controller.Controlable = true;
            }
            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                m_cooldownTimer -= deltaTime;
                var target = m_controller.Target;
                if (!m_isSpelling)
                {
                    if (m_cooldownTimer <= 0 && target != null)
                    {
                        OnBegin();
                        return;
                    }
                }
                else
                {
                    if (m_timer < m_asset.m_upDelay)
                    {
                        var t = m_timer;
                        var p = t / m_asset.m_upDelay;
                        if (p > 0.1f) m_collider.enabled = false;
                        m_modelRoot.position = m_root.position + new Vector3(0, m_asset.m_upCurve.Evaluate(p));
                    }
                    else if (m_timer > m_asset.m_upDelay + m_asset.m_airDelay)
                    {
                        var t = m_timer - m_asset.m_upDelay - m_asset.m_airDelay;
                        var p = t / m_asset.m_downDelay;
                        if (p < 1f)
                        {
                            if (!m_setPosition)
                            {
                                m_setPosition = true;
                                if (target != null)
                                {
                                    m_root.position = target.transform.position;
                                }
                                else
                                {
                                    m_root.position = m_targetPosition;
                                }
                            }
                            
                            if (p > 0.9f && !m_collide)
                            {
                                var count = Physics2D.OverlapCircleNonAlloc(m_root.position, m_asset.m_damageRadius, m_hits, Layers.UNIT_MASK);
                                for (int i = 0; i < count; i++)
                                {
                                    var hit = m_hits[i];
                                    if (hit.transform == m_root) continue;
                                    var state = hit.GetComponent<BaseState>();
                                    if (state.IsAlive)
                                    {
                                        if (hit.CompareTag(Tags.PLAYER))
                                        {
                                            state.ApplyDamage(m_asset.m_damage, EDamageBy.BossSlime_Duang, m_unit);
                                        }
                                    }
                                }
                                m_collider.enabled = true;
                                m_collide = false;
                            }

                            m_modelRoot.position = m_root.position + new Vector3(0, m_asset.m_downCurve.Evaluate(p));
                        }
                        else
                        {
                            m_modelRoot.localPosition = Vector3.zero;
                            OnEnd();
                        }
                    }
                    else
                    {
                        m_modelRoot.position = m_root.position + new Vector3(0, m_asset.m_upCurve.Evaluate(1f));
                    }
                }
            }
        }
    }
}