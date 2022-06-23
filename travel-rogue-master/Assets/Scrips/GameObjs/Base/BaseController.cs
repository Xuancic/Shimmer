using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BaseController : MonoBehaviour, ISubComponent
{
    public Rigidbody2D Rigidbody => m_rigidbody;
    
    [Header("移动")]
    [SerializeField]
    protected float m_moveSpeed;
    public float MoveSpeed
    {
        get
        {
            var baseValue = m_moveSpeed;
            var percentAdd = 0f;
            for (var i = m_unit.m_propAddList.Count - 1; i >= 0; i--)
            {
                var propAdd = m_unit.m_propAddList[i];
                if (propAdd.type == EPropType.MoveSpeed)
                {
                    baseValue += propAdd.value;
                }
                else if (propAdd.type == EPropType.MoveSpeedPercent)
                {
                    percentAdd += propAdd.value;
                }
            }
            return baseValue * (1 + percentAdd);
        }
    }
    
    [Header("碰撞")]
    public float m_collideDamage;
    public float m_beCollidedDuration;
    protected bool m_beCollideding;
    protected float m_beCollidedTimer;
    
    public bool Controlable
    {
        get => m_controlableMask > 0;
        set
        {
            m_controlableMask += value ? 1 : -1;
            //失去控制时重置位移
            if (!value)
            {
                ResetMovement();
            }
        }
    }
    private int m_controlableMask = 1;

    protected Unit m_unit;
    public virtual void Bind(Unit unit)
    {
        m_unit = unit;
    }

    public event Action<Unit> OnBeUnitCollided;
    public event Action<Vector2> OnBeObstacleCollided;

    protected BaseState m_state;
    protected Rigidbody2D m_rigidbody;
    protected virtual void Awake()
    {
        m_state = GetComponent<BaseState>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        BaseState.OnDeadEvent += OnDeadEvent;
    }
    protected virtual void OnDestroy()
    {
        BaseState.OnDeadEvent -= OnDeadEvent;
    }
    protected virtual void Update()
    {
        if (m_beCollideding)
        {
            var deltaTime = Time.deltaTime;
            m_beCollidedTimer += deltaTime;
            if (m_beCollidedTimer < m_beCollidedDuration)
            {
                RefreshBeCollided(deltaTime, m_beCollidedTimer / m_beCollidedDuration);
            }
            else
            {
                EndBeCollided();
            }
        }
    }

    public virtual void ResetMovement()
    {
        m_rigidbody.velocity = Vector2.zero;
    }
    public virtual void StartBeCollided(float collideDamage, Unit srcUnit, Vector3 srcPos)
    {
        //受到伤害并开始击退
        m_state.ApplyDamage(collideDamage, EDamageBy.Collide, srcUnit);
        m_beCollideding = true;
        m_beCollidedTimer = 0f;
    }
    public virtual void RefreshBeCollided(float deltaTime, float percent)
    {
        
    }
    public virtual void EndBeCollided()
    {
        m_beCollideding = false;
    }

    //回调
    private void OnDeadEvent(Unit unit)
    {
        if (unit == m_unit)
        {
            ResetMovement();
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (m_state.Invincible) return;
        var otherRoot = other.transform;
        if (otherRoot.gameObject.layer == Layers.UNIT && !otherRoot.CompareTag(tag))
        {
            var collideDamage = otherRoot.GetComponent<BaseController>().m_collideDamage;
            var srcUnit = otherRoot.GetComponent<Unit>();
            if (collideDamage > 0)
            {
                StartBeCollided(collideDamage, srcUnit, otherRoot.position);
            }
            OnBeUnitCollided?.Invoke(srcUnit);
        }
        else
        {
            OnBeObstacleCollided?.Invoke(other.contacts[0].point);
        }
    }
}