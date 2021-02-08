using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Status
{
    public int m_hp;
    public int m_effectId; //이펙트 
    public int m_attck;    
    public int m_defence;   
    public float m_knock; //넉백
    public float m_stun; //스턴

    public Status(int hp,int effectId, int attack,int defence,float knock,float stun)
    {
        m_hp = hp;
        m_effectId = effectId;
        m_attck = attack;      
        m_defence = defence;              
        m_knock = knock;
        m_stun = stun;        
    }
}