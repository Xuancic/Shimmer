using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Ability
{
    [CreateAssetMenu(menuName = "Travel/Captain/Rash", fileName = "Captain_Rash")]
    public class Captain_Rash : BaseAbilityAsset
    {
        [Tooltip("伤害系数")]
        public float m_damageFactor = 1f;
        [Tooltip("加速曲线")] 
        public AnimationCurve m_addCurve;
        [Tooltip("加速时间")] 
        public float m_maxAddTime;
        [Tooltip("眩晕buff")] 
        public EBuffType m_stunBuff;
        
        public override AbilityInstance Instantiate(Unit unit)
        {
            var instance = new Instance();
            instance.Init(this, unit);
            return instance;
        }
        
        public class Instance : AbilityInstance
        {
            public override EAbilityType AbilityType => EAbilityType.Captain_Rash;

            private bool m_loseControl;
            
            private Captain_Rash m_asset;
            private Unit m_unit;
            private Transform m_root;
            private EnemyController m_controller;
            private BaseBuffControl m_buffController;
            public override void Init(BaseAbilityAsset asset, Unit unit)
            {
                base.Init(asset, unit);
                m_asset = asset as Captain_Rash;
                
                m_controller = unit.GetComponent<EnemyController>();
                m_buffController = unit.GetComponent<BaseBuffControl>();
                m_root = unit.transform;
                m_unit = unit;
            }
            
            protected override bool OnBegin()
            {
                if (!base.OnBegin()) return false;
                m_loseControl = false;
                
                m_controller.OnBeUnitCollided += OnBeUnitCollided;
                m_controller.OnBeObstacleCollided += OnBeObstacleCollided;
                return true;
            }
            protected override void OnEnd()
            {
                base.OnEnd();
                if (m_loseControl)
                {
                    m_controller.Controlable = true;
                    m_loseControl = false;
                }
                m_controller.m_collideDamage = 0f;
                m_unit.m_propAddList.Remove(EPropAddSrcType.Captain_Rash);
                
                m_controller.OnBeUnitCollided -= OnBeUnitCollided;
                m_controller.OnBeObstacleCollided -= OnBeObstacleCollided;
            }
            public override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                var target = m_controller.Target;
                if (!m_isSpelling)
                {
                    if (target != null)
                    {
                        OnBegin();
                        return;
                    }
                }
                else
                {
                    if (target == null)
                    {
                        OnEnd();
                        return;
                    }
                    //加速
                    var additive = m_asset.m_addCurve.Evaluate(math.min(m_timer / m_asset.m_maxAddTime, 1f));
                    m_unit.m_propAddList.Set(EPropAddSrcType.Captain_Rash, new PropAdd
                    {
                        type = EPropType.MoveSpeedPercent,
                        value = additive,
                    });
                    //修改碰撞伤害
                    m_controller.m_collideDamage = m_asset.m_damageFactor * m_controller.MoveSpeed;
                    //达到最大速度后失控
                    if (!m_loseControl)
                    {
                        if (m_timer >= m_asset.m_maxAddTime)
                        {
                            m_controller.Controlable = false;
                            m_loseControl = true;
                        }
                    }
                    else
                    {
                        m_root.position += m_root.up * (m_controller.MoveSpeed * deltaTime);
                    }
                }
            }
            
            //回调
            private void OnBeUnitCollided(Unit otherUnit)
            {
                Debug.LogError($"{otherUnit.tag}, {m_unit.tag}");
                if (!otherUnit.CompareTag(m_unit.tag))
                {
                    m_buffController.AddBuff(m_asset.m_stunBuff);
                    OnEnd();
                }
            }
            private void OnBeObstacleCollided(Vector2 point)
            {
                if (m_timer >= m_asset.m_maxAddTime)
                {
                    m_buffController.AddBuff(m_asset.m_stunBuff);
                    OnEnd();
                }
            }
        }
    }
}