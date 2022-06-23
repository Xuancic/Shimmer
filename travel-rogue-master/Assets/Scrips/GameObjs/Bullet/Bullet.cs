using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolItem
{
    public string m_targetTag;
    
    private Action m_overTimeCallback;
    private Action<BaseState> m_hitCallback;

    private float m_lifeTime;
    private Rigidbody2D m_rigidbody;
    void IPoolItem.Init(Transform root)
    {
        m_rigidbody = root.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime <= 0f)
        {
            m_overTimeCallback?.Invoke();
        }
    }

    public void SetData(Vector2 vector, float lifeTime, Action<BaseState> hitCallback, Action overTimeCallback)
    {
        m_lifeTime = lifeTime;
        m_rigidbody.velocity = vector;
        m_hitCallback = hitCallback;
        m_overTimeCallback = overTimeCallback;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.UNIT && other.CompareTag(m_targetTag))
        {
            var otherState = other.GetComponent<BaseState>();
            if (otherState != null && otherState.IsAlive)
            {
                m_hitCallback?.Invoke(otherState);
            }
        }
    }
}