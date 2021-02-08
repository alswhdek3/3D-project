using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    private UIFollowTarget m_target;
    [SerializeField]
    private UILabel m_nickLabel;
    [SerializeField]
    private UIProgressBar m_hpBar;
    [SerializeField]
    private HUDText[] m_hudText;

    private int m_hp;
    
    public void SetHp(int currentHp)
    {
        m_hpBar.value = currentHp / (float)m_hp;
    }
    public void SetDamage(DamageType type,int damage)
    {
        if (type == DamageType.None || damage < 0) return;
        switch(type)
        {
            case DamageType.Nomal:
                m_hudText[0].Add(-damage, Color.red, 0f);
                break;
            case DamageType.Critical:
                m_hudText[1].Add(-damage, Color.yellow, 1f);
                break;
            case DamageType.Miss:
                m_hudText[2].Add("Miss", Color.white, 1f);
                break;
        }
    }
    public void SetHud(Transform target,string nick,int hp)
    {
        InitHud();
        m_target.target = target;
        m_target.enabled = true;
        m_nickLabel.text = nick;
        m_hp = hp;
        m_hpBar.value = 1f;
    }
    public void InitHud()
    {
        m_target.gameCamera = Camera.main;
        m_target.uiCamera = transform.parent.parent.GetComponentInChildren<Camera>();
    }
}
