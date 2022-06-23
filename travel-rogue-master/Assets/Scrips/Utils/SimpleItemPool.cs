﻿﻿using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolItem
{
    void Init(Transform root);
}
public class SimpleItemPool<T> where T : IPoolItem, new()
{
    private readonly GameObject m_prefab;
    private readonly Action<T> m_initCallback;
    private readonly Action<T> m_popCallback;
    private readonly Action<T> m_pushCallback;
    private readonly List<T> m_pool = new List<T>();

    public SimpleItemPool(GameObject prefab, Action<T> initCallback, Action<T> popCallback, Action<T> pushCallback)
    {
        m_prefab = prefab;
        m_initCallback = initCallback;
        m_popCallback = popCallback;
        m_pushCallback = pushCallback;
    }
    
    /// <summary>
    /// 生成一个T
    /// </summary>
    public T Pop()
    {
        T item;
        if (m_pool.Count > 0)
        {
            item = m_pool[0];
            m_pool.RemoveAt(0);
        }
        else
        {
            var newObj = UnityEngine.Object.Instantiate(m_prefab).transform;
            if (typeof(T).BaseType == typeof(MonoBehaviour))
            {
                item = newObj.GetComponent<T>();
            }
            else
            {
                item = new T();
            }
            item.Init(newObj);
            m_initCallback?.Invoke(item);
        }
        m_popCallback?.Invoke(item);
        return item;
    }
    /// <summary>
    /// 回收一个T
    /// </summary>
    public void Push(T item)
    {
        m_pushCallback?.Invoke(item);
        m_pool.Add(item);
    }
    /// <summary>
    /// 回收T列表
    /// </summary>
    public void Push(List<T> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            Push(list[i]);
        }
        list.Clear();
    }
}