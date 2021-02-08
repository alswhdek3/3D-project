using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{  
    [SerializeField]
    private UISprite m_durationGauge;
    [SerializeField]
    private UILabel m_durationLabel;     

    private float m_duration;
    private float m_time;

    private ButtonDelFun m_pressBtnDel; //버튼을 눌렀을때
    private ButtonDelFun m_releseBtnDel; //버튼에서 손을 놓았을때

    private ActionButtonType m_type;
    private bool m_isReady;
    public bool IsReady { get {return m_isReady = true; } set { m_isReady = value; } }

    public void SetButton(ActionButtonType type,ButtonDelFun pressBtnDel,ButtonDelFun releseBtnDel=null)
    {
        m_type = type;       
        m_pressBtnDel = pressBtnDel;
        m_releseBtnDel = releseBtnDel;
        m_isReady = true;        
    }
    public void SetDeration(float duration)
    {
        m_duration = duration;
        m_durationGauge.fillAmount = 0f;
        m_durationLabel.text = string.Empty;
    }
    public void OnPressActionBtn()
    {
        if(m_pressBtnDel != null)
        {
            if(m_isReady)
            {
                m_pressBtnDel();
                if(m_duration > 0f)
                {
                    m_isReady = false;
                }
            }
        }
    }
    public void OnReleseActionBtn()
    {
        if (m_releseBtnDel != null)
        {
            m_releseBtnDel();
        }
    }

    void Update()
    {
        if(!m_isReady)
        {
            m_time += Time.deltaTime;
            m_durationLabel.text = Mathf.FloorToInt(m_duration - m_time).ToString();
            m_durationGauge.fillAmount = m_time / m_duration;

            if(m_time >= m_duration)
            {
                m_isReady = true;
                m_durationLabel.text = string.Empty;
                m_durationGauge.fillAmount = 0f;
                m_time = 0f;
            }
        }
    }
}
