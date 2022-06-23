using UnityEngine;

namespace Ability
{
    public enum EAbilityType
    {
        Link,
        Ghost_Fire,
        Captain_Rash,
        Booom_Bomb,
        Spliter_Split,
    }
    public abstract class BaseAbilityAsset : ScriptableObject
    {
        public abstract AbilityInstance Instantiate(Unit unit);
        
        public abstract class AbilityInstance
        {
            public abstract EAbilityType AbilityType { get; }
            
            public bool IsSpelling { get; }
            protected bool m_isSpelling;
            protected float m_timer;

            protected BaseState m_state;
            protected BaseAbilityControl m_abilityControl;
            public virtual void Init(BaseAbilityAsset asset, Unit unit)
            {
                m_state = unit.GetComponent<BaseState>();
                m_abilityControl = unit.GetComponent<BaseAbilityControl>();
            }
            public virtual void Shutdown()
            {
                
            }

            protected virtual bool CanSpell()
            {
                return !m_isSpelling && m_abilityControl.Spellable && m_state.IsAlive;
            }
            protected virtual bool OnBegin()
            {
                if (!CanSpell())
                {
                    return false;
                }
                m_timer = 0f;
                m_isSpelling = true;
                m_abilityControl.Spellable = false;
                return true;
            }
            protected virtual void OnEnd()
            {
                m_isSpelling = false;
                m_abilityControl.Spellable = true;
            }
            public virtual void OnUpdate(float deltaTime)
            {
                m_timer += deltaTime;
            }
        }
    }
}