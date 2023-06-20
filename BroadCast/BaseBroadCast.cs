using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBroadCast : MonoBehaviour
{
    public abstract void Init(PlayerInfoPack info, SoldierFaction faction, int value);
}
