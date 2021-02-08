using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_sponCtrObj;
    private SponPointCtr[] m_sponCtr;
    private WayPointController[] m_wayPointCtr;
    private void Start()
    {
        m_sponCtr = m_sponCtrObj.GetComponentsInChildren<SponPointCtr>();
        m_wayPointCtr = m_sponCtrObj.GetComponentsInChildren<WayPointController>();
    }
    // Update is called once per frame
    void Update()
    {
        /*for(int i=0; i<m_sponCtr.Length; i++)
        {
            if(m_sponCtr[i].IsReady)
            {
                MonsterType type = (MonsterType)Random.Range(0, (int)MonsterType.Max);
                MonsterManager.Instance.CreateMonster(type, m_sponCtr[i].transform.position,m_wayPointCtr[i]);
                m_sponCtr[i].IsReady = false;
            }
        }*/
        if(m_sponCtr[0].IsReady)
        {
            MonsterManager.Instance.CreateMonster(MonsterType.Weapons, m_sponCtr[0].transform.position, m_wayPointCtr[0]);
            m_sponCtr[0].IsReady = false;
        }
    }
}
