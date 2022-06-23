using System;
using UnityEngine;

public class EnemyController : BaseController
{
    [Header("仇恨")]
    [Tooltip("出击距离")]
    public float m_inOfFightRange;
    public ChildEventListener m_inOfFightTrigger;
    [Tooltip("放弃追击距离")]
    public float m_outOfFightRange;
    [Tooltip("最小距离")]
    public float m_minRange;

    [Header("移动方式")] 
    public EMovementType m_moveType;
    public float m_moveDuration;
    public AnimationCurve m_moveCurve;
    private float m_moveTimer;
    public enum EMovementType
    {
        Normal,
        Jump,
    }

    /// <summary>
    /// 仇恨目标
    /// </summary>
    public Player Target => m_target;
    private Player m_target;
    private float m_radiusOffset;
    
    private BaseAbilityControl m_abilityControl;
    protected override void Awake()
    {
        base.Awake();
        m_abilityControl = GetComponent<BaseAbilityControl>();
        
        m_inOfFightTrigger.GetComponent<CircleCollider2D>().radius = m_inOfFightRange;
        m_inOfFightTrigger.OnUnitEnter = OnUnitEnter;
        
        OnBeUnitCollided += OnBeUnitCollidedEvent;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_inOfFightTrigger.OnUnitEnter = null;
        
        OnBeUnitCollided -= OnBeUnitCollidedEvent;
    }
    protected override void Update()
    {
        base.Update();
        if (!m_state.IsAlive) return;
        
        if (m_target != null)
        {
            if (!m_target.State.IsAlive)
            {
                m_target = null;
                return;
            }

            if (Controlable)
            {
                var delta = m_target.transform.position - transform.position;
                var disSq = Vector2.Dot(delta, delta);
                var radiusSq = m_outOfFightRange + m_radiusOffset;
                radiusSq *= radiusSq;
                //逃脱距离
                if (disSq > radiusSq * 1.1f)
                {
                    m_target = null;
                }
                else 
                {
                    //最小距离
                    if (disSq > m_minRange * m_minRange)
                    {
                        var deltaTime = Time.deltaTime;
                        var dir = delta.normalized;
                        switch (m_moveType)
                        {
                            case EMovementType.Normal:
                            {
                                transform.position += dir * (MoveSpeed * deltaTime);
                                break;
                            }
                            case EMovementType.Jump:
                            {
                                if (m_moveTimer < m_moveDuration)
                                {
                                    m_moveTimer += deltaTime;
                                    var percent = m_moveTimer / m_moveDuration;
                                    transform.position += dir * (MoveSpeed * m_moveCurve.Evaluate(percent) * deltaTime);
                                }
                                else
                                {
                                    m_moveTimer = 0f;
                                }
                                break;
                            }
                        }
                        transform.up = dir;
                    }
                }
            }
        }
    }
    
    //回调
    private void OnUnitEnter(Unit unit)
    {
        if (unit is Player player && m_target == null)
        {
            m_target = player;
            m_radiusOffset = unit.GetComponent<CircleCollider2D>().radius;
        }
    }
    private void OnBeUnitCollidedEvent(Unit otherUnit)
    {
        if (m_target == null)
        {
            if (otherUnit is Player player)
            {
                m_target = player;
                m_radiusOffset = otherUnit.GetComponent<CircleCollider2D>().radius;
            }
        }
    }
}