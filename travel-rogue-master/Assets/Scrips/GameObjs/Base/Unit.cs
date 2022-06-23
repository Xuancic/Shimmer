using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum EPropAddSrcType
{
    Player_Link,
    Captain_Rash,
}
public enum EPropType
{
    MoveSpeed,
    MoveSpeedPercent,
}
public struct PropAdd
{
    public EPropAddSrcType srcType;
    public EPropType type;
    public float value;
}
[DefaultExecutionOrder(-100)]
public class Unit : MonoBehaviour
{
    public List<PropAdd> m_propAddList = new List<PropAdd>();
    
    public int rank;

    protected virtual void Awake()
    {
        var subComponents = GetComponents<ISubComponent>();
        for (var i = subComponents.Length - 1; i >= 0; i--)
        {
            subComponents[i].Bind(this);
        }
    }
    protected virtual void Start()
    {
        
    }
}