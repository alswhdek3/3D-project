using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackArea : MonoBehaviour
{
    private bool m_isTarget;
    public bool IsTarget { get { return m_isTarget; } }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            m_isTarget = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_isTarget = false;
        }
    }
}
