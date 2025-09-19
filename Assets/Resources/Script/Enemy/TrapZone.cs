using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapZone : MonoBehaviour
{
    [SerializeField] private BoxCollider2D[] m_BoxCollider;
    [SerializeField] private Vector2 _fullSize;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < m_BoxCollider.Length; ++i)
            {
                m_BoxCollider[i].size = _fullSize;
                m_BoxCollider[i].offset = Vector2.zero;
            }
        }
    }
}
