using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarracksConstruct : UnitConstruct
{
    public CountToCreateSoldier createSpearman;

    private void Update()
    {
        if (createSpearman.t > 0)
        {
            createSpearman.t -= Time.deltaTime;
        }

        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (createSpearman.Update())
        {
            CreateSpearman();
        }
    }

    public override void ShowPanel()
    {
        GameObject g = Instantiate(Resources.Load("UI/Slot Button") as GameObject, Manager.manager.contentInfoPanel);
        g.GetComponent<Button>().onClick.AddListener(delegate { OnClickCreateSpearman(); });
        g.transform.GetChild(0).GetComponent<Text>().text = "Spearman";

        createSpearman.fillAmount = g.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        createSpearman.textAmount = g.transform.GetChild(1).GetChild(1).GetComponent<Text>();
    }

    void OnClickCreateSpearman()
    {
        if (Manager.manager.resourcesGame._food >= 15 && Manager.manager.resourcesGame._gold >= 10)
        {
            Manager.manager.resourcesGame._food -= 15;
            Manager.manager.resourcesGame._gold -= 10;
            Manager.manager.UpdateresourcesGame();

            createSpearman.currentCount++;
        }
    }

    void CreateSpearman()
    {
        GameObject g = Instantiate(Resources.Load("Soldier/Spearman") as GameObject, posToCreateSoldier.position, Quaternion.identity);
        g.GetComponent<UnitSoldier>().SetMove(new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z - 2));
    }

    public override void TakeDamage(Property property)
    {
        _property.curHealth -= property.dmgSoldier;
        UpdateHealthBar();
    }
}
