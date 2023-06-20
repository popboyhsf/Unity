using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NormalInfoBox : BaseBroadCast
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    Image playerIcon;


    public override void Init(PlayerInfoPack info, SoldierFaction faction, int value)
    {
        playerName.text = info.PlayerName;

        PlayerIconManager.Instance.GetPlayerIcon(playerIcon, info);
    }


}
