using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMoneyLisner : MonoBehaviour
{

    float timeToSpawn;
    float timeToStop;
    SpriteRenderer prefab;

    public void Init(SpawnMoney self,int count,Vector3 to)
    {
        timeToSpawn = self.timeToSpawn;
        timeToStop = self.timeToStop;
        prefab = self.prefab;

        StartCoroutine(_BoombToCollectCurrency(self,count,to));
    }

    //----------- boomb ani -----------------
    IEnumerator _BoombToCollectCurrency(SpawnMoney self,int count, Vector3 to)
    {
        var i = 0;
        List<SpawnChild> objList = new List<SpawnChild>();
        while (i < count)
        {
            var obj = CreateCurrencyObj();
            i++;
            var sc = obj.AddComponent<SpawnChild>();
            objList.Add(sc);
            sc.Init(self, to);
            yield return new WaitForSecondsRealtime(timeToSpawn);
        }


        yield return new WaitForSecondsRealtime(timeToStop);

        foreach (var item in objList)
        {
            item._FlyToPos();
        }

        yield break;
    }

    GameObject CreateCurrencyObj()
    {
        GameObject obj = GameObject.Instantiate(prefab.gameObject);
        obj.SetActive(true);
        obj.transform.SetParent(this.transform);
        obj.transform.position = this.transform.position;
        return obj;
    }


}
