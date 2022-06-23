using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour, ISubComponent
{
    [Header("伤害")]
    public float m_atkDamage;
    [Header("子弹")]
    public float m_bulletSpeed;
    public float m_bulletLifeTime;
    public GameObject m_bulletPrefab;
    public float m_fireInterval;

    private float m_intervalTimer;

    private Transform m_gunPos;
    private SimpleItemPool<Bullet> m_bulletPool;

    public void Bind(Unit unit)
    {
        m_unit = unit;
    }

    private Unit m_unit;
    private Camera m_mainCamera;
    private BaseAbilityControl m_abilityControl;
    private void Awake()
    {
        m_mainCamera = Camera.main;
        m_abilityControl = GetComponent<BaseAbilityControl>();
        
        m_bulletPool = new SimpleItemPool<Bullet>(m_bulletPrefab, null, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        });

        m_gunPos = transform.Find("GunPos");
        
    }
    private void Update()
    {
        var deltaTime = Time.deltaTime;
        if (m_intervalTimer > 0)
        {
            m_intervalTimer -= deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && !m_abilityControl.AnySpelling())
        {
            if (m_intervalTimer <= 0)
            {
                var worldPoint = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
                worldPoint.z = 0f;
                var fireDir = (worldPoint - transform.position).normalized;
            
                var bullet = m_bulletPool.Pop();
                bullet.transform.position = m_gunPos.position;
                bullet.transform.up = fireDir;
                bullet.SetData(fireDir * m_bulletSpeed, m_bulletLifeTime, otherState =>
                {
                    m_bulletPool.Push(bullet);
                    otherState.ApplyDamage(m_atkDamage, EDamageBy.PLayer_Fire, m_unit);
                }, () =>
                {
                    m_bulletPool.Push(bullet);
                });

                m_intervalTimer = m_fireInterval;
            }
        }
    }
}