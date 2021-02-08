using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMove : MonoBehaviour
{
    public Vector3 m_from;
    public Vector3 m_to;

    private float m_time;
    public float m_duration;

    private float m_stunTime;
    public float m_stunDuration;   
    [SerializeField]
    AnimationCurve m_animCurve;

    private MonsterController m_monster;

    private IEnumerator Coroutine_StartMove()
    {
        while(true)
        {
            m_time += Time.deltaTime;
            var result = Vector3.Lerp(m_from, m_to, m_animCurve.Evaluate(m_time));
            //var target = result - transform.position;
            //m_monster.m_nvAgent.Move(target);
            transform.position = result;
            if(m_time >= m_duration)
            {
                //m_time = 0f;
                StartCoroutine(Coroutine_EndMove());
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator Coroutine_EndMove()
    {
        while(true)
        {
            m_stunTime += Time.deltaTime;
            if(m_stunTime >= m_stunDuration)
            {
                m_monster.SetIdel(5f);
                m_stunTime = 0f;
                yield break;
            }
            yield return null;
        }
    }
    public void StopTweenMove()
    {
        StopAllCoroutines();
    }
    public void StartMove(MonsterController monster)
    {
        m_monster = monster;
        m_stunTime = 0f;
        m_time = 0f;
        //StopAllCoroutines();
        m_animCurve = AnimationCurve.Linear(0f, 0f, m_duration, m_duration);
        StartCoroutine(Coroutine_StartMove());
    }   
}
