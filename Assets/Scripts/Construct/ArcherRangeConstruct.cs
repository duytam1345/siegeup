using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherRangeConstruct : UnitConstruct
{
    public CountToCreateSoldier createArcherman;

    private void Update()
    {
        if (createArcherman.currentCount > 0)
        {
            CreateArcherman();
            createArcherman.currentCount--; 
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateArcherman(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Archerman";
    }

    void OnClickCreateArcherman()
    {
        createArcherman.currentCount++;
    }

    void CreateArcherman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Archerman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));
    }
}
