using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    private int m_damage;
    MonsterController m_monster;
    public void SetParticle(int damage,MonsterController monster)
    {
        m_damage = damage;
        m_monster = monster;
    }
    private void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            var target = other.GetComponent<PlayerController>();
            if (target != null)
            {
                target.SetDamage(m_damage, m_monster);
                gameObject.SetActive(false);
            }
        }
    }
}
