using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterAnimState
{
    None = -1,
    Idel,
    Run,
    Attack1,   
    Knockdown,
    Stun,
    Hit,
    Die,
    Max
}
public class MonsterAnimController : AnimController
{
    private MonsterAnimState m_animState = MonsterAnimState.Idel;

    public MonsterAnimState CurrentAnimState { get { return m_animState; } }
    
    public void Play(MonsterAnimState animState,bool isBlend=true)
    {
        m_animState = animState;
        Play(m_animState.ToString(), isBlend);
    }
}
