using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundClick : MonoBehaviour
{
    [SerializeField]
    SoundType soundType;

    private Button button;
    private Button thisButton 
    {
        get
        {
            if (button == null)
                button = this.GetComponent<Button>();

            return button;
        }
    }


    private void Awake()
    {
        thisButton.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        SoundController.Instance.PlaySound(soundType);
    }
}
