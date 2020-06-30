using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//然后，写一个实现类，继承运算器接口，实现其方法

public static class StringExtensions /*: ICalculator*/
{
    #region ICalculator 成员

    //两个单位间隔
    private static int unitInterval = 3;
    //单位
    private static string[] units = new string[]
    {
        "","K", "M", "B", "T", "AA","AB","AC","AD","AE","AF","AG","AH","AI",
        "AJ","AK","AL","AM","AN","AO","AP","AQ","AR","AS","AT","AU","AV","AW",
        "AX","AY","AZ","BA","BB","BC","BD","BE","BF","BG","BH","BI","BJ","BK",
        "BL","BM","BN","BO","BP","BQ","BR","BS","BT","BU","BV","BW","BX","BY",
        "BZ","CA","CB","CC","CD","CE","CF","CG","CH","CI","CJ","CK","CL","CM",
        "CN","CO","CP","CQ","CR","CS","CT","CU","CV","CW","CX","CY","CZ","DA",
        "DB","DC","DD","DE","DF","DG","DH","DI","DJ","DK","DL","DM","DN","DO",
        "DP","DQ","DR","DS","DT","DU","DV","DW","DX","DY","DZ","EA","EB","EC",
        "ED","EE","EF","EG","EH","EI","EJ","EK","EL","EM","EN","EO","EP","EQ",
        "ER","ES","ET","EU","EV","EW","EX","EY","EZ","FA","FB","FC","FD","FE",
        "FF","FG","FH","FI","FJ","FK","FL","FM","FN","FO","FP","FQ","FR","FS",
        "FT","FU","FV","FW","FX","FY","FZ","GA","GB","GC","GD","GE","GF","GG",
        "GH","GI","GJ","GK","GL","GM","GN","GO","GP","GQ","GR","GS","GT","GU",
        "GV","GW","GX","GY","GZ","HA","HB","HC","HD","HE","HF","HG","HH","HI",
        "HJ","HK","HL","HM","HN","HO","HP","HQ","HR","HS","HT","HU","HV","HW",
        "HX","HY","HZ","IA","IB","IC","ID","IE","IF","IG","IH","II","IJ","IK",
        "IL","IM","IN","IO","IP","IQ","IR","IS","IT","IU","IV","IW","IX","IY",
        "IZ","JA","JB","JC","JD","JE","JF","JG","JH","JI","JJ","JK","JL","JM",
        "JN","JO","JP","JQ","JR","JS","JT","JU","JV","JW","JX","JY","JZ","KA",
        "KB","KC","KD","KE","KF","KG","KH","KI","KJ","KK","KL","KM","KN","KO",
        "KP","KQ","KR","KS","KT","KU","KV","KW","KX","KY","KZ","LA","LB","LC",
        "LD","LE","LF","LG","LH","LI","LJ","LK","LL","LM","LN","LO","LP","LQ",
        "LR","LS","LT","LU","LV","LW","LX","LY","LZ","MA","MB","MC","MD","ME",
        "MF","MG","MH","MI","MJ","MK","ML","MM","MN","MO","MP","MQ","MR","MS",
        "MT","MU","MV","MW","MX","MY","MZ","NA","NB","NC","ND","NE","NF","NG",
        "NH","NI","NJ","NK","NL","NM","NN","NO","NP","NQ","NR","NS","NT","NU",
        "NV","NW","NX","NY","NZ","OA","OB","OC","OD","OE","OF","OG","OH","OI",
        "OJ","OK","OL","OM","ON","OO","OP","OQ","OR","OS","OT","OU","OV","OW",
        "OX","OY","OZ","PA","PB","PC","PD","PE","PF","PG","PH","PI","PJ","PK",
        "PL","PM","PN","PO","PP","PQ","PR","PS","PT","PU","PV","PW","PX","PY",
        "PZ","QA","QB","QC","QD","QE","QF","QG","QH","QI","QJ","QK","QL","QM",
        "QN","QO","QP","QQ","QR","QS","QT","QU","QV","QW","QX","QY","QZ","RA",
        "RB","RC","RD","RE","RF","RG","RH","RI","RJ","RK","RL","RM","RN","RO",
        "RP","RQ","RR","RS","RT","RU","RV","RW","RX","RY","RZ","SA","SB","SC",
        "SD","SE","SF","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SP","SQ",
        "SR","SS","ST","SU","SV","SW","SX","SY","SZ","TA","TB","TC","TD","TE",
        "TF","TG","TH","TI","TJ","TK","TL","TM","TN","TO","TP","TQ","TR","TS",
        "TT","TU","TV","TW","TX","TY","TZ","UA","UB","UC","UD","UE","UF","UG",
        "UH","UI","UJ","UK","UL","UM","UN","UO","UP","UQ","UR","US","UT","UU",
        "UV","UW","UX","UY","UZ","VA","VB","VC","VD","VE","VF","VG","VH","VI",
        "VJ","VK","VL","VM","VN","VO","VP","VQ","VR","VS","VT","VU","VV","VW",
        "VX","VY","VZ","WA","WB","WC","WD","WE","WF","WG","WH","WI","WJ","WK",
        "WL","WM","WN","WO","WP","WQ","WR","WS","WT","WU","WV","WW","WX","WY",
        "WZ","XA","XB","XC","XD","XE","XF","XG","XH","XI","XJ","XK","XL","XM",
        "XN","XO","XP","XQ","XR","XS","XT","XU","XV","XW","XX","XY","XZ","YA",
        "YB","YC","YD","YE","YF","YG","YH","YI","YJ","YK","YL","YM","YN","YO",
        "YP","YQ","YR","YS","YT","YU","YV","YW","YX","YY","YZ","ZA","ZB","ZC",
        "ZD","ZE","ZF","ZG","ZH","ZI","ZJ","ZK","ZL","ZM","ZN","ZO","ZP","ZQ",
        "ZR","ZS","ZT","ZU","ZV","ZW","ZX","ZY","ZZ"
    };

    public static string ToUnitString(this string str)
    {
        int unitIndex = 0;

        //若达到最大单位,则无需遍历
        if (str.Length >= units.Length * unitInterval)
        {
            unitIndex = units.Length - 1;
        }
        else
        {
            //遍历单位,寻找合适的
            for (; unitIndex < units.Length; unitIndex++)
            {
                if (str.Length <= (unitIndex + 1) * unitInterval)
                {
                    break;
                }
            }
        }
        //整数索引
        int prefixIndex = str.Length - unitIndex * unitInterval;
        //整数位置
        string prefix = str.Substring(0, prefixIndex);
        //小数位置
        string suffix = "";
        if (unitIndex > 0)
        {
            suffix = "." + str.Substring(prefixIndex, 2) + units[unitIndex];
        }

        return prefix + suffix;
    }

    //大整数加法
    public static string Add(this string param1, string param2)
    {
        //将字符串转换成字符数组
        char[] ch1 = param1.ToCharArray();
        char[] ch2 = param2.ToCharArray();

        //将字符串数组反转
        Array.Reverse(ch1);
        Array.Reverse(ch2);

        //调用数组加法
        char[] ch = adds(ch1, ch2);
        //将字符串数组再反转回去
        Array.Reverse(ch);

        //将结果转化成字符串型           
        string result = new string(ch);

        return result;
    }

    //大整数减法
    public static string Sub(this string param1, string param2)
    {
        if (param1.LessOrEqual( param2))
        {
            return "0";
        }

        //将字符串转换成字符数组
        char[] ch1 = param1.ToCharArray();
        char[] ch2 = param2.ToCharArray();

        //将字符串数组反转
        Array.Reverse(ch1);
        Array.Reverse(ch2);

        //调用数组减法
        char[] ch = subs(ch1, ch2);
        //将字符串数组再反转回去
        Array.Reverse(ch);

        //将结果转化成字符串型           
        string result = new string(ch);
        return result;
    }

    public static string Mul(this string param1, float param2)
    {
        string result = param1.Mul(((int)(param2 * 10000)).ToString()).Div("10000");
        return result;
    }
    //大整数乘法
    public static string Mul(this string param1, string param2)
    {
        char[] ch1 = param1.ToCharArray();
        char[] ch2 = param2.ToCharArray();
        char[] ch3 = initArray(1);
        Array.Reverse(ch1);
        Array.Reverse(ch2);
        for (int i = 0; i < ch2.Length; i++)
        {
            int n = int.Parse(ch2[i].ToString());
            char[] chp = addZeros(multiplies(n, ch1), i);
            ch3 = adds(chp, ch3);
        }

        Array.Reverse(ch3);
        string result = new string(ch3);
        return result;
    }

    static int divLength = 4;
    public static string Div(this string param1, string param2, string minValue="1")
    {
        //Dbg.Log(param1 + " Div " + param2);

        if (param1.Equals(param2))
        {
            return "1";
        }
        else if (param1.Less(param2))
        {
            return minValue;
        }

        string prefix = param2;
        if (param2.Length > divLength)
        {
            prefix = param2.Substring(0, divLength);
        }
        //Dbg.Log("prefix " + prefix);
        int multi = Mathf.RoundToInt(100000000 / int.Parse(prefix));
        //Dbg.Log("multi " + multi);
        string result = param1.Mul(multi.ToString());
        //Dbg.Log("result " + result);

        if (param2.Length > divLength)
        {
            //Dbg.Log("div " + (8 + param2.Length - divLength).ToString());
            result = div(result, 8 + param2.Length - divLength);
        }
        else
        {
            //Dbg.Log("div 8");
            result = div(result, 8);
        }
        //Dbg.Log("result " + result);
        return result;

    }

    //大整数大于
    public static bool Greater(this string param1, string param2)
    {
        if (param1.Length == param2.Length)
        {
            return greater(param1.ToCharArray(), param2.ToCharArray());
        }
        else
        {
            return param1.Length > param2.Length;
        }
    }

    //大整数小于
    public static bool Less(this string param1, string param2)
    {
        if (param1.Length == param2.Length)
        {
            return less(param1.ToCharArray(), param2.ToCharArray());
        }
        else
        {
            return param1.Length < param2.Length;
        }
    }

    //大整数大于等于
    public static bool GreaterOrEqual (this string param1, string param2)
    {
        return param1.Greater(param2) || param1 == param2;
    }

    //大整数小于等于
    public static bool LessOrEqual(this string param1, string param2)
    {
        return param1.Less(param2) || param1 == param2;
    }

    #endregion

    //数组加法，字符串运算时转化成字符数组来计算的
    private static char[] adds(char[] ch1, char[] ch2)
    {
        //取较长数组长度
        int length = (ch1.Length > ch2.Length ? ch1.Length : ch2.Length);

        //调用自定义方法代替规范化数组
        char[] cha = initArray(ch1, length);
        char[] chb = initArray(ch2, length);
        char[] chc = initArray(length + 1);

        for (int i = 0; i < length; i++)
        {
            //取字符数组第i个元素化成整型
            int num1 = int.Parse(cha[i].ToString());
            int num2 = int.Parse(chb[i].ToString());
            int num3 = int.Parse(chc[i].ToString());

            //将整数结算结果化成字符数组的形式
            char[] chp = (num1 + num2 + num3).ToString().ToCharArray();
            //将结果填充到存放结果的数组中
            if (chp.Length == 2)
            {
                chc[i] = chp[1];
                chc[i + 1] = chp[0];
            }
            else chc[i] = chp[0];
        }

        //返回结果消除高位的零            
        return delZeros(chc);
    }


    //数组减法，字符串运算时转化成字符数组来计算的
    private static char[] subs(char[] ch1, char[] ch2)
    {
        //取较长数组长度
        int length = (ch1.Length > ch2.Length ? ch1.Length : ch2.Length);

        //调用自定义方法代替规范化数组
        char[] cha = initArray(ch1, length);
        char[] chb = initArray(ch2, length);
        char[] chc = initArray(length+1);

        for (int i = 0; i < length; i++)
        {
            //取字符数组第i个元素化成整型
            int num1 = int.Parse(cha[i].ToString());
            int num2 = int.Parse(chb[i].ToString());
            int num3 = int.Parse(chc[i].ToString());


            if (num1 >= num2 + num3)
            {
                chc[i] = (num1 - num2 - num3).ToString().ToCharArray()[0];
            }
            else
            {
                chc[i + 1] = '1';
                char[] chp = (10 + num1 - num2 - num3).ToString().ToCharArray();
                if (chp.Length > 1)
                {
                    chc[i] = chp[1];
                }
                else
                {
                    chc[i] = chp[0];
                }
            }
        }

        //返回结果消除高位的零            
        return delZeros(chc);
    }

    //比较相同位数的大小
    private static bool greater(char[] ch1, char[] ch2)
    {
        //前几位,若不大于则
        for (int i = 0; i < ch1.Length; i++)
        {
            int num1 = int.Parse(ch1[i].ToString());
            int num2 = int.Parse(ch2[i].ToString());
            if (num1 > num2)
            {
                return true;
            }
            else if(num1 < num2)
            {
                return false;
            }
            else if(num1 == num2)
            {
                continue;
            }
        }
        return false;
    }

    //比较相同位数的大小
    private static bool less(char[] ch1, char[] ch2)
    {
        for (int i = 0; i < ch1.Length; i++)
        {
            int num1 = int.Parse(ch1[i].ToString());
            int num2 = int.Parse(ch2[i].ToString());
            if (num1 < num2)
            {
                return true;
            }
            else if (num1 > num2)
            {
                return false;
            }
            else if (num1 == num2)
            {
                continue;
            }
        }
        return false;
    }

    //个位整数与数组乘法
    private static char[] multiplies(int n, char[] chs)
    {
        //存放结果的数组
        char[] cht = initArray(chs.Length + 1);
        for (int i = 0; i < chs.Length; i++)
        {
            int s = int.Parse(chs[i].ToString());
            int t = int.Parse(cht[i].ToString());
            char[] chp = (n * s + t).ToString().ToCharArray();
            if (chp.Length == 2)
            {
                cht[i] = chp[1];
                cht[i + 1] = chp[0];
            }
            else cht[i] = chp[0];
        }

        //返回结果消除高位的零            
        return delZeros(cht);
    }

    //个位整数与数组除法
    private static string div(string param1,int n)
    {
        string result = param1.Remove(param1.Length - n, n);
        return result;
    }

    //初始化字符串数组，各元素为'0'
    private static char[] initArray(int n)
    {
        char[] ch = new char[n];
        for (int i = 0; i < n; i++)
        {
            ch[i] = '0';
        }
        return ch;
    }

    //规范化数组,实现将原数组中元素复制到目标数组
    //如果目标数组长度小于原数组则截取,
    //如长度大于原数组则补零
    private static char[] initArray(char[] cha, int n)
    {
        char[] chb = new char[n];
        int i;
        for (i = 0; i < n && i < cha.Length; i++)
        {
            chb[i] = cha[i];
        }
        while (i < n)
        {
            chb[i] = '0';
            i++;
        }
        return chb;
    }

    //数组添零，即将原数以十数量级提升
    private static char[] addZeros(char[] ch, int n)
    {
        char[] cho = new char[ch.Length + n];
        int i;

        for (i = 0; i < n; i++)
        {
            cho[i] = '0';
        }
        for (; i < cho.Length; i++)
        {
            cho[i] = ch[i - n];
        }
        return cho;
    }

    //消除高位的零，如果全为零的话，保留一个
    private static char[] delZeros(char[] ch)
    {
        int i;
        for (i = 0; i < ch.Length - 1; i++)
        {
            if (ch[ch.Length - 1 - i] != '0')
                break;
        }
        return initArray(ch, ch.Length - i);
    }

}
