using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarracksConstruct : UnitConstruct
{
    public CountToCreateSoldier createSpearman;

    private void Update()
    {
        if (createSpearman.currentCount > 0)
        {
            CreateSpearman();
            createSpearman.currentCount--;
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateSpearman(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Spearman";
    }

    void OnClickCreateSpearman()
    {
        createSpearman.currentCount++;
    }

    void CreateSpearman() 
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Spearman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));
    }
}
