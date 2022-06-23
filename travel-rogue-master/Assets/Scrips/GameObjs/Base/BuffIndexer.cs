using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class BuffData
{
    public EBuffType type;
    public ENegativeType negativeType;
    public float duration;
    public GameObject effectPrefab;
}
[CreateAssetMenu(menuName = "Travel/BuffIndexer", fileName = "BuffIndexer")]
public class BuffIndexer : ScriptableObject
{
    public static BuffIndexer Instance;
    public BuffData[] m_buffData;
    private Dictionary<EBuffType, BuffData> m_buffDict;

    public BuffData this[EBuffType type]
    {
        get => m_buffDict[type];
    }
    public void Initialize()
    {
        Instance = this;
        m_buffDict = new Dictionary<EBuffType, BuffData>();
        for (var i = m_buffData.Length - 1; i >= 0; i--)
        {
            var buffData = m_buffData[i];
            m_buffDict[buffData.type] = buffData;
        }
    }
}