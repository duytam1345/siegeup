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
}
