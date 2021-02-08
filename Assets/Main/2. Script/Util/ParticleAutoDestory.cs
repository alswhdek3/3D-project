using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestory : MonoBehaviour
{
    private bool m_isFinished;
    private ParticleSystem[] m_particles;
    private System.DateTime m_activeTime;
    private float m_duration = 2f;
    public bool IsFinished { get { return m_isFinished; } set { m_isFinished = value; } }

    private void EffectEnd()
    {
        gameObject.SetActive(false);
    }
    private void Start()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }
    private void OnEnable()
    {
        m_activeTime = System.DateTime.Now;
        m_isFinished = false;
    }
    private void Update()
    {
        System.TimeSpan result = System.DateTime.Now - m_activeTime;
        if(result.TotalSeconds >= m_duration)
        {
            if(!m_isFinished)
            {
                m_isFinished = true;
                EffectEnd();
            }
        }
    }
}
