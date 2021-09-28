using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FackI2Language : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    I2Language.LanguageEnum language;


    private static FackI2Language _instance;

    private I2Language.LanguageEnum languageLocal;

    public static FackI2Language Instance { get => _instance;}
    public I2Language.LanguageEnum LanguageLocal { get => languageLocal;}
    public I2Language.LanguageEnum Language { get => language; }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (languageLocal != language)
        {
            I2Language.Instance.ApplyLanguage(language);
            languageLocal = language;
        }
    }
#endif
}
