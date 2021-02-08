using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolUnit : MonoBehaviour
{
    private string m_effectName;
    private EffectPool m_effectPool;
    private float m_delay = 1f;
    private System.DateTime m_inactiveTime;

    public bool IsReady
    {
        get
        {
            if (!gameObject.activeSelf)
            {
                System.TimeSpan result = System.DateTime.Now - m_inactiveTime;
                if (result.TotalSeconds > m_delay)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public void SetObjectPool(string effectName, EffectPool effectPool)
    {
        m_effectName = effectName;
        m_effectPool = effectPool;
        ResetParent();
    }
    public void ResetParent()
    {
        transform.SetParent(m_effectPool.transform); //Effect를 EffectPool 자식으로 넣는다.
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
    private void OnDisable()
    {
        m_inactiveTime = System.DateTime.Now;
        m_effectPool.AddPoolUnit(m_effectName, this);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
