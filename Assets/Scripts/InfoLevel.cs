using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewObject
{
    public enum NameObject
    {
        CityHall,
        House,
        Farm,
        ArcherRang,
        Barracks,
        Tower,
        Peasant,
        Spearman,
        Archerman,
        Swordsman,
        Crossbowman,


    }


    public NameObject nameObject;
}

[CreateAssetMenu(fileName = "Level", menuName = "Info Level")]
public class InfoLevel : ScriptableObject
{
    public string nameLevel;

    public string infoLevel;

    public NewObject[] newObjects;
}
