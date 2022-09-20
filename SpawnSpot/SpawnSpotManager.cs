using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpotManager : MonoBehaviour
{
    private static SpawnSpotManager _instance;
    public static SpawnSpotManager Instance => _instance;

    [SerializeField]
    float flySpeed;

    [SerializeField, Range(0f, 90f)]
    float radian;

    [SerializeField, Range(0f, 1f)]
    float inflectionPointPercentage = 0.3f;

    [SerializeField]
    Transform targetPos;

    [SerializeField]
    GameObject flyPointObj;


    private void Awake()
    {
        _instance = this;
        flyPointObj.SetActive(false);
    }

    public void SpawnPoint(MainStageChild stageChild)
    {
        //stageChild.ChangeBackImage(null);

        var _newObj = Instantiate(flyPointObj, flyPointObj.transform.parent);

        _newObj.transform.position = stageChild.transform.position;

        _newObj.SetActive(true);

        var _sc =_newObj.AddComponent<InflectionPointParam>();

        SumPos(stageChild.transform.position, targetPos.position,out List<Vector3> newP);

        _sc.StartSinMove(newP, null ,flySpeed);
    }


    private void SumPos(Vector2 startPoint, Vector2 endPoint,out List<Vector3> outPos)
    {

        outPos = new List<Vector3>();

        var _tempPoint = endPoint - startPoint;

        _tempPoint *= inflectionPointPercentage;

        _tempPoint += startPoint;

        var _xOffset = Vector3.Distance(_tempPoint, startPoint) * Mathf.Tan(radian);

        var _offset = Random.Range(0, 2) == 1 ? 1 : -1;

        _tempPoint.x += _xOffset * _offset;

        outPos.Add(startPoint);
        outPos.Add(_tempPoint);
        outPos.Add(endPoint);
    }
}
