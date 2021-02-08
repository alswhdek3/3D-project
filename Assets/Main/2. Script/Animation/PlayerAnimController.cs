using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimState
{
    None = -1,
    Idel,
    Walk,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    Knock,
    Fire,
    Ultimate,
    Hit,
    Die,
    Max
}
public class PlayerAnimController : AnimController
{
    private PlayerAnimState m_state = PlayerAnimState.Idel;

    public PlayerAnimState CurrentAnimState { get { return m_state; } }
    public void Play(PlayerAnimState state,bool isBlend=true)
    {
        m_state = state;
        Play(m_state.ToString(), isBlend);
    }
}
