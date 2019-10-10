using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEndTime
{

    private static int startTime;
    private static int endTime;

    public static void StartGame()
    {
        startTime = DateTime.Now.Minute * 60+ DateTime.Now.Second;
    }

    public static void EndGame()
    {
        endTime = DateTime.Now.Minute * 60 + DateTime.Now.Second;

        var i = endTime - startTime;

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("playtime", i.ToString());
        AnalysisController.TraceEvent(EventName.gameEnd, dic);
    }

}
