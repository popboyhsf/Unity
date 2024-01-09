using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdateEffect : MonoBehaviour
{
    private float m_startNum;
    private float m_endNum;
    private float m_curNum;
    private float m_duration;
    private Image m_slider;
    private float perFrameNum;//每帧变化量
                            // Use this for initialization
    private bool isStart = false;
    private float costTime;

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            costTime += Time.deltaTime;
            if (costTime >= m_duration)
            {
                m_slider.fillAmount = m_endNum;
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
                m_slider.fillAmount = m_curNum;
            }
        }
    }

    public void Init(Image image, float startNum, float endNum, float duration)
    {
        m_slider = image;
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

    public static void ShowEffect(Image image, float finalNum, float duration)
    {
        SliderUpdateEffect effect = image.gameObject.GetComponent<SliderUpdateEffect>();
        if (effect == null)
        {
            effect = image.gameObject.AddComponent<SliderUpdateEffect>();
        }
        float tstartNum;
        if (!float.TryParse(image.fillAmount.ToString(), out tstartNum))
        {
            Debug.LogError("this Text is no a Slider compment!");
            return;
        }
        if (tstartNum >= finalNum) return;
        effect.Init(image, tstartNum, finalNum, duration);
    }
}
