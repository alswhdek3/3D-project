using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionButtonType
{
    None=-1,
    ComboAttack,
    Knock,
    Fire,
    UItimate,
    Max
}

public delegate void ButtonDelFun();
public class ActionManager : SingtonMonoBehaviour<ActionManager>
{  
    private ActionButton[] m_actionButton;
    private ButtonDelFun m_pressBtnDel;
    private ButtonDelFun m_releseBtnDel;

    void Start()
    {
        m_actionButton = gameObject.GetComponentsInChildren<ActionButton>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetButton(ActionButtonType type,float duration,ButtonDelFun pressBtnDel,ButtonDelFun releseBtnDel = null)
    {
        m_actionButton[(int)type].SetButton(type, pressBtnDel, releseBtnDel);
        m_actionButton[(int)type].SetDeration(duration);
    }
    public void OnActionPressBtn(ActionButtonType type)
    {
        m_actionButton[(int)type].OnPressActionBtn();
    }
    public void OnActionReleseBtn(ActionButtonType type)
    {
        m_actionButton[(int)type].OnReleseActionBtn();
    }
}
