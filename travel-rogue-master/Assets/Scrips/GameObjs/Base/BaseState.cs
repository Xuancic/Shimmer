using System;
using DG.Tweening;
using UnityEngine;

public enum EDamageBy
{
    Collide,
    PLayer_Link,
    PLayer_Fire,
    Ghost_Fire,
    Booom_Bomb,
    BossSlime_Duang,
}
public class BaseState : MonoBehaviour, ISubComponent
{
    public bool IsAlive => !m_isDead;
    
    public float m_life;
    public float m_maxLife;

    /// <summary>
    /// 无敌
    /// </summary>
    public bool Invincible
    {
        get => m_invincibleMask > 0;
        set
        {
            m_invincibleMask += value ? 1 : -1;
            if (m_invincibleMask < 0)
            {
                m_invincibleMask = 0;
                Debug.LogError("好像有什么不对");
            }
        }
    }
    private int m_invincibleMask;
    
    public static event Action<Unit, float, EDamageBy, Unit> OnBeHitEvent;
    public static event Action<Unit> OnDeadEvent;
    
    protected bool m_isDead;

    private Unit m_unit;
    public virtual void Bind(Unit unit)
    {
        m_unit = unit;
    }

    protected virtual void Update()
    {
        if (m_isDead) return;
        
    }

    public virtual bool ApplyDamage(float damage, EDamageBy damageBy, Unit srcUnit)
    {
        if (m_isDead || Invincible) return false;
        m_life -= damage;
        if (m_life <= 0)
        {
            m_isDead = true;
            m_life = 0f;
            OnBeHitEvent?.Invoke(m_unit, damage, damageBy, srcUnit);
            OnDeadEvent?.Invoke(m_unit);
            OnDead();
        }
        else
        {
            OnBeHitEvent?.Invoke(m_unit, damage, damageBy, srcUnit);
        }
        return true;
    }

    public virtual void OnDead()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.DOScale(0, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}