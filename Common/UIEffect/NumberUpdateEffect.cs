using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//数字刷新效果
public class NumberUpdateEffect : MonoBehaviour
{
    enum EType
    {
        TEXT,
        TEXTPRO,
        NULL
    }
    private int m_startNum;
    private int m_endNum;
    private int m_curNum;
    private float m_duration;
    private Text m_text;
    private TextMeshProUGUI m_textPro;
    private int perFrameNum;//每帧变化量
                            // Use this for initialization
    private bool isStart = false;
    private float costTime;
    private EType mType = EType.NULL;

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            costTime += Time.deltaTime;
            if (costTime >= m_duration)
            {
                switch (mType)
                {
                    case EType.TEXT:
                        m_text.text = m_endNum.ToString();
                        break;
                    case EType.TEXTPRO:
                        m_textPro.text = m_endNum.ToString();
                        break;
                    case EType.NULL:
                        break;
                    default:
                        break;
                }
                
                isStart = false;
            }
            else
            {
                m_curNum += perFrameNum;
                if ((perFrameNum > 0 && m_curNum > m_endNum) || (perFrameNum < 0 && m_curNum < m_endNum))
                {
                    m_curNum = m_endNum;
                    isStart = false;
                }
                switch (mType)
                {
                    case EType.TEXT:
                        m_text.text = m_curNum.ToString();
                        break;
                    case EType.TEXTPRO:
                        m_textPro.text = m_curNum.ToString();
                        break;
                    case EType.NULL:
                        break;
                    default:
                        break;
                }
               
            }
        }
    }

    public void Init(Text text, int startNum, int endNum, float duration)
    {
        mType = EType.TEXT;
        m_text = text;
        m_startNum = startNum;
        m_endNum = endNum;
        m_duration = duration;
        perFrameNum = (m_endNum - m_startNum) / 15;
        if (perFrameNum == 0)
            perFrameNum = (m_endNum - m_startNum) / Mathf.Abs(m_endNum - m_startNum);
        costTime = 0;
        m_curNum = startNum;
        isStart = true;
    }

    public void Init(TextMeshProUGUI text, int startNum, int endNum, float duration)
    {
        mType = EType.TEXTPRO;
        m_textPro = text;
        m_startNum = startNum;
        m_endNum = endNum;
        m_duration = duration;
        perFrameNum = (m_endNum - m_startNum) / 15;
        if (perFrameNum == 0)
            perFrameNum = (m_endNum - m_startNum) / Mathf.Abs(m_endNum - m_startNum);
        costTime = 0;
        m_curNum = startNum;
        isStart = true;
    }

    public static void ShowEffect(Text text, int finalNum, float duration)
    {
        NumberUpdateEffect effect = text.gameObject.GetComponent<NumberUpdateEffect>();
        if (effect == null)
        {
            effect = text.gameObject.AddComponent<NumberUpdateEffect>();
        }
        int tstartNum;
        if (!int.TryParse(text.text, out tstartNum))
        {
            Debug.LogError("this Text is no a Number text!");
            return;
        }
		if(tstartNum==finalNum)return;
        effect.Init(text, tstartNum, finalNum, duration);
    }

    public static void ShowEffect(TextMeshProUGUI text, int finalNum, float duration)
    {
        NumberUpdateEffect effect = text.gameObject.GetComponent<NumberUpdateEffect>();
        if (effect == null)
        {
            effect = text.gameObject.AddComponent<NumberUpdateEffect>();
        }
        int tstartNum;
        if (!int.TryParse(text.text, out tstartNum))
        {
            Debug.LogError("this Text is no a Number text!");
            return;
        }
        if (tstartNum == finalNum) return;
        effect.Init(text, tstartNum, finalNum, duration);
    }
}
