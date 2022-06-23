using System;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{
    public Vector3 eulerAngle;

    private Transform m_transform;
    private void Awake()
    {
        m_transform = transform;
    }
    private void LateUpdate()
    {
        m_transform.eulerAngles = eulerAngle;
    }
}