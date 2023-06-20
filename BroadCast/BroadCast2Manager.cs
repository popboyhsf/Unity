using Mkey;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static BroadCastManager;

public class BroadCast2Manager : SingletonMonoBehaviour<BroadCast2Manager>
{

    [SerializeField]
    NormalInfoBox enterGameL, enterGameR;

    [SerializeField]
    NormalInfoBox clickLikeL, clickLikeR;


    [ShowInInspector, ReadOnly]
    private Dictionary<Show, Queue<QueueInfo>> broadMessageQ = new Dictionary<Show, Queue<QueueInfo>>();

    private Show broadStatus = 0x00000000;

    /// <summary>
    /// 加入游戏广播
    /// </summary>
    /// <param name="info"></param>
    /// <param name="faction"></param>
    public void EnterGameBroadMessage(PlayerInfoPack info, SoldierFaction faction)
    {
        QueueInfo _pack = new QueueInfo();
        switch (faction)
        {
            case SoldierFaction.left:
                _pack = new QueueInfo(enterGameL, info, faction, -999);
                ShowBroadMessage(Show.EnterGameL, _pack);
                break;
            case SoldierFaction.right:
                _pack = new QueueInfo(enterGameR, info, faction, -999);
                ShowBroadMessage(Show.EnterGameR, _pack);
                break;
        }
        
        
    }

    /// <summary>
    /// 连续点赞
    /// </summary>
    /// <param name="info"></param>
    /// <param name="faction"></param>
    public void ClickLikeBroadMessage(PlayerInfoPack info, SoldierFaction faction)
    {
        QueueInfo _pack = new QueueInfo();
        switch (faction)
        {
            case SoldierFaction.left:
                _pack = new QueueInfo(clickLikeL, info, faction, -999);
                ShowBroadMessage(Show.ClickLikeL, _pack);
                break;
            case SoldierFaction.right:
                _pack = new QueueInfo(clickLikeR, info, faction, -999);
                ShowBroadMessage(Show.ClickLikeR, _pack);
                break;
        }

        
    }

    private void ShowBroadMessage(Show key,QueueInfo info)
    {

        if (!broadMessageQ.ContainsKey(key))
        {
            broadMessageQ.Add(key, new Queue<QueueInfo>());
        }

        if (broadStatus.HasFlag(key))
        {
            if (!broadMessageQ[key].Contains(info))
            {
                broadMessageQ[key].Enqueue(info);
            }
        }
        else
        {
            broadStatus |= key;

            var _obj = Instantiate(info.broad, info.broad.transform.parent);
            _obj.gameObject.SetActive(true);
            _obj.Init(info.info, info.faction, info.count);
            var _delayTime = _obj.GetComponentInChildren<Animator>().GetAnimationClipLength("born");
            _obj.AddComponent<DelayDestroy>().Init(_delayTime, () => {

                broadStatus &= (~key);

                if (this.broadMessageQ[key].Count > 0)
                {
                    var _ms = this.broadMessageQ[key].Dequeue();
                    ShowBroadMessage(key, _ms);
                }

            });
        }
    }

    private void ClearAll()
    {
        broadStatus = 0x00000000;
        broadMessageQ.Clear();
    }


    [Flags]
    public enum Show
    {
        EnterGameL = 0x00000001,
        EnterGameR = 0x00000010,
        ClickLikeL = 0x00000100,
        ClickLikeR = 0x00001000,
    }
}
