using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnMoneyManager : SingletonMonoBehaviour<SpawnMoneyManager>
{
    public SpawnMoney from;
    public Transform to;
    public float offsetScale = 1f;

    public void SpawnCoin(Vector3 startPos, UnityAction onFlyEnded)
    {
        //SoundController.Instance.PlaySound(SoundType.DropCoin);
        from.BoombToCollectCurrency(startPos, 5, to.position, onFlyEnded, offsetScale);
    }
}
