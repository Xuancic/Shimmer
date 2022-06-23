using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : MonoBehaviour
{
    public Image m_playerHpBar;
    public Image m_playerSpBar;
    
    private class UI_HpBarItem : IPoolItem
    {
        private RectTransform m_parent;
        private Transform m_root;
        
        private Transform m_unitRoot;
        private BaseState m_state;
        private Image m_hpBar;

        public void Init(Transform root)
        {
            m_hpBar = root.Find("Bar").GetComponent<Image>();
            m_root = root;
        }
        public void SetParent(Transform parent)
        {
            m_parent = parent as RectTransform;
            m_root.SetParent(parent, false);
        }

        public void SetOwner(Unit unit)
        {
            m_state = unit.GetComponent<BaseState>();
            m_unitRoot = unit.transform;
            
            m_hpBar.fillAmount = m_state.m_life / m_state.m_maxLife;
            RefreshPosition();
        }
        public void Refresh(float deltaTime)
        {
            m_hpBar.fillAmount = Mathf.Lerp(m_hpBar.fillAmount, m_state.m_life / m_state.m_maxLife, deltaTime * 5f);
            RefreshPosition();
        }

        private void RefreshPosition()
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(GameManager.Instance.playerCamera, m_unitRoot.position + new Vector3(0, 0.75f));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parent, screenPoint, GameManager.Instance.uiCamera, out var localPoint);
            m_root.localPosition = localPoint;
        }
    }
    private SimpleItemPool<UI_HpBarItem> m_hpBarPool;
    private List<UI_HpBarItem> m_hpBarList;
    private Dictionary<Unit, UI_HpBarItem> m_hpBarDict;
    public GameObject m_hpBarPrefab;
    public Transform m_hpBarRoot;

    private Player m_player;
    private PlayerState m_playerState;
    private void Awake()
    {
        GameManager.Instance.OnLoadGame += OnLoadGame;
        BaseState.OnBeHitEvent += OnBeHitEvent;
        BaseState.OnDeadEvent += OnDeadEvent;
        
        m_hpBarPool = new SimpleItemPool<UI_HpBarItem>(m_hpBarPrefab, null, item =>
        {
            m_hpBarList.Add(item);
            item.SetParent(m_hpBarRoot);
        }, item =>
        {
            m_hpBarList.Remove(item);
            item.SetParent(null);
        });
        m_hpBarList = new List<UI_HpBarItem>();
        m_hpBarDict = new Dictionary<Unit, UI_HpBarItem>();
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnLoadGame -= OnLoadGame;
        BaseState.OnBeHitEvent -= OnBeHitEvent;
        BaseState.OnDeadEvent -= OnDeadEvent;
    }
    private void Update()
    {
        if (m_player == null) return;
        var deltaTime = Time.deltaTime;
        m_playerHpBar.fillAmount = Mathf.Lerp(m_playerHpBar.fillAmount, m_playerState.m_life / m_playerState.m_maxLife, deltaTime * 5f);
        m_playerSpBar.fillAmount = Mathf.Lerp(m_playerSpBar.fillAmount, m_playerState.m_sp / m_playerState.m_maxSp, deltaTime * 5f);
        for (var i = m_hpBarList.Count - 1; i >= 0; i--)
        {
            m_hpBarList[i].Refresh(deltaTime);
        }
    }

    //回调
    private void OnLoadGame()
    {
        m_player = GameManager.Instance.player;
        m_playerState = m_player.GetComponent<PlayerState>();
        m_playerHpBar.fillAmount = m_playerSpBar.fillAmount = 1f;
    }
    private void OnBeHitEvent(Unit self, float damage, EDamageBy damageBy, Unit srcUnit)
    {
        if (self != m_player && !m_hpBarDict.ContainsKey(self))
        {
            var item = m_hpBarPool.Pop();
            m_hpBarDict.Add(self, item);
            item.SetOwner(self);
        }
    }
    private void OnDeadEvent(Unit self)
    {
        if (m_hpBarDict.TryGetValue(self, out var item))
        {
            m_hpBarPool.Push(item);
            m_hpBarDict.Remove(self);
        }
    }
}