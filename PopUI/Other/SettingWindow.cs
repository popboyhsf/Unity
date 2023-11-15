﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : PopUIBase
{
    public override string thisPopUIEnum => PopUIEnum.SettingWindow.ToString();

    public override string thisUIType => PopUIType.POP.ToString();

    [SerializeField]
    Button closs, music, vibrator, retry;
    [SerializeField]
    GameObject musicOn, musicoOff, vibratorOn, vibratorOff;

    private bool isMusic = false;
    private bool IsVibrator = false;



    private void Awake()
    {
        buttonlist.Add(music);
        buttonlist.Add(vibrator);

        if (music) music.onClick.AddListener(MusicBtn);
        if (vibrator) vibrator.onClick.AddListener(SoundBtn);
        if (closs) closs.onClick.AddListener(HiddenUIAI);
        if (retry) retry.onClick.AddListener(RetryGame);


        isMusic = SettingData.Music;
        IsVibrator = SettingData.Vibrator;

        ChangeMusicState(SettingData.Music);
        ChangeVibratorState(SettingData.Vibrator);
    }


    private void MusicBtn()
    {
        SettingData.Music = !SettingData.Music;
        ChangeMusicState(SettingData.Music);
    }

    private void SoundBtn()
    {
        SettingData.Vibrator = !SettingData.Vibrator;
        ChangeVibratorState(SettingData.Vibrator);
    }

    private void RetryGame()
    {
        //TODO 
        HiddenUIAI();
    }

    private void ChangeMusicState(bool _is)
    {
        if (isMusic != _is)
        {
            SoundController.Instance.IsSoundOn = _is;
            isMusic = _is;
        }

        if (_is)
        {
            if (musicOn) musicOn.SetActive(true);
            if (musicoOff) musicoOff.SetActive(false);
        }
        else
        {
            if (musicOn) musicOn.SetActive(false);
            if (musicoOff) musicoOff.SetActive(true);
        }
    }
    private void ChangeVibratorState(bool _is)
    {
        if (IsVibrator != _is)
        {
            UtilsAndroid.IsVibrator = _is;
            IsVibrator = _is;
        }
        if (_is)
        {
            if (vibratorOn) vibratorOn.SetActive(true);
            if (vibratorOff) vibratorOff.SetActive(false);
        }
        else
        {
            if (vibratorOn) vibratorOn.SetActive(false);
            if (vibratorOff) vibratorOff.SetActive(true);
        }
    }

    public override void BeforShow(object[] value)
    {

    }

    public override void AfterHiddenUI()
    {

    }
}

public static class SettingData
{
    private const string flag = "SettingData";
    private const string musicflag = "music";
    private const string vibratorflag = "vibrator";


    public static bool Music
    {
        get => PlayerPrefs.GetInt(flag + "_" + musicflag, 1).IntToBool();
        set => PlayerPrefs.SetInt(flag + "_" + musicflag, value.BoolToInt());
    }
    public static bool Vibrator
    {
        get => PlayerPrefs.GetInt(flag + "_" + vibratorflag, 1).IntToBool();
        set => PlayerPrefs.SetInt(flag + "_" + vibratorflag, value.BoolToInt());
    }
}