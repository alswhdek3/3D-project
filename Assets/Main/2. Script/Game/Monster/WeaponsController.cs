using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsController : MonsterController
{
    [SerializeField]
    private Transform m_dummyWaponPos;  
    protected override void AnimEvent_Attack()
    {
        var target = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
        var my = transform.position;
        var distance = target - my;     
        var distDow = Mathf.Pow(m_attackDist, 2f);
        if(Mathf.Approximately(distance.sqrMagnitude,distDow) || distance.sqrMagnitude < distDow)
        {
            var effectName = TableEffect.Instance.m_dicData[19].Prefab[0];
            var effect = EffectPool.Instance.Create(effectName);
            var rigidBody = effect.GetComponent<Rigidbody>();
            var collider = effect.GetComponent<ParticleCollider>();
            int damage = Random.Range(m_status.m_attck, m_status.m_attck + 21);                 
            effect.transform.position = m_dummyWaponPos.position;
            var plyaer = new Vector3(m_player.transform.position.x, effect.transform.position.y, m_player.transform.position.z);
            var dist = plyaer - effect.transform.position;
            effect.transform.forward = dist.normalized;
            rigidBody.velocity = dist.normalized * 10f;
            collider.SetParticle(damage, this);
        }
    }
    void Start()
    {      
        m_idelTime = 0f;
        m_duration = 5f;
        m_traceDist = 8f;
        m_attackDist = 8f;
        InitMonster();                  
    }
    private void Update()
    {
        MonsterPattn();
    }
}
