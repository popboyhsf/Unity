package com.unity3d.player;

import android.content.Context;
import android.os.Build;
import android.os.LocaleList;
import android.telephony.TelephonyManager;
import android.text.TextUtils;
import java.util.Locale;

public class CountryCodePlugin {

    // 获取国家缩写名称
    public static String getCountry(Context context) {
        TelephonyManager telephonyManager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        String country = telephonyManager.getSimCountryIso().toUpperCase();

        if (TextUtils.isEmpty(country)) {
            country = getLocale(context).getCountry().toUpperCase(Locale.US);
        }

        if (TextUtils.isEmpty(country)) {
            country = "US";
        }
        return country;
    }

    // 获取语言代码
    public static String getLanguage(Context context) {
        return getLocale(context).getLanguage().toLowerCase(Locale.US);
    }

    private static Locale getLocale(Context context) {
        Locale locale;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            locale = LocaleList.getDefault().get(0);
        } else {
            locale = context.getResources().getConfiguration().locale;
        }
        return locale;
    }
}