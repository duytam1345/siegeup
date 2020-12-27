using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CountToCreateSoldier
{
    public Image fillAmount;
    public Text textAmount;

    //player
    public int currentCount;
    public float tToCreate;
    public float tToCreateSecond;

    //enemy
    public float t;
    public void StartCreate()
    {
        t = tToCreate;
    }

    public bool Update()
    {
        if (fillAmount)
        {
            fillAmount.fillAmount = currentCount > 0 ? tToCreateSecond / tToCreate : 0;
        }

        if (textAmount)
        {
            textAmount.text = currentCount > 0 ? currentCount.ToString() : "";
        }

        if (Manager.manager.testMode)
        {
            if (currentCount > 0)
            {
                currentCount--;
                return true;
            }
        }
        else
        {
            if (currentCount > 0)
            {
                tToCreateSecond -= Time.deltaTime;

                if (tToCreateSecond <= 0)
                {
                    currentCount--;
                    tToCreateSecond = tToCreate;
                    return true;
                }
            }
        }

        return false;
    }
}

public class UnitConstruct : Unit
{
    public Transform posToCreateSoldier;

    public virtual void ShowPanel() { }

    public void CheckToEffectFire(int b, int a)
    {
        Vector3 v = transform.position;

        v.x += Random.Range(-1.5f, 1.5f);
        v.z += Random.Range(-1.5f, 1.5f);

        if (b >= 75 && a < 75)
        {
            Manager.manager.CreateFireEffect(v, transform.GetChild(2));
        }
        else if (b >= 50 && a < 50)
        {
            Manager.manager.CreateFireEffect(v, transform.GetChild(2));
        }
        else if (b >= 25 && a < 25)
        {
            Manager.manager.CreateFireEffect(v, transform.GetChild(2));
        }
    }
}
