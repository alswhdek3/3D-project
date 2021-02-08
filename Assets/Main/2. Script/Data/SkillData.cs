using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public int m_attack; //공격력
    public int m_attackArea; //공격 AttackArea Number
    public int m_skillEffectId; //액셀 데이터 ID
    public int m_defence; //방어력
    public float m_knock; //넉백되는 힘
    public float m_stun; //스턴 시간
    public float m_attackRate; //공격 성공률
    public float m_criticalRate; //크리티컬 성공률
    public int m_criticalAttack; //크리티컬 공격력
}
