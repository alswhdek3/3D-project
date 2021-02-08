using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    private Animator m_animtor;
    private Dictionary<string, float> m_dicLength = new Dictionary<string, float>();

    public Animator AnimationStateInfo { get { return m_animtor; } }
    public float ClipLength(string animName)
    {
        return m_dicLength[animName];
    }
    public void Play(string animName,bool isBlend=true)
    {
        if(isBlend)
        {
            m_animtor.SetTrigger(animName);
        }
        else
        {
            m_animtor.Play(animName);
        }
    }
    void Start()
    {
        m_animtor = GetComponent<Animator>();                
        var animClip = m_animtor.runtimeAnimatorController;
        for(int i=0; i<animClip.animationClips.Length; i++)
        {
            m_dicLength.Add(animClip.animationClips[i].name, animClip.animationClips[i].length);
        }
    }
}
