using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    None=-1,
    Nomal,   
    Weapons,
    Max
}
public class MonsterManager : SingtonMonoBehaviour<MonsterManager>
{
    private GameObjectPool<MonsterController> m_monsterPool;   
    private GameObjectPool<HUDController> m_hudPool;

    [SerializeField]
    private GameObject m_hudPoolObj;

    private Dictionary<MonsterType, List<MonsterController>> m_dicMonsterPool = new Dictionary<MonsterType, List<MonsterController>>();   
    private Dictionary<MonsterType, GameObject> m_dicMonsterPrefab = new Dictionary<MonsterType, GameObject>();

    [SerializeField]
    private GameObject m_monsterPrefab;
    [SerializeField]
    private GameObject m_hudPrefab;
    public GameObject m_dummyObj;

    private IEnumerator Coroutine_SetActive(GameObject obj, bool isOn)
    {
        yield return new WaitForEndOfFrame();
        obj.gameObject.SetActive(isOn);
    }
    public MonsterController CreateMonster(MonsterType type, Vector3 pos, WayPointController wayPointCtr)
    {
        MonsterController monster = null;
        var hud = m_hudPool.Get();
        var monPool = m_dicMonsterPool[type];
        if(monPool.Count > 0)
        {
            monster = monPool[0];
            monPool.Remove(monster);
            StartCoroutine(Coroutine_SetActive(monster.gameObject, true));
            monster.SetMonster(type, pos, wayPointCtr, hud);          
            hud.SetHud(monster.m_dummyHud, string.Format("기사_{0:00}", (int)type + 1), monster.m_status.m_hp);
            hud.gameObject.SetActive(true);
            return monster;
        }
        var prefab = m_dicMonsterPrefab[type];
        monster = CreateMonsterPoolUnit(type, prefab);
        monster.transform.SetParent(m_dummyObj.transform);
        monster.transform.localPosition = Vector3.zero;
        monster.transform.localScale = Vector3.one;
        StartCoroutine(Coroutine_SetActive(monster.gameObject, true));
        monster.SetMonster(type, pos, wayPointCtr, hud);      
        hud.SetHud(monster.m_dummyHud, string.Format("기사_{0:00}", (int)type + 1), monster.m_status.m_hp);
        hud.gameObject.SetActive(true);
        return monster;
    }
    private MonsterController CreateMonsterPoolUnit(MonsterType type, GameObject prefab)
    {       
        var obj = Instantiate(prefab);
        var monster = obj.GetComponent<MonsterController>();
        if (monster == null)
        {
            monster = obj.GetComponent<MonsterController>();
        }       
        return monster;
    }  
    public void AddPoolUnitMonster(MonsterType type, MonsterController monster)
    {
        if (!m_dicMonsterPool.ContainsKey(type)) return;
        //monster.gameObject.SetActive(false);
        m_dicMonsterPool[type].Add(monster);
    }
    public void AddHudPool(HUDController hud)
    {
        //hud.gameObject.SetActive(false);
        m_hudPool.Set(hud);
    }
    protected override void OnStart()
    {
        //List<WeaponsController> weaponsList = new List<WeaponsController>();
        for (int i = 0; i < (int)MonsterType.Max; i++)
        {
            m_monsterPrefab = Resources.Load(string.Format("Prefab/Monster/Monster_{0:00}", i + 1)) as GameObject;
            List<MonsterController> monList = new List<MonsterController>();
            monList.Clear();
            m_monsterPool = new GameObjectPool<MonsterController>(4, () =>
            {
                var obj = Instantiate(m_monsterPrefab);
                StartCoroutine(Coroutine_SetActive(obj, false));
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                var monster = obj.GetComponent<MonsterController>();
                monList.Add(monster);
                return monster;   
            });
            if (!m_dicMonsterPool.ContainsKey((MonsterType)i))
                m_dicMonsterPool.Add((MonsterType)i, monList);

            var prefab = Resources.Load(string.Format("Prefab/Monster/Monster_{0:00}", i + 1)) as GameObject;
            StartCoroutine(Coroutine_SetActive(prefab, false));                                  
            if (!m_dicMonsterPrefab.ContainsKey((MonsterType)i))
                    m_dicMonsterPrefab.Add((MonsterType)i, prefab);
        }
        m_hudPool = new GameObjectPool<HUDController>(4, () =>
        {
            var obj = Instantiate(m_hudPrefab);
            //StartCoroutine(Coroutine_SetActive(obj, false));
            obj.transform.SetParent(m_hudPoolObj.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            var hud = obj.GetComponent<HUDController>();
            hud.gameObject.SetActive(false);
            return hud;
        });
    }
}