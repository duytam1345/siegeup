using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountToCreateSoldier
{
    public int currentCount;
    public float tToCreate;
    public float tToCreateSecond;
}

public class UnitConstruct : Unit
{
    public Transform posToCreateSoldier;

    public virtual void ShowPanel() { }
}
