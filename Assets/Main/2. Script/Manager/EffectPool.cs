using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : SingtonMonoBehaviour<EffectPool>
{
    private List<string> m_effectNameList = new List<string>();
    private Dictionary<string, List<EffectPoolUnit>> m_dicEffectPool = new Dictionary<string, List<EffectPoolUnit>>();
    private Dictionary<string, GameObject> m_dicEffectPrefab = new Dictionary<string, GameObject>();
    private GameObjectPool<EffectPoolUnit> m_effectPool;
    
    private IEnumerator Coroutine_SetActive(GameObject obj,bool isOn)
    {
        yield return new WaitForEndOfFrame();
        obj.gameObject.SetActive(isOn);
    }
    public EffectPoolUnit Create(string effectName)
    {
        EffectPoolUnit poolUnit = null;
        var effectPool = m_dicEffectPool[effectName];
        if(effectPool.Count > 0)
        {
            if(effectPool[0] != null && effectPool[0].IsReady)
            {
                poolUnit = effectPool[0];
                effectPool.Remove(poolUnit);
                poolUnit.SetObjectPool(effectName, this);               
                StartCoroutine(Coroutine_SetActive(poolUnit.gameObject, true));
                return poolUnit;
            }           
        }
        var prefab = m_dicEffectPrefab[effectName];
        poolUnit = CreateEffectPoolUnit(effectName, prefab);
        StartCoroutine(Coroutine_SetActive(poolUnit.gameObject, true));
        return poolUnit;
    }
    private EffectPoolUnit CreateEffectPoolUnit(string effectName,GameObject prefab)
    {
        var obj = Instantiate(prefab);
        var poolUnit = obj.GetComponent<EffectPoolUnit>();
        var destroyUnit = obj.GetComponent<ParticleAutoDestory>();        
        if (poolUnit == null)
        {
            poolUnit = obj.AddComponent<EffectPoolUnit>();
        }
        if(destroyUnit == null)
        {
            destroyUnit = obj.AddComponent<ParticleAutoDestory>();
        }
            
        poolUnit.SetObjectPool(effectName, this);
        return poolUnit;
    }
    public void AddPoolUnit(string effectName,EffectPoolUnit poolUnit)
    {
        var effectPool = m_dicEffectPool[effectName];
        if(effectPool != null)
        {
            effectPool.Add(poolUnit);
        }
    }
    public void Load()
    {
        TableEffect.Instance.Load();
        
        foreach(KeyValuePair<int,TableEffect.Data> pair in TableEffect.Instance.m_dicData)
        {
            for(int i=0; i<pair.Value.Prefab.Length; i++)
            {
                if(!m_effectNameList.Contains(pair.Value.Prefab[i]))
                {
                    m_effectNameList.Add(pair.Value.Prefab[i]);
                }
            }
        }
        for(int i=0; i<m_effectNameList.Count; i++)
        {
            List<EffectPoolUnit> effectList = new List<EffectPoolUnit>();
            effectList.Clear();
            m_effectPool = new GameObjectPool<EffectPoolUnit>(1, () =>
            {
                 var prefab = Resources.Load("Prefab/Effect/" + m_effectNameList[i]) as GameObject;
                 StartCoroutine(Coroutine_SetActive(prefab, false));
                 var poolUnit = CreateEffectPoolUnit(m_effectNameList[i], prefab);              
                 effectList.Add(poolUnit);
                 return poolUnit;
            });
            m_dicEffectPool.Add(m_effectNameList[i], effectList);

            //GameObejct Prefab 생성
            var obj = Resources.Load("Prefab/Effect/" + m_effectNameList[i]) as GameObject;            
            StartCoroutine(Coroutine_SetActive(obj, false));
            m_dicEffectPrefab.Add(m_effectNameList[i], obj);
        }
    }
    protected override void OnStart()
    {
        Load();
    }
}
