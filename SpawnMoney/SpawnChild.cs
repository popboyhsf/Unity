using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChild : MonoBehaviour
{

    //--------------- private -------------
    float range;
    float timeToBoom;
    float timeToCollect;


    bool isDrop; //是否是掉落模式
    float dropY;  //掉落高度偏差
    float dropYOffsetMin;  //掉落高度偏差偏移最小
    float dropYOffsetMax;  //掉落高度偏差偏移最大
    float dropCount;  //掉落次数
    float dropUpSpeed;  //弹起速度
    float dropUpSpeedWeaken; //弹起衰减

    float gravity = 9.8f; 

    Vector3 to;
    Vector3 toY;

    public void Init(SpawnMoney self, Vector3 to)
    {
        range = self.range;
        timeToBoom = self.timeToBoom;
        timeToCollect = self.timeToCollect;


        isDrop = self.isDrop;
        dropY = self.dropY;
        dropYOffsetMin = self.dropYOffsetMin;
        dropYOffsetMax = self.dropYOffsetMax;

        dropY += Random.Range(dropYOffsetMin, dropYOffsetMax);

        dropCount = self.dropCount;
        dropUpSpeed = self.dropUpSpeed;
        dropUpSpeedWeaken = self.dropUpSpeedWeaken;


        gravity = self.gravity;

        _BoombToCollectCurrency(to);
    }

    //----------- boomb ani -----------------
    void _BoombToCollectCurrency(Vector3 to)
    {
        this.to = to;
        StartCoroutine(ExporeOut()); 
    }

    public void _FlyToPos()
    {
        StartCoroutine(FlyToPos(to));
    }

    IEnumerator ExporeOut()
    {
        float timer = 0;
        Vector3 pos = Random.insideUnitCircle;
        if (isDrop) pos = new Vector3(pos.x, pos.y < 0 ? pos.y * -1 : pos.y, pos.z);
        Vector3 from = this.transform.position;
        Vector3 to = pos * range + this.transform.position;
        toY = new Vector3(to.x, this.transform.parent.position.y - dropY, to.z);

        while (timer < timeToBoom)
        {
            timer += Time.deltaTime;

            if (isDrop)
            {
                var pos1 = Vector3.Lerp(to,toY, timer / timeToBoom);
                pos = Vector3.Lerp(this.transform.position, pos1, timer / timeToBoom);

                this.transform.position = pos;
            }
            else
            {
                pos = Vector3.Lerp(this.transform.position, to, timer / timeToBoom);
                this.transform.position = pos;
            }
            yield return null;
        }

        if (isDrop && Vector3.Distance(toY, pos) <= 0.05f)
        {
           
            StartCoroutine(Drop());
        }


    }
    IEnumerator Drop()
    {
        var jumpCount = 0;
        var timer = 0f;
        var speed = dropUpSpeed;
        var canJump = true;
        while (true)
        {
            timer += Time.deltaTime;
            speed -= gravity * timer;

            this.transform.Translate(Vector3.up * speed * Time.deltaTime);

            this.transform.position = new Vector3(this.transform.position.x,Mathf.Max(this.transform.position.y,toY.y), this.transform.position.z);

            if (Vector3.Distance(this.transform.position, toY) > 0.1f)
                canJump = true;

            if (canJump && jumpCount < dropCount && Vector3.Distance(this.transform.position, toY) <= 0.05f)
            {
                jumpCount++;
                canJump = false;
                speed = dropUpSpeed * Mathf.Pow(dropUpSpeedWeaken, jumpCount);
                timer = 0;
            }

            

            yield return null;

        }
    }


    IEnumerator FlyToPos(Vector3 to)
    {
        float timer = 0;
        Vector3 from = this.transform.position;     

        while (timer < timeToCollect)
        {
            timer += Time.deltaTime;
            Vector3 pos = Vector3.Lerp(from, to, timer / timeToCollect);
            this.transform.position = pos;
            yield return null;
        }

        Destroy(this.gameObject);
        yield break;

    }
}
