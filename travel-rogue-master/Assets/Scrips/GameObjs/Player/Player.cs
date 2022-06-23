using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : Unit
{
    public PlayerController Controller
    {
        get
        {
            if (m_controller == null)
            {
                m_controller = GetComponent<PlayerController>();
            }
            return m_controller;
        }
    }
    private PlayerController m_controller;
    
    public PlayerState State
    {
        get
        {
            if (m_state == null)
            {
                m_state = GetComponent<PlayerState>();
            }
            return m_state;
        }
    }
    private PlayerState m_state;

    public Vector2 coordInMap;
    private Level level;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        coordInMap = Vector2.zero;
        level = GameManager.Instance.level;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Door"))
        {
            if (col.transform.name=="DoorUp")
            {
                level.MoveToNextRoom(Vector2.up);
            }

            if (col.transform.name=="DoorDown")
            {
                level.MoveToNextRoom(Vector2.down);
            }
            if (col.transform.name=="DoorLeft")
            {
                level.MoveToNextRoom(Vector2.left);
            }

            if (col.transform.name=="DoorRight")
            {
                level.MoveToNextRoom(Vector2.right);
            }
        }
    }
}