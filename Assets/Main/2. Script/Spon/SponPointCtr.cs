using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SponPointCtr : MonoBehaviour
{
    private float m_time;
    private float m_duration = 10f;
    private bool m_isReady;

    public bool IsReady { get { return m_isReady; }set { m_isReady = value; } }
    public void SetDuration(float duration)
    {
        m_duration = duration;
    }
    private IEnumerator monsterCreatePatten()
    {
        while(true)
        {
            if(!m_isReady)
            {
                m_time += Time.deltaTime;
                if (m_time >= m_duration)
                {
                    m_time = 0f;
                    m_isReady = true;
                }
            }
            yield return null;
            
        }
    }
    private void Start()
    {
        StartCoroutine(monsterCreatePatten());
    }
}
