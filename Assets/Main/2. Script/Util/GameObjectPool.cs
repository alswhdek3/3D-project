using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : class
{
    public delegate T CreateFuncDel();
    private CreateFuncDel m_createFunDel;
    private int m_count;
    private Queue<T> m_queueObject = new Queue<T>();

    public GameObjectPool(int count,CreateFuncDel createFuncDel)
    {
        m_count = count;
        m_createFunDel = createFuncDel;
        AllObecjtCreate();
    }
    private void AllObecjtCreate()
    {
        for(int i=0; i<m_count; i++)
        {
            var obj = m_createFunDel();
            m_queueObject.Enqueue(obj);
        }
    }
    public T Get()
    {
        if(m_queueObject.Count >0)
        {
            return m_queueObject.Dequeue();
        }
        else
        {
            var obj = m_createFunDel();
            return obj;
        }
    }
    public void Set(T obj)
    {
        m_queueObject.Enqueue(obj);
    }
}
