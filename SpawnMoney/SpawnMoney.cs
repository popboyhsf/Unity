using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SpawnMoney : MonoBehaviour
{

    [Header("[Count Param]")]
    public int maxIcon; //最大Icon数量
    public int minIcon; //最少Icon数量
    [Header("[Boomb Param]")]
    public float range; //炸出范围
    public float timeToSpawn;  //产生间隔
    public float timeToBoom;  //飞出时间
    public float timeToStop;  //飞出后停滞的时间
    public float timeToCollect; //飞去固定位置的时间

    [Header("[Drop Param]")]
    public bool isDrop; //是否是掉落模式
    public float gravity; //重力
    public float dropY;  //掉落高度偏差
    public float dropYOffsetMin;  //掉落高度偏差偏移最小
    public float dropYOffsetMax;  //掉落高度偏差偏移最大
    public float dropCount;  //掉落次数
    public float dropUpSpeed;  //弹起速度
    public float dropUpSpeedWeaken; //弹起衰减

    [Header("[UnityObj]")]
    public SpriteRenderer prefab; //复制的预设

    private GameObject obj;

    private void Start()
    {
        prefab.gameObject.SetActive(false);
    }


    //--------------- function ------------
    public void BoombToCollectCurrency(Vector3 startPos, int count, Vector3 to, UnityAction onFlyEnded = null,float offsetScale = 1f)
    {
        obj = new GameObject("parentOfCurrency");
        obj.transform.position = startPos;
        obj.transform.localScale *= offsetScale;

        int coinCount = Mathf.Clamp(count, minIcon, maxIcon);

        obj.AddComponent<SpawnMoneyLisner>().Init(this, coinCount, to);

        var timer = timeToSpawn * coinCount + timeToBoom + timeToStop + timeToCollect + 0.5f;

        obj.AddComponent<DelayDestroy>().Init(timer, onFlyEnded);
    }




}