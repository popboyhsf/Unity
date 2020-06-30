using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> where T : new()
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    public int CountAll { get; private set; }
    public int CountActive { get { return CountAll - CountInactive; } }
    public int CountInactive { get { return m_Stack.Count; } }

    public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    protected virtual T Create()
    {
        return new T();
    }

    public virtual T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = Create();
            CountAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public virtual void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debuger.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}
