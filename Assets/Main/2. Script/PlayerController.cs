using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    private Vector3 m_dir;
    [SerializeField]
    private float m_speed;
    private CharacterController m_charCtr;
    private NavMeshAgent m_nvAgent;
    private PlayerAnimController m_animCtr;
    private List<PlayerAnimState> m_comboList = new List<PlayerAnimState>()
    {
        PlayerAnimState.Attack1,PlayerAnimState.Attack2,PlayerAnimState.Attack3,PlayerAnimState.Attack4
    };
    private Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>();
    private List<int> m_dirList = new List<int>();
    private int m_comboIndex;
    private bool m_isCombo = false;
    //데미지 관련 변수
    [SerializeField]
    private GameObject m_attackAreaObj;
    private AttackArea[] m_attackArea;
    [SerializeField]
    private GameObject m_hitEffectPrefab;

    private Dictionary<PlayerAnimState, SkillData> m_skillTable = new Dictionary<PlayerAnimState, SkillData>();
    
    private MonsterController m_target = null;
    private Vector3 m_prevDist = Vector3.zero;

    public Transform m_dummyHit;
    public Transform m_firePos;

    public PlayerData m_playerData = new PlayerData();
    private MonsterController m_monster;

    private bool m_isDie = false;   
    private bool IsAttack { get { return m_animCtr.CurrentAnimState >= PlayerAnimState.Attack1 && m_animCtr.CurrentAnimState <= PlayerAnimState.Attack4; } }
    public bool IsAlive { get { return m_isDie;} }
    private void Move()
    {
        if (!MovePadController.Instance.IsDrag)
            m_dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        else
        {
            m_dir = TouchMove();
            m_dir.y = 0f;
        }            
        var animStateInfo = m_animCtr.AnimationStateInfo;
        var animState = animStateInfo.GetCurrentAnimatorStateInfo(0);
        if (m_nvAgent.enabled == true && !m_isDie && !animState.IsName("Hit") && !m_isCombo)
                m_nvAgent.Move(m_dir.normalized * m_speed * Time.deltaTime);
        else
            m_charCtr.Move(m_dir.normalized * m_speed * Time.deltaTime);
        if (m_dir != Vector3.zero)
        {
            ResetCombo();
            transform.forward = m_dir;
            if (m_animCtr.CurrentAnimState == PlayerAnimState.Idel)
            {
                m_animCtr.Play(PlayerAnimState.Walk);               
            }
        }        
        else
        {           
            if (m_animCtr.CurrentAnimState == PlayerAnimState.Walk)
            {
                m_animCtr.Play(PlayerAnimState.Idel);                          
            }                       
            if (animState.IsName(PlayerAnimState.Walk.ToString()))
                    m_animCtr.Play(PlayerAnimState.Idel);
        }
        if(m_dir != Vector3.zero)
        {
            if(IsAttack)
            {
                m_dir = Vector3.zero;
                m_nvAgent.enabled = false;
            }
            else
            {
                m_nvAgent.enabled = true;
            }
        }
    }

    private Vector3 TouchMove()
    {
        Vector3 dir = MovePadController.Instance.GetPos();
        if(dir.x < 0f)
        {
            dir += Vector3.right * dir.x;
        }
        if(dir.x > 0f)
        {
            dir += Vector3.right * dir.x;
        }
        if(dir.y > 0f)
        {
            dir += Vector3.forward * dir.y;
        }
        if (dir.y < 0f)
        {
            dir += Vector3.forward * dir.y;
        }       
        return dir;
    }

    #region AnimationEvent
    private IEnumerator Coroutine_AnimFinised(bool comboCheck)
    {        
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
       
        if (comboCheck) //콤보가 끝나지않았으면 다음콤보 진행
        {            
            m_animCtr.Play(m_comboList[m_comboIndex]);          
        }
        else //콤보가 종료되었으면 대기,걷기 애니메이션 전환
        {
            m_isCombo = false;
            m_comboIndex = 0;
            if (m_dir != Vector3.zero)
            {
                if(m_animCtr.CurrentAnimState != PlayerAnimState.Walk)
                {
                    m_animCtr.Play(PlayerAnimState.Walk);
                }
            }
            else
            {
                if(m_animCtr.CurrentAnimState != PlayerAnimState.Idel)
                {
                    m_animCtr.Play(PlayerAnimState.Idel);                   
                }    
            }
        }
    }
    private void AnimEvent_AttackCheckFinished() //콤보 공격이 끝났는지 체크
    {
        bool comboCheck = false;
        if(m_keyBuffer.Count == 0 || m_keyBuffer.Count > 1)
        {
            comboCheck = false;
            m_isCombo = false;
            m_keyBuffer.Clear();
        }
        if(m_keyBuffer.Count == 1)
        {
            comboCheck = true;
            m_isCombo = true;
            m_comboIndex++;
            if(m_comboIndex > m_comboList.Count -1)
            {
                m_comboIndex = 0;
            }
        }
        StartCoroutine(Coroutine_AnimFinised(comboCheck));
    }
    private void AnimEvent_SetAttack()
    {
        SkillData skillData = null;
        DamageType type = DamageType.None;
        int damage = 0;
        if (!m_skillTable.TryGetValue(m_animCtr.CurrentAnimState, out skillData)) return;
        var unitList = m_attackArea[skillData.m_attackArea].UnitList;       
        for (int i = 0; i < unitList.Count; i++)
        {
            var dummy = Util.FindChildrenObject(unitList[i], "Dummy_Hit");
            if (dummy != null)
            {                
                var mon = unitList[i].GetComponent<MonsterController>();              
                type = GetDamageType(out damage, skillData, mon); // out 형태로 가져온다.               
                var distnace = mon.transform.position - transform.position;
                /*if (Mathf.Abs(distnace.sqrMagnitude) <= m_prevDist.sqrMagnitude || m_prevDist == Vector3.zero && mon.IsAlive && mon.gameObject.activeSelf)
                {
                    m_prevDist = distnace.normalized;
                }*/
                transform.forward = distnace.normalized;
                mon.SetDamage(type, damage, skillData);
                var effectName = TableEffect.Instance.m_dicData[skillData.m_skillEffectId].Prefab[0];
                var effect = EffectPool.Instance.Create(effectName);
                effect.transform.position = dummy.position;
                var distance = transform.position - effect.transform.position;
                distance.y = 0f;
                effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, distance.normalized);
                if (!mon.IsAlive)
                    return;
            }            
        }        
    }
    private void AnimEvent_DistanceAttack() //원거리 공격 스킬
    {
        SkillData skillData;
        if (!m_skillTable.TryGetValue(m_animCtr.CurrentAnimState, out skillData)) return;

        var unitList = m_attackArea[skillData.m_attackArea].UnitList;
        for(int i=0; i<unitList.Count; i++)
        {
            var dummy = Util.FindChildrenObject(unitList[i], "Dummy_Hit");
            if(dummy != null)
            {
                var monster = unitList[i].GetComponent<MonsterController>();
                if(monster.IsAlive)
                {
                    var effectName = TableEffect.Instance.m_dicData[skillData.m_skillEffectId].Prefab[0];
                    var effect = EffectPool.Instance.Create(effectName);
                    effect.transform.position = m_firePos.position;
                    var distance = monster.transform.position + Vector3.up * 1.5f - effect.transform.position;
                    effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, distance.normalized);
                    var rigidBody = effect.GetComponent<Rigidbody>();
                    rigidBody.velocity = distance.normalized * 10f;
                    var collider = effect.GetComponent<ParticleCollider>();
                    collider.SetParticle(skillData.m_attack, monster);
                }
            }           
        }      
    }
    #endregion

    private DamageType GetDamageType(out int damage,SkillData skilldata,MonsterController monster)
    {
        DamageType type = DamageType.None;
        if(CarulatorDamage.AttackSuccess(skilldata.m_attackRate))
        {
            type = DamageType.Nomal;
            damage = CarulatorDamage.NomalDamage(skilldata.m_attack, monster.Status.m_defence);
            if(CarulatorDamage.CriticalSuccess(skilldata.m_criticalRate))
            {
                type = DamageType.Critical;
                damage = CarulatorDamage.criticalDamage(skilldata.m_attack, skilldata.m_criticalAttack);
            }
        }
        else
        {
            type = DamageType.Miss;
            damage = 0;
        }        
        return type;
    }
    #region ActionButtonSkill
    private void ActionButton_ComboDown()
    {
        m_dir = Vector3.zero;
        if (m_animCtr.CurrentAnimState == PlayerAnimState.Idel || m_animCtr.CurrentAnimState == PlayerAnimState.Walk)
        {
            m_animCtr.Play(m_comboList[m_comboIndex]);
        }
        else
        {
            m_isCombo = true;
            if (IsInvoking("ResetCombo"))
            {
                CancelInvoke("ResetCombo");
            }
            m_keyBuffer.Enqueue(KeyCode.Space);
            Invoke("ResetCombo", m_animCtr.ClipLength(m_animCtr.CurrentAnimState.ToString()) / 2f);
        }
        if (m_dir != Vector3.zero)
        {
            if (m_isCombo)
            {
                if (m_animCtr.CurrentAnimState == PlayerAnimState.Walk)
                    m_animCtr.Play(PlayerAnimState.Idel);
                m_dir = Vector3.zero;
                m_animCtr.Play(m_comboList[m_comboIndex]);
            }
        }
    }  
    private void ActionButton_ComboUp()
    {
        m_isCombo = false;
    }
    private void ActionButton_Knock()
    {
        if(m_animCtr.CurrentAnimState != PlayerAnimState.Knock)
        {
            m_dir = Vector3.zero;
            m_animCtr.Play(PlayerAnimState.Knock);
        }
    }
    private void ActionButton_Fire()
    {
        if (m_animCtr.CurrentAnimState != PlayerAnimState.Fire)
        {
            m_dir = Vector3.zero;
            m_animCtr.Play(PlayerAnimState.Fire);          
        }
    }
    private void ActionButton_UItimate()
    {
        if (m_animCtr.CurrentAnimState != PlayerAnimState.Ultimate)
        {
            m_dir = Vector3.zero;
            m_animCtr.Play(PlayerAnimState.Ultimate);
        }
    }
    #endregion

    #region 콤보
    private void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {           
            m_dir = Vector3.zero;
            if (m_animCtr.CurrentAnimState == PlayerAnimState.Idel || m_animCtr.CurrentAnimState == PlayerAnimState.Walk)
            {                
                m_animCtr.Play(m_comboList[m_comboIndex]);                
            }
            else
            {               
                m_isCombo = true;
                if(IsInvoking("ResetCombo"))
                {
                    CancelInvoke("ResetCombo");
                }
                m_keyBuffer.Enqueue(KeyCode.Space);
                Invoke("ResetCombo", m_animCtr.ClipLength(m_animCtr.CurrentAnimState.ToString()) / 2f);
            }
        }
        if(m_dir != Vector3.zero)
        {
            if(m_isCombo)
            {
                if (m_animCtr.CurrentAnimState == PlayerAnimState.Walk)
                        m_animCtr.Play(PlayerAnimState.Idel);
                m_dir = Vector3.zero;
                m_animCtr.Play(m_comboList[m_comboIndex]);    
            }
        }          
    }
    private void ResetCombo()
    {
        m_isCombo = false;
        m_keyBuffer.Clear();
    }
    #endregion

    #region 피격

    #region 충돌영역에서 몬스터 지우기
    public void RemoveMonsterFromArea(MonsterController monster)
    {
        for(int i=0; i<m_attackArea.Length; i++)
        {
            m_attackArea[i].RemoveMonster(monster.gameObject);
        }
    }
    #endregion

    public void SetDamage(int damage,MonsterController monster)
    {
        m_monster = monster;
        int realDamage = damage - m_playerData.m_heroData.m_defence;
        if (realDamage <= 0) return; //미스 데미지텍스트 출력
        if (!m_isDie && m_playerData.m_heroData.m_hp > 0 && m_animCtr.CurrentAnimState != PlayerAnimState.Hit)
        {
            m_playerData.m_heroData.m_hp -= realDamage;
            Debug.LogError("Hp : " + m_playerData.m_heroData.m_hp);
            m_animCtr.Play(PlayerAnimState.Hit, false);
            Invoke("RestartIdel", m_animCtr.ClipLength(PlayerAnimState.Hit.ToString()));
            if (m_playerData.m_heroData.m_hp <= 0)
            {
                if (IsInvoking("RestartIdel")) CancelInvoke("RestartIdel");
                m_isDie = true;
                m_animCtr.Play(PlayerAnimState.Die);
                monster.SetIdel(5f);
            }
        }   
    }
    #endregion
    private void RestartIdel()
    {
        m_animCtr.Play(PlayerAnimState.Idel);
    }
    private void InitSkillData()
    {
        m_playerData.m_heroData = new PlayerData.HeroData() { m_hp = 2000, m_mp = 0, m_defence = 5 };

        m_skillTable.Add(PlayerAnimState.Attack1, new SkillData() { m_attack = Random.Range(10, 20), m_attackArea = 0,m_skillEffectId = 1, m_attackRate = 70f, m_criticalRate = 50f, m_criticalAttack = Random.Range(50, 100), m_defence = 2, m_knock = 0f, m_stun = 0 });
        m_skillTable.Add(PlayerAnimState.Attack2, new SkillData() { m_attack = Random.Range(20, 30), m_attackArea = 0, m_skillEffectId = 1, m_attackRate = 80f, m_criticalRate = 50f, m_criticalAttack = Random.Range(50, 100), m_defence = 2, m_knock = 0f, m_stun = 0 });
        m_skillTable.Add(PlayerAnimState.Attack3, new SkillData() { m_attack = Random.Range(35, 40), m_attackArea = 0, m_skillEffectId = 1, m_attackRate = 90f, m_criticalRate = 50f, m_criticalAttack = Random.Range(50, 100), m_defence = 2, m_knock = 0f, m_stun = 0 });
        m_skillTable.Add(PlayerAnimState.Attack4, new SkillData() { m_attack = Random.Range(50, 70), m_attackArea = 0, m_skillEffectId = 15, m_attackRate = 100f, m_criticalRate = 50f, m_criticalAttack = Random.Range(50, 100), m_defence = 2, m_knock = 3f, m_stun = 5f });
        m_skillTable.Add(PlayerAnimState.Fire, new SkillData() { m_attack = Random.Range(100, 200), m_attackArea = 1, m_skillEffectId = 17, m_attackRate = 90f, m_criticalRate = 50f, m_criticalAttack = Random.Range(50, 100), m_defence = 2, m_knock = 0f, m_stun = 0 });

        ActionManager.Instance.SetButton(ActionButtonType.ComboAttack, 0f, ActionButton_ComboDown, ActionButton_ComboUp);    
        ActionManager.Instance.SetButton(ActionButtonType.Knock, 7f, ActionButton_Knock, null);
        ActionManager.Instance.SetButton(ActionButtonType.Fire, 2f, ActionButton_Fire, null);
        ActionManager.Instance.SetButton(ActionButtonType.UItimate, 20f, ActionButton_UItimate, null);
    }
    void Awake()
    {
        m_charCtr = GetComponent<CharacterController>();
        m_nvAgent = GetComponent<NavMeshAgent>();
        m_animCtr = GetComponent<PlayerAnimController>();
        TableEffect.Instance.Load();
    }
    private void Start()
    {
        InitSkillData();
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackArea>();
        m_hitEffectPrefab = Resources.Load("Prefab/Effect/FX_Attack01_01") as GameObject;
        m_dummyHit = Util.FindChildrenObject(gameObject, "Dummy_Hit");
        m_dirList.Add(0);
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
    }
}
