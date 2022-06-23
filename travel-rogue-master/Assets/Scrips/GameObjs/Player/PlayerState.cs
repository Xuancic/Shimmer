using UnityEngine;

public class PlayerState : BaseState
{
    [Header("体力")]
    public float m_sp;
    public float m_maxSp;
    public float m_spRegen;
    
    [Header("运气")]
    public float m_luck;
    
    protected override void Update()
    {
        base.Update();
        if (m_isDead) return;
        var deltaTime = Time.deltaTime;
        if (m_sp < m_maxSp)
        {
            m_sp = Mathf.Min(m_sp + deltaTime * m_spRegen, m_maxSp);
        }
    }
}