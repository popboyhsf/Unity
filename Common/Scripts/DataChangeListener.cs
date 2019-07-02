using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataChangeListener<T> where T : struct
{
    public delegate void CallChange(T o);
    public CallChange CallBackChange;

    private T m_o;

    public T Listener
    {
        set
        {
            if (m_o.Equals(value)) return;
            CallBackChange?.Invoke(value);
            m_o = value;

        }
    }
}
