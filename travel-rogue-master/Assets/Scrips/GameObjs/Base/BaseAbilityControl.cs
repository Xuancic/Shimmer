using System;
using System.Collections.Generic;
using Ability;
using UnityEngine;

public class BaseAbilityControl : MonoBehaviour, ISubComponent
{
    public bool Spellable
    {
        get => m_spellableMask > 0;
        set => m_spellableMask += value ? 1 : -1;
    }
    private int m_spellableMask = 1;
    
    public List<BaseAbilityAsset> m_allAbilityAsset = new List<BaseAbilityAsset>();
    public List<BaseAbilityAsset.AbilityInstance> m_allAbilityInstance = new List<BaseAbilityAsset.AbilityInstance>();

    private Unit m_unit;
    public virtual void Bind(Unit unit)
    {
        m_unit = unit;
    }

    private void Awake()
    {
        m_allAbilityInstance = new List<BaseAbilityAsset.AbilityInstance>();
        for (var i = m_allAbilityAsset.Count - 1; i >= 0; i--)
        {
            m_allAbilityInstance.Add(m_allAbilityAsset[i].Instantiate(m_unit));
        }
    }
    private void OnDestroy()
    {
        for (var i = m_allAbilityAsset.Count - 1; i >= 0; i--)
        {
            m_allAbilityInstance[i].Shutdown();
        }
    }

    protected void Update()
    {
        var deltaTime = Time.deltaTime;
        for (var i = m_allAbilityInstance.Count - 1; i >= 0; i--)
        {
            var instance = m_allAbilityInstance[i];
            instance.OnUpdate(deltaTime);
        }
    }

    public virtual bool AnySpelling()
    {
        for (var i = m_allAbilityInstance.Count - 1; i >= 0; i--)
        {
            if (m_allAbilityInstance[i].IsSpelling)
            {
                return true;
            }
        }
        return false;
    }
}