using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GiftCardChosseManager))]
public class GiftCardChosseManagerInspector : Editor
{
    private GiftCardChosseManager giftCardChosseManager;

    private int gridId = 0;

    private int listSize;

    private Vector2 _scrollLogView = Vector2.zero;

    private bool canEdit = false;

    private bool foldout = false;

    private GUIStyle idStyle = new GUIStyle();

    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化对象
    protected SerializedObject _serializedObject2;


    //序列化属性
    protected SerializedProperty _assetLstProperty;
    //序列化属性
    protected SerializedProperty _assetLstProperty2;
    //序列化属性
    protected SerializedProperty _assetLstProperty3;
    //序列化属性
    protected SerializedProperty _assetLstProperty4;

    [SerializeField]//必须要加
    protected List<CountryTypeImageList> _assetLst3 = new List<CountryTypeImageList>();

    [SerializeField]//必须要加
    protected List<Sprite> _assetLst4 = new List<Sprite>();

    private void OnEnable()
    {
        giftCardChosseManager = (GiftCardChosseManager)target;

        //使用当前类初始化
        _serializedObject = new SerializedObject(giftCardChosseManager);
        _serializedObject2 = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("countryGiftImageLists");
        _assetLstProperty2 = _serializedObject.FindProperty("bindList");
        _assetLstProperty3 = _serializedObject2.FindProperty("_assetLst3");
        _assetLstProperty4 = _serializedObject2.FindProperty("_assetLst4");
    }

    public override void OnInspectorGUI()
    {
        //foldout = EditorGUILayout.Foldout(foldout, "折叠原始數據");

        //if (foldout)
        //{
        //    base.OnInspectorGUI();
        //}
            

        //更新
        _serializedObject.Update();
        _serializedObject2.Update();

        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
        {
            canEdit = EditorGUILayout.BeginToggleGroup("修改", canEdit);
            {
                EditorGUILayout.HelpBox("由程序控制的部分，請不要隨意修改", MessageType.Warning);
                giftCardChosseManager.Param 
                    = 
                    (GiftCardChosseParam)EditorGUILayout.ObjectField("param",giftCardChosseManager.Param, typeof(GiftCardChosseParam), true);

                EditorGUILayout.PropertyField(_assetLstProperty2, true);
               
            }
            EditorGUILayout.EndToggleGroup();
        }

        //Resize our list
        listSize = _assetLstProperty.arraySize;
        listSize = EditorGUILayout.IntField("国家种类", listSize);

        if (listSize != _assetLstProperty.arraySize)
        {
            while (listSize > _assetLstProperty.arraySize)
            {
                _assetLstProperty.InsertArrayElementAtIndex(_assetLstProperty.arraySize);
            }
            while (listSize < _assetLstProperty.arraySize)
            {
                _assetLstProperty.DeleteArrayElementAtIndex(_assetLstProperty.arraySize - 1);
            }
        }





        EditorGUILayout.BeginVertical();

        string[] _countrys = new string[giftCardChosseManager.CountryGiftImageLists.Count];
        for (int i = 0; i < giftCardChosseManager.CountryGiftImageLists.Count; i++)
        {
            _countrys[i] = giftCardChosseManager.CountryGiftImageLists[i].country;
        }  
        gridId = GUILayout.SelectionGrid(gridId, _countrys, 4);

        _scrollLogView = GUILayout.BeginScrollView(_scrollLogView, GUILayout.Height(500));

        var _target = giftCardChosseManager.CountryGiftImageLists[gridId];
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("国家", GUILayout.MaxWidth(50));
        _target.country = EditorGUILayout.TextField(_target.country, GUILayout.MaxWidth(50));
        
        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        _assetLst3 = _target.images;
        var listSize2 = _assetLstProperty3.arraySize;
        EditorGUILayout.LabelField("圖片种类", GUILayout.MaxWidth(50));
        listSize2 = EditorGUILayout.IntField(listSize2, GUILayout.MaxWidth(50));
        //Debug.Log( " = " + listSize2);

        

        if (listSize2 != _assetLstProperty3.arraySize)
        {
            while (listSize2 > _assetLstProperty3.arraySize)
            {
                _assetLstProperty3.InsertArrayElementAtIndex(_assetLstProperty3.arraySize);
            }
            while (listSize2 < _assetLstProperty3.arraySize)
            {
                _assetLstProperty3.DeleteArrayElementAtIndex(_assetLstProperty3.arraySize - 1);
            }
        }



        EditorGUILayout.EndHorizontal();

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {
            //提交修改
            _serializedObject2.ApplyModifiedProperties();
        }


        for (int i = 0; i < _assetLst3.Count; i++)
        {
            //开始检查是否有修改
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            EditorGUILayout.Space();


            CountryTypeImageList _newOne = new CountryTypeImageList();
            _newOne.ID = i;
            _assetLst4 = new List<Sprite>(_assetLst3[i].typeImages);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            idStyle.normal.textColor = Color.red;
            idStyle.fontSize = 15;
            idStyle.fixedWidth = 50;

            GUILayout.Label("ID:" + _newOne.ID, idStyle);

            //_newOne.ID = EditorGUILayout.IntField(_newOne.ID, GUILayout.MaxWidth(50));

            GUILayout.Label("圖片數量", GUILayout.MaxWidth(50));

            _serializedObject2.Update();


            var listSize3 = _assetLstProperty4.arraySize;
            listSize3 = EditorGUILayout.IntField(listSize3, GUILayout.MaxWidth(50));

            if (listSize3 != _assetLstProperty4.arraySize)
            {
                while (listSize3 > _assetLstProperty4.arraySize)
                {
                    _assetLstProperty4.InsertArrayElementAtIndex(_assetLstProperty4.arraySize);
                }
                while (listSize3 < _assetLstProperty4.arraySize)
                {
                    _assetLstProperty4.DeleteArrayElementAtIndex(_assetLstProperty4.arraySize - 1);
                }
            }


            GUILayout.EndVertical();


            EditorGUILayout.BeginHorizontal();


            for (int j = 0; j < _assetLst4.Count; j++)
            {
                EditorGUILayout.BeginVertical();
                //GUILayout.Label(_newOne.typeImages[j].name);


                //EditorGUILayout.LabelField(j == 0 ? "左側圖" : "右側圖");
                _assetLst4[j] 
                    = (Sprite)EditorGUILayout.ObjectField(_assetLst4[j], typeof(Sprite), false,GUILayout.Width(150),GUILayout.Height(150));
                
 
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            _newOne.typeImages = _assetLst4;
            _assetLst3[i] = _newOne;
            //结束检查是否有修改
            if (EditorGUI.EndChangeCheck())
            {
                //提交修改
                _serializedObject2.ApplyModifiedProperties();
            }
        }

        


        GUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {
            //提交修改
            _serializedObject2.ApplyModifiedProperties();
            _serializedObject.ApplyModifiedProperties();
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(giftCardChosseManager);
    }
}
