using System.Collections.Generic;
using UnityEngine;

namespace Ability
{
    [CreateAssetMenu(menuName = "Travel/Player/Link", fileName = "Link")]
    public class Link : BaseAbilityAsset
    {
        [Tooltip("蓄力时间")]
        public float chargeTime;
        [Tooltip("扫描半径")]
        public float scanRadius;
        [Tooltip("扫描角度")]
        public float scanAngle;
        
        [Tooltip("伤害参数1")]
        public float damageParam1 = 0.9f;
        [Tooltip("伤害参数2")]
        public float damageParam2 = 2.5f;
        
        [Tooltip("Buff类型")]
        public EBuffType buffType;
        
        [Tooltip("减速百分比")]
        public float slowPercent;
        
        public override AbilityInstance Instantiate(Unit unit)
        {
            var instance = new Instance();
            instance.Init(this, unit);
            return instance;
        }
        
        public class Instance : AbilityInstance
        {
            public override EAbilityType AbilityType => EAbilityType.Link;

            private float m_pressTimer;

            private static readonly Collider2D[] m_hits = new Collider2D[10];
            
            private HashSet<BaseState> m_validTargets = new HashSet<BaseState>();

            private Link m_asset;
            private Unit m_unit;
            private Transform m_root;
            private PlayerAttacker m_attacker;
            private PlayerController m_controller;
            public override void Init(BaseAbilityAsset asset, Unit unit)
            {
                base.Init(asset, unit);
                m_asset = asset as Link;
                
                m_unit = unit;
                m_root = unit.transform;
                m_attacker = unit.GetComponent<PlayerAttacker>();
                m_controller = unit.GetComponent<PlayerController>();
            }

            protected override bool OnBegin()
            {
                if (!base.OnBegin()) return false;
                m_validTargets.Clear();
                m_unit.m_propAddList.Set(EPropAddSrcType.Player_Link, new PropAdd
                {
                    type = EPropType.MoveSpeedPercent,
                    value = -m_asset.slowPercent,
                });
                m_controller.Dashable = false;
                return true;
            }
            protected override void OnEnd()
            {
                base.OnEnd();
                m_unit.m_propAddList.Remove(EPropAddSrcType.Player_Link);
                m_controller.Dashable = true;
            }
            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (!m_isSpelling)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_pressTimer = 0f;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        m_pressTimer += deltaTime;
                        if (m_pressTimer >= 0.2f)
                        {
                            OnBegin();
                        }
                    }
                }
                else
                {
                    m_pressTimer += deltaTime;
                    //释放
                    if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
                    {
                        //判断蓄力
                        if (m_pressTimer >= m_asset.chargeTime)
                        {
                            var validList = new List<BaseState>();
                            foreach (var state in m_validTargets)
                            {
                                if (state.IsAlive)
                                {
                                    var buffControl = state.GetComponent<BaseBuffControl>();
                                    if (buffControl.HasBuff(m_asset.buffType))
                                    {
                                        validList.Add(state);
                                    }
                                }
                            }
                            var validCount = validList.Count;
                            for (var i = validList.Count - 1; i >= 0; i--)
                            {
                                var state = validList[i];
                                //造成伤害
                                var damage = Mathf.Pow(m_asset.damageParam1 + m_attacker.m_atkDamage / m_asset.damageParam2, validCount);
                                state.ApplyDamage(damage, EDamageBy.PLayer_Link, m_unit);
                                //清空buff
                                state.GetComponent<BaseBuffControl>().RemoveBuff(m_asset.buffType);
                            }
                            m_validTargets.Clear();
                        }
                        OnEnd();
                        return;
                    }
                    
                    var up = m_root.up;
                    var origin = m_root.position;
                    var count = Physics2D.OverlapCircleNonAlloc(origin, m_asset.scanRadius, m_hits, Layers.UNIT_MASK);
                    for (int i = 0; i < count; i++)
                    {
                        var hit = m_hits[i];
                        if (hit.CompareTag(Tags.ENEMY))
                        {
                            var state = hit.GetComponent<BaseState>();
                            if (state.IsAlive)
                            {
                                var to = (hit.transform.position - origin).normalized;
                                if (Vector2.Angle(up, new Vector2(to.x, to.y)) < m_asset.scanAngle / 2f)
                                {
                                    hit.GetComponent<BaseBuffControl>().AddBuff(m_asset.buffType);
                                    m_validTargets.Add(state);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}