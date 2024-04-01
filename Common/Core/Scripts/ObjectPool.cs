using System.Collections.Generic;
using UnityEngine.Events;


public class ObjectPool<T> where T : new()
{
    private readonly LinkedList<T> m_ActiveList = new LinkedList<T>();
    private readonly Stack<T> m_InactiveStack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    public int CountAll { get; private set; }
    public int CountActive { get { return CountAll - CountInactive; } }
    public int CountInactive { get { return m_InactiveStack.Count; } }

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
        if (m_InactiveStack.Count == 0)
        {
            element = Create();
            CountAll++;
        }
        else
        {
            element = m_InactiveStack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        m_ActiveList.AddLast(element);
        return element;
    }

    public virtual void Release(T element)
    {
        if (!m_ActiveList.Contains(element))
            Dbg.LogError($"Internal error. Trying to destroy {element} that is not in m_ActiveList.");

        m_ActiveList.Remove(element);

        if (m_InactiveStack.Count > 0 && ReferenceEquals(m_InactiveStack.Peek(), element))
            Dbg.LogError($"Internal error. Trying to destroy {element} that is already released to pool.");

        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_InactiveStack.Push(element);
    }

    public void ReleaseAll()
    {
        while (m_ActiveList.Count > 0)
        {
            Release(m_ActiveList.First.Value);
        }
    }
}
