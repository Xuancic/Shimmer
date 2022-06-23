using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void BeAttacked(int damage, Vector2 dir, float force);

}
