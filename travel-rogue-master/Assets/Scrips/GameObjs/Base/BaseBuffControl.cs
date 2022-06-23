using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[Flags]
public enum ENegativeType
{
    None,
    Stun = 1 << 0, //眩晕
}
public enum EBuffType
{
    Link,
    Captain_Stun,
}
public class BuffInstance
{
    public BuffData buffData;
    
    public float timer;
    public GameObject effect;
}
public class BaseBuffControl : MonoBehaviour
{
    public List<BuffInstance> m_allBuffInstance;
    private Dictionary<EBuffType, BuffInstance> m_buffInstanceDict;

    private BaseState m_state;
    private BaseController m_controller;
    private BaseAbilityControl m_abilityController;
    protected virtual void Awake()
    {
        m_state = GetComponent<BaseState>();
        m_controller = GetComponent<BaseController>();
        m_abilityController = GetComponent<BaseAbilityControl>();
        m_allBuffInstance = new List<BuffInstance>();
        m_buffInstanceDict = new Dictionary<EBuffType, BuffInstance>();
    }
    protected virtual void OnDestroy()
    {
        for (var i = m_allBuffInstance.Count - 1; i >= 0; i--)
        {
            OnInterrupt(m_allBuffInstance[i]);
        }
        m_allBuffInstance.Clear();
    }
    protected virtual void Update()
    {
        if (m_state.IsAlive)
        {
            var deltaTime = Time.deltaTime;
            for (var i = m_allBuffInstance.Count - 1; i >= 0; i--)
            {
                var buffInstance = m_allBuffInstance[i];
                buffInstance.timer += deltaTime;
                if (buffInstance.buffData.duration <= 0 || buffInstance.timer < buffInstance.buffData.duration)
                {
                    
                }
                else
                {
                    m_allBuffInstance.RemoveAt(i);
                    m_buffInstanceDict.Remove(buffInstance.buffData.type);
                    OnOverTime(buffInstance);
                }
            }
        }
        else
        {
            for (var i = m_allBuffInstance.Count - 1; i >= 0; i--)
            {
                OnInterrupt(m_allBuffInstance[i]);
            }
            m_allBuffInstance.Clear();
        }
    }

    public virtual BuffInstance AddBuff(EBuffType buffType)
    {
        var data = BuffIndexer.Instance[buffType];
        if (!m_buffInstanceDict.ContainsKey(buffType))
        {
            var buffInstance = new BuffInstance
            {
                buffData = data,
            };
            if (data.effectPrefab != null)
            {
                buffInstance.effect = Instantiate(data.effectPrefab, transform);
            }
            //眩晕
            if ((data.negativeType & ENegativeType.Stun) != 0)
            {
                m_controller.Controlable = false;
                if (m_abilityController != null) m_abilityController.Spellable = false;
            }
            m_allBuffInstance.Add(buffInstance);
            m_buffInstanceDict.Add(buffType, buffInstance);
            return buffInstance;
        }
        else
        {
            var buffData = m_buffInstanceDict[buffType];
            buffData.timer = 0f;
            return buffData;
        }
    }
    public virtual void RemoveBuff(EBuffType buffType)
    {
        if (m_buffInstanceDict.TryGetValue(buffType, out var buffData))
        {
            m_allBuffInstance.Remove(buffData);
            m_buffInstanceDict.Remove(buffType);
            OnInterrupt(buffData);
        }
    }
    public virtual bool HasBuff(EBuffType buffType)
    {
        return m_buffInstanceDict.ContainsKey(buffType);
    }

    protected virtual void OnInterrupt(BuffInstance buffInstance)
    {
        if (buffInstance.effect != null)
        {
            Destroy(buffInstance.effect);
        }
        //眩晕
        if ((buffInstance.buffData.negativeType & ENegativeType.Stun) != 0)
        {
            m_controller.Controlable = true;
            if (m_abilityController != null) m_abilityController.Spellable = true;
        }
    }
    protected virtual void OnOverTime(BuffInstance buffInstance)
    {
        if (buffInstance.effect != null)
        {
            Destroy(buffInstance.effect);
        }
        //眩晕
        if ((buffInstance.buffData.negativeType & ENegativeType.Stun) != 0)
        {
            m_controller.Controlable = true;
            if (m_abilityController != null) m_abilityController.Spellable = true;
        }
    }
}