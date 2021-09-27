using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PowerSystem : MonoBehaviour
{
    [SerializeField]
    List<Text> timeT;
    [SerializeField]
    List<TextMeshProUGUI> timeTPro;
    [SerializeField]
    List<Text> numT;
    [SerializeField]
    List<TextMeshProUGUI> numTPro;

    [Header("扣体力特效相关")]
    [SerializeField]
    GameObject dec_eff_obj;
    [SerializeField]
    float dec_eff_lerp_time = 1f;

    private static PowerSystem _instance;

    public static PowerSystem Instance { get => _instance; }

    


    private double reTime;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Refresh();
        StartTime();
    }

    private void Update()
    {
        CheckPower(Time.deltaTime);
    }

    /// <summary>
    /// 游戏开始初始化
    /// </summary>
    public void StartTime()
    {
        var time = PowerData.GetTimer;
        var _timeLimit = PowerData.PowerRestore;
        while (time > _timeLimit)
        {
            time -= _timeLimit;
            RestorePower(time);
        }

        if (PowerData.GetPower >= PowerData.power)
        {
            time = 0;
        }

        this.reTime = PowerData.PowerRestore - time;
    }

    /// <summary>
    /// 开始一局游戏
    /// </summary>
    /// <returns>返回能否开始</returns>
    public bool StartOnGame()
    {
        if (PowerData.GetPower <= 0) return false;
        else
        {
            PowerData.SetPower = PowerData.GetPower - PowerData.powerOnce;

            if (PowerData.GetPower >= PowerData.power - 1)
            {
                PowerData.SetTimer = DateTime.Now.ToString();
                this.reTime = PowerData.PowerRestore;
            }

            Refresh();
            SpawnDecPowerEff();


            Debuger.Log("剩余体力 == "+ PowerData.GetPower);
            Debuger.Log("日期 == "+ PowerData.GetTimer);
            return true;
        }
    }

    /// <summary>
    /// 进入体力倒计时阶段
    /// </summary>
    /// <param name="dt">时间</param>
    public void CheckPower(float dt)
    {
        if (PowerData.GetPower < PowerData.power)
        {
            if (reTime <= 0)
            {
                RestorePower();
            }
            else
            {
                reTime -= dt;

                timeT.ForEach(t=> { 
                    t.text = reTime.doubleToData();
                });
                timeTPro.ForEach(t => {
                    t.text = reTime.doubleToData();
                });
                //console.log("倒计时 == " + this.Timer);
            }
            //if (!timeTxt.gameObject.activeSelf) timeTxt.gameObject.SetActive(true);
        }
        else
        {
            timeT.ForEach(t => {
                t.text = "00:00";
            });
            timeTPro.ForEach(t => {
                t.text = "00:00";
            });
            //if (timeTxt.gameObject.activeSelf) timeTxt.gameObject.SetActive(false);
        }
           
    }

    /// <summary>
    /// 增加1点体力
    /// </summary>
    public void RestorePower(double time = 0)
    {
        if (PowerData.GetPower >= PowerData.power) return;

        reTime = PowerData.PowerRestore;

        PowerData.SetPower = PowerData.GetPower + 1;

        PowerData.SetCount = PowerData.GetCount + 1;

        if (PowerData.GetPower >= PowerData.power)
        {
            PowerData.SetTimer = "";
        }
        else
        {
            
            PowerData.SetTimer = DateTime.Now.AddSeconds(-time).ToString();
        }


        Refresh();

        Debuger.Log("增加体力到 == " + PowerData.GetPower);
    }

    /// <summary>
    /// 看广告增加体力
    /// </summary>
    public void RestorePowerAD()
    {
        PowerData.SetPower = PowerData.GetPower + PowerData.powerAD;

        if (PowerData.GetPower >= PowerData.power)
        {
            PowerData.SetTimer = "";
        }

        Refresh();

    }

    /// <summary>
    /// 从游戏中获得体力
    /// </summary>
    public void RestorePowerGame(int itemBoxCount)
    {
        PowerData.SetPower = PowerData.GetPower + itemBoxCount;

        if (PowerData.GetPower >= PowerData.power)
        {
            PowerData.SetTimer = "";
        }

        Refresh();

    }


    /// <summary>
    /// 刷新
    /// </summary>
    public void Refresh()
    {
        numT.ForEach(t => {
            t.text = PowerData.GetPower.ToString();
        });
        numTPro.ForEach(t => {
            t.text = PowerData.GetPower.ToString();
        });
    }

    private void SpawnDecPowerEff()
    {
        var _sc = Instantiate(dec_eff_obj, dec_eff_obj.transform.parent).AddComponent<DelayDestroy>();
        _sc.gameObject.SetActive(true);
        _sc.Init(dec_eff_lerp_time,null);
    }
}
