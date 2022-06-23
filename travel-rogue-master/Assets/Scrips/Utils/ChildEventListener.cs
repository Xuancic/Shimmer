using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildEventListener : MonoBehaviour
{
    public Action<Unit> OnUnitEnter;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.UNIT && !other.isTrigger)
        {
            var unit = other.GetComponent<Unit>();
            if (unit.GetComponent<BaseState>().IsAlive)
            {
                OnUnitEnter?.Invoke(unit);
            }
        }
    }
}
