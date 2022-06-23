using System.Collections.Generic;
using UnityEngine;

namespace Ability
{
    [CreateAssetMenu(menuName = "Travel/Ghost/Fire", fileName = "Ghost_Fire")]
    public class Ghost_Fire : BaseAbilityAsset
    {
        [Tooltip("伤害")]
        public float m_damage = 0.7f;
        [Tooltip("子弹速度")]
        public float m_bulletSpeed = 5f;
        [Tooltip("子弹生存时间")]
        public float m_bulletLifeTime = 2f;
        [Tooltip("子弹预制体")]
        public GameObject m_bulletPrefab;
        [Tooltip("射击间隔")]
        public float m_fireInterval = 2f;
        [Tooltip("后摇时间")]
        public float m_fireStopTime = 0.2f;
        [Tooltip("发射距离")]
        public float m_fireDistance = 5f;
        
        public override AbilityInstance Instantiate(Unit unit)
        {
            var instance = new Instance();
            instance.Init(this, unit);
            return instance;
        }
        
        public class Instance : AbilityInstance
        {
            public override EAbilityType AbilityType => EAbilityType.Ghost_Fire;

            private float m_intervalTimer;
            private SimpleItemPool<Bullet> m_bulletPool;

            private Ghost_Fire m_asset;
            private Unit m_unit;
            private Transform m_root;
            private EnemyController m_controller;
            public override void Init(BaseAbilityAsset asset, Unit unit)
            {
                base.Init(asset, unit);
                m_asset = asset as Ghost_Fire;
                
                m_controller = unit.GetComponent<EnemyController>();
                m_root = unit.transform;
                
                m_bulletPool = new SimpleItemPool<Bullet>(m_asset.m_bulletPrefab, null, bullet =>
                {
                    bullet.gameObject.SetActive(true);
                }, bullet =>
                {
                    bullet.gameObject.SetActive(false);
                });
            }
            
            protected override bool OnBegin()
            {
                if (!base.OnBegin()) return false;
                //停止移动
                m_controller.Controlable = false;
                //发射子弹
                var target = m_controller.Target;
                if (target != null && m_intervalTimer <= 0)
                {
                    var fireDir = (target.transform.position - m_root.position).normalized;
            
                    var bullet = m_bulletPool.Pop();
                    bullet.transform.position = m_root.position;
                    bullet.transform.up = fireDir;
                    bullet.SetData(fireDir * m_asset.m_bulletSpeed, m_asset.m_bulletLifeTime, otherState =>
                    {
                        m_bulletPool.Push(bullet);
                        otherState.ApplyDamage(m_asset.m_damage, EDamageBy.Ghost_Fire, m_unit);
                    }, () =>
                    {
                        m_bulletPool.Push(bullet);
                    });

                    m_intervalTimer = m_asset.m_fireInterval;
                }
                return true;
            }
            protected override void OnEnd()
            {
                base.OnEnd();
                //恢复移动
                m_controller.Controlable = true;
            }
            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (!m_isSpelling)
                {
                    if (m_intervalTimer > 0)
                    {
                        m_intervalTimer -= deltaTime;
                    }

                    var target = m_controller.Target;
                    if (target != null && m_intervalTimer <= 0)
                    {
                        var delta = target.transform.position - m_root.position;
                        var disSq = Vector2.Dot(delta, delta);
                        var fireDistanceSq = m_asset.m_fireDistance * m_asset.m_fireDistance;
                        if (disSq < fireDistanceSq)
                        {
                            OnBegin();
                        }
                    }
                }
                else
                {
                    if (m_timer >= m_asset.m_fireStopTime)
                    {
                        OnEnd();
                    }
                }
            }
        }
    }
}