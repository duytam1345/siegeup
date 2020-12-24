using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherRangeConstruct : UnitConstruct
{
    public CountToCreateSoldier createArcherman;

    private void Update()
    {
        if (createArcherman.t > 0)
        {
            createArcherman.t -= Time.deltaTime;
        }

        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (createArcherman.Update())
        {
            CreateArcherman();
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateArcherman(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Archerman";

        createArcherman.fillAmount = g.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createArcherman.textAmount = g.transform.GetChild(1).GetChild(1).GetComponent<Text>();
    }

    void OnClickCreateArcherman()
    {
        if (Manager.manager.resourcesGame._food >= 15 && Manager.manager.resourcesGame._wood >= 15)
        {
            Manager.manager.resourcesGame._food -= 15;
            Manager.manager.resourcesGame._wood -= 15;
            Manager.manager.UpdateresourcesGame();

            createArcherman.currentCount++;
        }
    }

    void CreateArcherman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Archerman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));

        Manager.manager.AddToCurrentSoldier(g.GetComponent<UnitSoldier>());
    }
}
