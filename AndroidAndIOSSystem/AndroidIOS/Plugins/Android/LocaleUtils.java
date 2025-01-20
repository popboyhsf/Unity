package com.unity3d.player;

import android.content.Context;
import android.os.Build;
import android.os.LocaleList;
import android.telephony.TelephonyManager;
import android.text.TextUtils;

import java.util.Locale;

public class LocaleUtils {

    /**
     * 获取国家缩写名称，首先从 SIM 卡读取，无法读出再从 Android 底层 Locale 中读取
     */
    public static String getCountryCode(Context context) {
        TelephonyManager telephonyManager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        String countryCode = telephonyManager.getSimCountryIso().toUpperCase();

        if (TextUtils.isEmpty(countryCode)) {
            countryCode = getDeviceCountryCode();
        }

        if (TextUtils.isEmpty(countryCode)) {
            countryCode = "US"; // 默认为美国
        }
        return countryCode;
    }

    /**
     * 获取设备的语言代码
     */
    public static String getLanguageCode() {
        return getLocale().getLanguage().toLowerCase(Locale.US);
    }

    private static Locale getLocale() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            return LocaleList.getDefault().get(0);
        } else {
            return Locale.getDefault();
        }
    }

    private static String getDeviceCountryCode() {
        return getLocale().getCountry().toUpperCase(Locale.US);
    }
}
