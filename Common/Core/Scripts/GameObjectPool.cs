using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : ObjectPool<GameObject>
{
    public delegate GameObject CreateDelegate();
    CreateDelegate createAction;

    public GameObjectPool() : base(null, null)
    {

    }

    public void Init(CreateDelegate actionOnCreate)
    {
        createAction = actionOnCreate;
    }

    protected override GameObject Create()
    {
        return createAction?.Invoke();
    }
}
