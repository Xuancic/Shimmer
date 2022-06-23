using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : BaseController
{
    [Header("被碰撞位移")]
    public float m_beCollidedMoveSpeed;
    public AnimationCurve m_beCollidedMoveCurve;
    private Vector2 m_beCollidedMoveDir;
    
    [Header("冲刺")]
    public float m_dashSpeed;
    public float m_dashDuration;
    public AnimationCurve m_dashCurve;
    public float m_dashCostSp;
    public bool Dashable
    {
        get => m_dashableMask > 0;
        set
        {
            m_dashableMask += value ? 1 : -1;
            //失去控制时重置位移
            if (!value)
            {
                ResetMovement();
            }
        }
    }
    private int m_dashableMask = 1;
    private bool m_dashing;
    private float m_dashTimer;
    private Vector2 m_dashDir;

    private Camera m_mainCamera;
    protected override void Awake()
    {
        base.Awake();
        m_mainCamera = Camera.main;
    }
    protected override void Update()
    {
        base.Update();
        if (!m_state.IsAlive) return;
        
        var deltaTime = Time.deltaTime;
        var controlable = Controlable;
        
        if (!m_dashing && controlable)
        {
            var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            //移动
            if (dir != Vector2.zero)
            {
                m_rigidbody.position += dir * (MoveSpeed * deltaTime);
            }
            //冲刺
            var playerState = m_state as PlayerState;
            if (Dashable && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && playerState.m_sp >= m_dashCostSp)
            {
                m_dashing = true;
                m_dashTimer = 0f;
                //记录方向
                var up = transform.up;
                m_dashDir = dir != Vector2.zero ? dir : new Vector2(up.x, up.y);
                //消耗
                playerState.m_sp -= m_dashCostSp;
            }
        }

        if (controlable)
        {
            //朝向
            var worldPoint = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPoint.z = 0f;
            transform.up = (worldPoint - transform.position).normalized;
        }
        
        //冲刺
        if (m_dashing)
        {
            m_dashTimer += deltaTime;
            if (m_dashTimer < m_dashDuration)
            {
                var speed = m_dashCurve.Evaluate(m_dashTimer / m_dashDuration) * m_dashSpeed * deltaTime;
                m_rigidbody.position += m_dashDir * speed;
            }
            else
            {
                m_dashing = false;
            }
        }
    }
    
    public override void ResetMovement()
    {
        base.ResetMovement();
        m_dashing = false;
    }
    public override void StartBeCollided(float collideDamage, Unit srcUnit, Vector3 srcPos)
    {
        base.StartBeCollided(collideDamage, srcUnit, srcPos);
        //无敌并失去控制
        m_state.Invincible = true;
        Controlable = false;
        //开始被击退
        m_beCollidedMoveDir = (transform.position - srcPos).normalized;
    }
    public override void RefreshBeCollided(float deltaTime, float percent)
    {
        base.RefreshBeCollided(deltaTime, percent);
        var speed = m_beCollidedMoveCurve.Evaluate(percent) * m_beCollidedMoveSpeed * deltaTime;
        m_rigidbody.position += m_beCollidedMoveDir * speed;
    }
    public override void EndBeCollided()
    {
        base.EndBeCollided();
        m_state.Invincible = false;
        Controlable = true;
    }
}