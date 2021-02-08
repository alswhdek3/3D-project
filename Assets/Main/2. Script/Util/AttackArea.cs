using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    List<GameObject> m_unitList = new List<GameObject>();

    public List<GameObject> UnitList { get { return m_unitList; }}

    public void RemoveMonster(GameObject monster)
    {
        m_unitList.Remove(monster);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Monster"))
        {
            m_unitList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            m_unitList.Remove(other.gameObject);
        }
    }
}
