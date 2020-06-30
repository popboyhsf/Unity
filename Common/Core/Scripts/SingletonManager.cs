using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//namespace Core
//{
    public class SingletonManager<T> : SingletonMonoBehaviour<T> where T : Component
    {
        override protected void Init()
        {
            base.Init();
            GameObject controller = GameObject.FindGameObjectWithTag("GameController");
            if (controller == null)
            {
                controller = new GameObject("GameController")
                {
                    tag = "GameController"
                };
            }
            Transform.ResetParent(controller.transform);
        }
    }
//}
