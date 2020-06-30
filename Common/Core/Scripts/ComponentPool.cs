using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComponentPool<T>: ObjectPool<T> where T:Component,new ()
{
    public delegate T CreateDelegate();
    CreateDelegate createAction;


    public ComponentPool() : base(null, null)
    {

    }


    public void Init(CreateDelegate actionOnCreate)
    {
        createAction = actionOnCreate;
    }

    protected override T Create()
    {
        return createAction?.Invoke();
    }
}
