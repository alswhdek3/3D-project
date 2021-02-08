using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum MonsterState
{
    None = -1,
    Idel,
    Trace,
    Attack,
    Patrol,
    Knock,
    Stun,
    Hit,
    Die,
    Max = -1
}
public class MonsterController : MonoBehaviour
{
    protected MonsterType m_type;
    protected MonsterState m_state = MonsterState.Idel;
    protected MonsterAnimController m_animCtr;
    private TweenMove m_tween;
    protected NavMeshAgent m_nvAgent;
    protected CapsuleCollider m_collider;
    protected PlayerController m_player;
    [SerializeField]
    private MonsterAttackArea m_attackArea;

    //WayPoint   
    protected WayPointController m_wayPointCtr;
    protected HUDController m_hudCtr;

    private int m_wayPointIndex;
    private float m_wayPointMaxDist = 0.5f;

    protected float m_idelTime;
    protected float m_duration = 5f;
    protected float m_attackDist; // // 공격 사정거리
    protected float m_attackDistPow;
    protected float m_traceDist; //추적 사정거리
   
    public Transform m_dummyHud;

    private bool m_isAilve;
    public MonsterState State { get { return m_state; }set { m_state = value; } }
    public Status m_status;
    public bool IsAlive { get { return m_isAilve; }set { m_isAilve = value; } }
    public Status Status { get { return m_status; } }
    void Start()
    {
        m_traceDist = 5f;
        m_attackDist = 1.2f;
        InitMonster();        
    }    
    protected void InitMonster()
    {       
        m_animCtr = GetComponent<MonsterAnimController>();
        m_collider = GetComponent<CapsuleCollider>();
        m_nvAgent = GetComponent<NavMeshAgent>();
        m_tween = GetComponent<TweenMove>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_dummyHud = Util.FindChildrenObject(gameObject, "Dummy_HUD");
        m_isAilve = true;
        m_attackDistPow = Mathf.Pow(m_attackDist, 2f);
    }
    #region 동작 패턴을 변경
    public void SetIdel(float value)
    {
        if (m_animCtr.CurrentAnimState != MonsterAnimState.Idel)
        {
            m_nvAgent.isStopped = false;
            m_idelTime = m_duration - value;
            Debug.LogError(m_idelTime);
            m_animCtr.Play(MonsterAnimState.Idel);
            State = MonsterState.Idel;                     
        }
    }
    public void SetKnock()
    {
        if(m_animCtr.CurrentAnimState != MonsterAnimState.Knockdown)
        {
            m_animCtr.Play(MonsterAnimState.Knockdown);
            State = MonsterState.Knock;
        }
    }
    #endregion

    protected bool GetTargetSearch()
    {
        RaycastHit hit;
        var me = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
        var target = new Vector3(m_player.transform.position.x, me.y, m_player.transform.position.z);        
        var dist = target - me;
        if(Physics.Raycast(me,dist.normalized,m_traceDist, 1 << LayerMask.NameToLayer("Player")))
        {
            return true;
        }
        return false;
    }
    protected virtual void AnimEvent_Attack()
    {
        if (m_player.IsAlive) return;
        var target = m_attackArea.IsTarget;
        float damage = Random.Range(m_status.m_attck, m_status.m_attck + 11);
        if (target)
        {
            m_player.SetDamage((int)damage, this);
            var effectName = TableEffect.Instance.m_dicData[m_status.m_effectId].Prefab[0];
            var effect = EffectPool.Instance.Create(effectName);
            effect.transform.position = transform.position;
            var distance = effect.transform.position - m_player.m_dummyHit.position;
            distance.y = 0f;
            effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, distance.normalized);
        }
    }
    public void SetMonster(MonsterType type,Vector3 pos,WayPointController wayPointCtr,HUDController hudCtr)
    {
        m_type = type;
        m_hudCtr = hudCtr;
        m_status = new Status(200 * ((int)m_type + 1),2, 10 * ((int)m_type + 1), (int)m_type + 1, 0, 0);                     
        transform.position = pos;
        m_wayPointCtr = wayPointCtr;        
    }
    public void SetDamage(DamageType damageType,int damage,SkillData skillData)
    {       
        if (m_status.m_hp > 0)
        {
            if (skillData.m_knock > 0 && State != MonsterState.Knock && m_isAilve && State != MonsterState.Die)
            {
                SetKnock(skillData.m_knock, skillData.m_stun);
                State = MonsterState.Knock;
                m_animCtr.Play(MonsterAnimState.Knockdown);
            }
            if(skillData.m_knock <= 0 && State != MonsterState.Knock && m_isAilve && State != MonsterState.Die)
            {
                m_animCtr.Play(MonsterAnimState.Hit);
                var distance = m_player.transform.position - transform.position;
                transform.forward = distance.normalized;
                if (IsInvoking("RestartIdel"))
                {
                    CancelInvoke("RestartIdel");
                }
                Invoke("RestartIdel", m_animCtr.ClipLength(MonsterAnimState.Hit.ToString()));
            }
            m_status.m_hp -= damage;
            Debug.LogError("Hp : " + m_status.m_hp);
             m_hudCtr.SetDamage(damageType, damage);
            m_hudCtr.SetHp(m_status.m_hp);
            if(m_status.m_hp <= 0)
            {
                SetDie();
            }
        }      
    }
    private void SetDie()
    {      
        m_isAilve = false;
        m_player.RemoveMonsterFromArea(this);
        m_hudCtr.gameObject.SetActive(false);
        Invoke("RemoveMonster", 2f);
        m_animCtr.Play(MonsterAnimState.Die);
        State = MonsterState.Die;

        MonsterManager.Instance.AddHudPool(m_hudCtr);
    }
    private void RestartIdel()
    {
        if (State == MonsterState.Die) return;
        SetIdel(2f);
    }
    private void SetKnock(float value,float stunDuation)
    {
        var distance = transform.position - m_player.transform.position;
        distance.y = 0f;
        m_tween.m_from = transform.position;
        m_tween.m_to = m_tween.m_from + distance.normalized * value;
        m_tween.m_duration = 0.5f * (value / 2f);
        m_tween.m_stunDuration = stunDuation;
        m_tween.StartMove(this);
    }
    private void RemoveMonster()
    {
        gameObject.SetActive(false);
        MonsterManager.Instance.AddPoolUnitMonster(m_type, this);
    }
    private void AnimEvent_EndAttack()
    {
        m_nvAgent.ResetPath();
        m_nvAgent.isStopped = true;
        m_idelTime = m_duration - 2f;
        m_animCtr.Play(MonsterAnimState.Idel);
       
    }
    #region 몬스터 패턴
    protected virtual void MonsterPattn()
    {
        var me = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
        var player = new Vector3(m_player.transform.position.x, me.y, m_player.transform.position.z);
        var distance = player - me;

        switch (m_state)
        {
            case MonsterState.Idel:             
                m_idelTime += Time.deltaTime;               
                var target = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
                var dist = target - transform.position; //플레이어와 몬스터의 사이의 거리를 구한다.
                if (m_idelTime >= m_duration && !m_player.IsAlive)
                {
                    if(GetTargetSearch() && State != MonsterState.Die) //레이저로 플레이어를 검출이되었으면
                    {                                              
                        if(Mathf.Approximately(dist.sqrMagnitude, m_attackDistPow) || dist.sqrMagnitude < m_attackDistPow)//공격 사정거리에 있으면
                        {
                            m_nvAgent.stoppingDistance = m_attackDist;
                            m_nvAgent.isStopped = true;
                            m_nvAgent.ResetPath();
                            transform.forward = dist.normalized;
                            if (m_animCtr.CurrentAnimState != MonsterAnimState.Hit)
                            {
                                if(m_animCtr.CurrentAnimState != MonsterAnimState.Attack1)
                                    m_animCtr.Play(MonsterAnimState.Attack1);
                                //return;
                            }                           
                        }                        
                        else //공격 사정거리에는 없고 추적사정거리에는 있으면
                        {                           
                            State = MonsterState.Trace;
                            m_animCtr.Play(MonsterAnimState.Run);                                                   
                        }
                    }
                    else
                    {
                        if (State != MonsterState.Die)
                        {
                            m_nvAgent.isStopped = false;
                            State = MonsterState.Patrol;
                            m_animCtr.Play(MonsterAnimState.Run);
                            m_nvAgent.stoppingDistance = m_wayPointMaxDist;
                        }
                    }                   
                }    
                break;
            case MonsterState.Trace:
                //var distnace = m_player.transform.position - transform.position;
                if(Mathf.Approximately(distance.sqrMagnitude,m_attackDistPow) || distance.sqrMagnitude < m_attackDistPow)
                {
                    State = MonsterState.Idel;
                    m_animCtr.Play(MonsterAnimState.Idel);
                    m_nvAgent.ResetPath();
                    m_nvAgent.stoppingDistance = m_attackDist;
                    m_nvAgent.isStopped = true;
                    m_idelTime = m_duration - 1f;                   
                }
                else
                {
                    m_nvAgent.isStopped = false;
                    m_nvAgent.SetDestination(m_player.transform.position);
                    if(!GetTargetSearch())
                    {
                        m_nvAgent.isStopped = true;
                        m_nvAgent.ResetPath();
                        m_idelTime = m_duration - 2f;
                        m_animCtr.Play(MonsterAnimState.Idel);
                        State = MonsterState.Idel;
                    }
                }
                break;
            case MonsterState.Attack:               
                break;
            case MonsterState.Patrol:
                m_nvAgent.SetDestination(m_wayPointCtr.WayPoint[m_wayPointIndex].transform.position);
                var wayPointDistance = transform.position - m_wayPointCtr.WayPoint[m_wayPointIndex].transform.position;
                wayPointDistance.y = 0f;
                var patrolDistPow = Mathf.Pow(m_wayPointMaxDist, 2f);
                if(Mathf.Approximately(wayPointDistance.sqrMagnitude,patrolDistPow) || wayPointDistance.sqrMagnitude < patrolDistPow)
                {
                    m_wayPointIndex++;
                    if (m_wayPointIndex > m_wayPointCtr.WayPoint.Length - 1)
                    {
                        m_wayPointIndex = 0;
                    }
                    m_nvAgent.ResetPath();
                    m_nvAgent.isStopped = true;
                    m_animCtr.Play(MonsterAnimState.Idel);
                    State = MonsterState.Idel;
                    m_idelTime = m_duration - 3f;                                                                        
                }
                if(GetTargetSearch())
                {
                    m_nvAgent.isStopped = true;
                    m_nvAgent.ResetPath();
                    m_idelTime = m_duration - 1f;
                    m_animCtr.Play(MonsterAnimState.Idel);
                    State = MonsterState.Idel;
                }
                break;
            case MonsterState.Knock:
                m_nvAgent.ResetPath();
                m_nvAgent.isStopped = true;               
                break;
            case MonsterState.Stun:
                break;
            case MonsterState.Hit:
                m_nvAgent.ResetPath();
                m_nvAgent.isStopped = true;
                break;
            case MonsterState.Die:                
                //m_collider.enabled = false;
                m_nvAgent.ResetPath();
                m_nvAgent.isStopped = true;                               
                break;
        }
        if(GetTargetSearch())
        {
            Debug.DrawRay(me, distance.normalized * m_traceDist, Color.red);
        }
        else
        {
            Debug.DrawRay(me, distance.normalized * 1000f, Color.yellow);
        }
    }
    #endregion
   
    void Update()
    {
        MonsterPattn();
        if(!m_isAilve)
        {
            m_tween.StopTweenMove();
        }
    }
}
