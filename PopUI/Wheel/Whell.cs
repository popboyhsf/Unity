using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Whell : PopUIBase
{

    [SerializeField]
    Button onStart;


    [Header("旋转物体")]
    [SerializeField]
    RectTransform rotateRtf;

    [SerializeField]
    int prizeCount, adjustAngle;
    [Header("停止转动随机偏移百分比")]
    [SerializeField]
    float offsetRange = 0.3f;



    private int perPrizeAngle;
    private Vector3 curRotate = new Vector3(0, 0, 0);

    private bool _isRuning = false;

    private float _getWhellNum = 0f;

    //获取物品的ID
    modeEnum itemID = modeEnum.UnKnow;

    private void Awake()
    {
        onStart.onClick.AddListener(onBtnStartClick);
    }

    //点击开始的回调
    void onBtnStartClick()
    {
        if (_isRuning) return;

        if (PowerSystem.Instance.StartOnGame())
            CallBackBtnStartClick();
        else
            Debug.LogWarning("体力不足");

        
    }

    void CallBackBtnStartClick()
    {
        itemID = WheelData.GetRandomItemID();

        Debug.Log("item ==== " + itemID);



        perPrizeAngle = 360 / prizeCount;
        _isRuning = true;
        if(itemID != modeEnum.UnKnow)
            StartRotate((int)itemID - 1);
    }



    //转盘旋转
    private void StartRotate(int stopIndex)
    {
        float finalAngle = (perPrizeAngle * stopIndex) + adjustAngle +
            Random.Range(-perPrizeAngle * offsetRange, perPrizeAngle * offsetRange);

        finalAngle += Random.Range(4, 7) * 360;

        Vector3 finalRotate = new Vector3(0, 0, finalAngle);

        DOTween.To(() => curRotate, param => curRotate = param, finalRotate, 5.0f)
            .OnUpdate(() => rotateRtf.localRotation = Quaternion.Euler(curRotate)).OnComplete(Stop);
    }

    private void Stop()
    {
        curRotate = new Vector3(0, 0, curRotate.z % 360);
        rotateRtf.localRotation = Quaternion.Euler(curRotate);

        _isRuning = false;

        StartCoroutine(CallClossUI());
    }

    IEnumerator CallClossUI()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        ClossUI();
    }


    public void ClossUI()
    {
        if (_isRuning) return;
        HiddenUIAI();
    }

    public override void FSetBeforShow()
    {
        
    }

    public override void FSetAfterHiddenUI()
    {
        PopUIManager.Instance.StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        yield return null;
        var mode = gameStage.UnKnow;

        switch (itemID)
        {
            case modeEnum.normalMode:
                mode = gameStage.normalMode;
                break;
            case modeEnum.findMode:
                mode = gameStage.findMode;
                break;
            case modeEnum.selectMode:
                mode = gameStage.selectMode;

                break;
            case modeEnum.selectMode2:
                mode = gameStage.selectMode;
                break;
            case modeEnum.selectMode3:
                mode = gameStage.selectMode;
                break;
            case modeEnum.rewardMode:
                mode = gameStage.rewardMode;
                break;
            case modeEnum.UnKnow:
                break;
            default:
                break;
        }
        MainGame.Instance.SelectStart(mode,itemID);

        yield break;
    }
}
